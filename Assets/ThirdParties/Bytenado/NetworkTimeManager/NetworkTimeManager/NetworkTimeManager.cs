// Bytenado, LLC
// support@bytenado.com
// https://www.bytenado.com/terms.html
// Version 1.0.0

using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bytenado
{
    /// <summary>
    /// Provides a current UTC network time from a group of NTP servers on the internet.
    ///     <list type="bullet">
    ///     <item>The time is directly synced with the current device system time.</item>
    ///     <item>If internet connection is lost, the time continues uninterrupted as long as the system time has not
    ///     changed significantly between update frames and the applicaiton has not been paused very long.</item>
    ///     <item>Exceptions are caught and shown in the debug log.</item>
    ///     </list>
    /// </summary>
    public sealed class NetworkTimeManager : MonoBehaviour
    {
        // Editor-only members.
        #if UNITY_EDITOR
        [Header("[Unity Editor Only]")]
        [Tooltip("Show messages to indicate the status of the network time in the debug log.")]
        [SerializeField] private bool _showDebugMessages = true;
        [Tooltip("Show warnings to indicate an issue that could cause the app to not function correctly.")]
        [SerializeField] private bool _showDebugWarnings = true;
        private bool _applicationPaused;
        #endif

        // Network communication and sync settings.
        private const int NtpServerCooldownSeconds = 64;            // Time to wait before requesting time from same NTP server (64 to 1024).
        private const int NetworkTimeoutMilliseconds = 2000;        // Time to wait on a NTP server response before canceling.
        private const int NtpRequestMaxFails = 2;                   // Allowed timeouts to a NTP server before excluding it as a request option.
        private const int AllowedOffSyncSeconds = 1;                // Allowed time change between network time and system time each update frame.
        private const int AllowedPauseSeconds = 10;                 // Allowed time to be paused before requiring a resync to a NTP server.
        private const float WaitForNetworkMinSeconds = 1f;          // Minimum time to wait before trying to connect to a NTP server after all connections failed.
        private const float WaitForNetworkMaxSeconds = 60f;         // Maximum time to wait before trying to connect to a NTP server as each failed attempt increases the wait time by 1 second.
        private const string SaveNtpTimesName = "NextNtpTimes";     // PlayerPref name to save next request times for NTP servers.

        // NTP message data settings.
        private const int NtpUdpPort = 123;                         // Standard NTP port.
        private const int NtpMessageBytes = 48;                     // Standard NTP message size.
        private const byte NtpRequestHeader = 0x1B;                 // Standard NTP message header.
        private const int NtpSecondsOffsetByte = 40;                // Standard byte position for current time seconds in NTP message.
        private const int NtpFractionOfSecondOffsetByte = 44;       // Standard byte position for current time fraction of seconds in NTP message.

        // NTP servers by priority (1 to 12).
        private (string DomainNameAddress, DateTime NextRequestTime, int FailCount)[] _ntpServers = new(string, DateTime, int)[9] {
            ( "time.cloudflare.com", DateTime.MinValue, 0 ),
            ( "time.google.com", DateTime.MinValue, 0 ),
            ( "pool.ntp.org", DateTime.MinValue, 0 ),
            ( "time.aws.com", DateTime.MinValue, 0 ),
            ( "time.windows.com", DateTime.MinValue, 0 ),
            ( "time.apple.com", DateTime.MinValue, 0 ),
            ( "time.nist.gov", DateTime.MinValue, 0 ),
            ( "clock.isc.org", DateTime.MinValue, 0 ),
            ( "ntp.ubuntu.com", DateTime.MinValue, 0 )
        };

        // Network time management.
        private DateTime _epochTime = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);   // Standard beginning of NTP time. New epoch on February 7, 2036.
        private DateTime _networkTimeUtc;                                           // Current network time for the app to use.
        private DateTime _appPauseTime;                                             // System time the app was paused at.
        private TimeSpan _timeDifferenceUtc;                                        // Difference between the NTP time and system time.
        private bool _timeInSync;                                                   // To check if network time is correct before using it.
        private bool _currentlySyncingTime;                                         // Prevent multiple simultaneous NTP server syncs.

        /// <summary>
        /// Singleton instance of the <see cref="NetworkTimeManager"/> object to reference.
        /// </summary>
        public static NetworkTimeManager Instance { get; private set; }

        /// <summary>
        /// The current network time in UTC.
        ///     <list type="bullet">
        ///     <item>Check <see cref="IsTimeInSync"/> to make sure the time is in sync before using this value.</item>
        ///     </list>
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///     <item>The time in UTC from a NTP sever.</item>
        ///     <item><see cref="DateTime.MinValue"/> if a NTP server has never been reached since starting the application.</item>
        ///     <item>The last valid network time before losing sync and needing to receive time from a NTP server.</item>
        ///     </list>
        /// </returns>
        public DateTime DateTimeUtc {
            get {

                // Debug log messages.
                #if UNITY_EDITOR
                if (_showDebugWarnings) {
                    if (_networkTimeUtc == DateTime.MinValue)
                        Debug.LogWarning("\"NetworkTimeManager.DateTimeUtc\" has not been set. "
                            + "The value is \"DateTime.MinValue\" when this is the case.");
                    if (!_timeInSync)
                        Debug.LogWarning("\"NetworkTimeManager.DateTimeUtc\" is not in sync. "
                            + "Check \"NetworkTimeManager.IsTimeInSync\" before getting the getting the network time.");
                }
                #endif

                return _networkTimeUtc;
            }
        }

        /// <summary>
        /// Is the network time in sync with the device system time.
        ///     <list type="bullet">
        ///     <item>The time is considered in sync when it has a valid time from a NTP server, and the time has not
        ///     changed by more than the allowed off sync seconds between update frames.</item>
        ///     <item>The time will stay in sync when the network connection is lost as long as the system time has not
        ///     changed by more than the allowed off sync seconds, and the application has not been paused for more
        ///     than the allowed pause seconds.</item>
        ///     </list>
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///     <item><see langword="true"/> if the network time is correct.</item>
        ///     <item><see langword="false"/> if the network time is not correct.</item>
        ///     </list>
        /// </returns>
        public bool IsTimeInSync { get => _timeInSync; }

        /// <summary>
        /// Set network time as off sync and resync to a NTP server.
        /// </summary>
        public void ForceTimeResync()
        {
            _timeInSync = false;
            if (_currentlySyncingTime) return;
            StartCoroutine(SyncNetworkTimeCoroutine(WaitForNetworkMinSeconds));
        }

        public void Init()
        {
            // Destroy if singleton instance already exists.
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
                return;
            }

            // Set this instance as a singleton game object.
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Load the saved next request times for each NTP server.
            LoadRequestTimesForNtpServers();
        }

        private void Start()
        {
            // Get network time.
            StartCoroutine(SyncNetworkTimeCoroutine(WaitForNetworkMinSeconds));

            // Allow simulate applicaiton pausing when pause button is pressed inside Unity Editor.
            #if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
            #endif
        }

        private void Update()
        {
            // Wait for network to be in sync.
            if (_currentlySyncingTime) return;

            // Calculate the next network time from the system time.
            DateTime systemTimeUtc = DateTime.UtcNow;
            DateTime nextNetworkTimeUtc = systemTimeUtc.Add(_timeDifferenceUtc);

            // Time has changed too much and is now out of sync.
            if (Mathf.Abs((float)(_networkTimeUtc - nextNetworkTimeUtc).TotalSeconds) > AllowedOffSyncSeconds) {
                _timeInSync = false;
                StartCoroutine(SyncNetworkTimeCoroutine(WaitForNetworkMinSeconds));
                return;
            }

            // Set network time value.
            _networkTimeUtc = nextNetworkTimeUtc;
        }

        private void OnApplicationQuit()
        {
            // Save the next request times for each NTP server.
            SaveRequestTimesForNtpServers();
        }

        private void OnApplicationPause(bool isPaused)
        {
            // Do nothing if time not in sync or currently syncing.
            if (!_timeInSync || _currentlySyncingTime)
                return;

            // Remember system time when paused.
            if (isPaused)
                _appPauseTime = DateTime.UtcNow;

            // Set network time if app wasn't paused too long.
            else {
                DateTime systemTime = DateTime.UtcNow;
                double appPauseForSeconds = (systemTime - _appPauseTime).TotalSeconds;
                if (appPauseForSeconds < 0 || appPauseForSeconds > AllowedPauseSeconds)
                    _timeInSync = false;
                else _networkTimeUtc = systemTime.Add(_timeDifferenceUtc);
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Simulates the applicaiton pausing by calling <see cref="OnApplicationPause(bool)"/> when the pause button
        /// is pressed while playing inside Unity Editor.
        /// </summary>
        private void OnPauseStateChanged(PauseState pauseState)
        {
            _applicationPaused = pauseState == PauseState.Paused;
            OnApplicationPause(_applicationPaused);
        }
        #endif

        /// <summary>
        /// Find an available NTP server to sync network time with current system time.
        ///     <list type="bullet">
        ///     <item>Will wait <paramref name="waitSecondsBetweenFails"/> seconds if all NTP requests fail, and the
        ///     wait time will be increased by 1 second for each consecutive fail until <see cref="WaitForNetworkMaxSeconds"/>.</item>
        ///     </list>
        /// </summary>
        /// <param name="waitSecondsBetweenFails">The seconds to wait if all NTP requests fail before trying to sync again.</param>
        private IEnumerator SyncNetworkTimeCoroutine(float waitSecondsBetweenFails)
        {
            // Debug log messages.
            #if UNITY_EDITOR
            if (_showDebugMessages)
                Debug.Log("NetworkTimeManager: Getting time from a NTP server.");
            if (_showDebugWarnings && waitSecondsBetweenFails < 1f)
                Debug.LogWarning("NetworkTimeManager: The starting seconds to wait between sync attempts is less than 1.");
            #endif

            // Get time from a NTP server and sync with system time.
            _currentlySyncingTime = true;
            yield return null;
            DateTime systemTimeUtc;
            while (true) {
                yield return new WaitForSecondsRealtime(waitSecondsBetweenFails);
                for (int i = 0; i < _ntpServers.Length; i++) {

                    // Lost network reachability.
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                        break;

                    // Skip current NTP request.
                    systemTimeUtc = DateTime.UtcNow;
                    if (systemTimeUtc < _ntpServers[i].NextRequestTime || _ntpServers[i].FailCount > NtpRequestMaxFails)
                        continue;

                    // Set next request time.
                    _ntpServers[i].NextRequestTime = systemTimeUtc.AddSeconds(NtpServerCooldownSeconds);

                    // Get the network UTC time.
                    Task<DateTime> GetTimeFromNtpServerTask = GetTimeFromNtpServerAsync(_ntpServers[i].DomainNameAddress);
                    while (!GetTimeFromNtpServerTask.IsCompleted)
                        yield return null;
                    DateTime ntpTime = GetTimeFromNtpServerTask.Result;

                    // Failed to get time.
                    if (ntpTime == DateTime.MinValue) {
                        _ntpServers[i].FailCount++;

                        // Debug log messages.
                        #if UNITY_EDITOR
                        if (_showDebugMessages)
                            Debug.Log($"NetworkTimeManager: NTP request to \"{_ntpServers[i].DomainNameAddress}\" failed.");
                        #endif
                    }

                    // Successfully received time.
                    else {
                        systemTimeUtc = DateTime.UtcNow;
                        _timeDifferenceUtc = systemTimeUtc - ntpTime;
                        _ntpServers[i].FailCount = 0;

                        // Debug log messages.
                        #if UNITY_EDITOR
                        if (_showDebugMessages)
                            Debug.Log($"NetworkTimeManager: Received {ntpTime} from \"{_ntpServers[i].DomainNameAddress}\".");
                        #endif

                        // Set network time as in sync.
                        _networkTimeUtc = systemTimeUtc.Add(_timeDifferenceUtc);
                        _timeInSync = true;
                        _currentlySyncingTime = false;
                        yield break;
                    }
                }

                // Assume no internet, so prepare for next request attempt.
                for (int i = 0; i < _ntpServers.Length; i++)
                    _ntpServers[i].FailCount = 0;
                if (waitSecondsBetweenFails < WaitForNetworkMaxSeconds)
                    waitSecondsBetweenFails = Math.Min(waitSecondsBetweenFails + 1f, WaitForNetworkMaxSeconds);

                // Debug log messages.
                #if UNITY_EDITOR
                if (_showDebugWarnings)
                    Debug.LogWarning("NetworkTimeManager: Failed to update network time from any NTP servers. "
                        + $"Retry in {waitSecondsBetweenFails} {(waitSecondsBetweenFails == 1f ? "second" : "seconds")}.");
                #endif
            }
        }

        /// <summary>
        /// Request the time from a NTP server.
        /// </summary>
        /// <param name="ntpServerDnsAddress">The domain name address of the NTP server.</param>
        /// <returns>
        ///     <list type="bullet">
        ///     <item>If successful, the extracted <see cref="DateTime"/> from the received NTP message.</item>
        ///     <item>If there is an exception, <see cref="DateTime.MinValue"/>.</item>
        ///     </list>
        /// </returns>
        private async Task<DateTime> GetTimeFromNtpServerAsync(string ntpServerDnsAddress)
        {
            try {

                // Prepare NTP message data.
                byte[] ntpData = new byte[NtpMessageBytes];
                ntpData[0] = NtpRequestHeader;

                // Get NTP server IP addresses.
                IPAddress[] ntpServerIpAddresses;
                ntpServerIpAddresses = Dns.GetHostEntry(ntpServerDnsAddress).AddressList;

                // Validate there is an IP address to use.
                if (ntpServerIpAddresses.Length == 0)
                    throw new InvalidOperationException($"NetworkTimeManager: No IP address found for \"{ntpServerDnsAddress}\".");

                // Connect and send the NTP request to the first resolved IP address.
                using UdpClient thisDeviceUdpClient = new();
                thisDeviceUdpClient.Client.ReceiveTimeout = NetworkTimeoutMilliseconds;
                thisDeviceUdpClient.Connect(new IPEndPoint(ntpServerIpAddresses[0], NtpUdpPort));
                DateTime sendTime = DateTime.UtcNow;
                await thisDeviceUdpClient.SendAsync(ntpData, ntpData.Length);

                // Receive incoming data.
                UdpReceiveResult udpReceiveResult = await thisDeviceUdpClient.ReceiveAsync();
                DateTime receiveTime = DateTime.UtcNow;
                ntpData = udpReceiveResult.Buffer;
                IPEndPoint ntpServerEndPoint = udpReceiveResult.RemoteEndPoint;

                // Validate the response source IP address.
                if (!ntpServerIpAddresses.Any(ipAddress => ipAddress.Equals(ntpServerEndPoint.Address)))
                    throw new InvalidOperationException($"NetworkTimeManager: Received message IP address does not match \"{ntpServerDnsAddress}\".");

                // Validate message is correct size, type, and version for the expected NTP response.
                if (ntpData.Length != NtpMessageBytes
                    || (ntpData[0] & 0x07) != 4
                    || ((ntpData[0] >> 3) & 0x07) != 3 && ((ntpData[0] >> 3) & 0x07) != 4)
                    throw new InvalidOperationException("NetworkTimeManager: Received message is not a valid size, type, or version for the expected NTP data.");

                // Validate NTP server is synchronized by checking leap indicator.
                if (((ntpData[0] >> 6) & 0x03) == 3)
                    throw new InvalidOperationException($"NetworkTimeManager: NTP server \"{ntpServerDnsAddress}\" is not synchronized.");

                // Extract the time from the message.
                ulong ntpSeconds = BitConverter.ToUInt32(ntpData, NtpSecondsOffsetByte);
                ulong ntpFractionOfSecond = BitConverter.ToUInt32(ntpData, NtpFractionOfSecondOffsetByte);

                // Swap the endianness if current system does not match NTP big-endian format.
                if (BitConverter.IsLittleEndian) {
                    ntpSeconds = SwapEndianness(ntpSeconds);
                    ntpFractionOfSecond = SwapEndianness(ntpFractionOfSecond);
                }

                // Validate round-trip time.
                TimeSpan roundTripTime = receiveTime - sendTime;
                if (roundTripTime.TotalMilliseconds > NetworkTimeoutMilliseconds || roundTripTime.TotalMilliseconds < 0)
                    throw new InvalidOperationException("NetworkTimeManager: The system time changed too much while waiting on a NTP response.");

                // Calculate and return the NTP time.
                double ntpMilliseconds = (ntpSeconds * 1000) + (ntpFractionOfSecond * 1000 / 0x100000000L);
                DateTime ntpTime = _epochTime.AddMilliseconds(ntpMilliseconds);
                return ntpTime.AddMilliseconds(-(roundTripTime.TotalMilliseconds / 2));
            }

            // Log error.
            catch (Exception exception) {
                Debug.LogException(exception);
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Swap the endianness of <paramref name="dataToConvert"/>.
        /// </summary>
        private uint SwapEndianness(ulong dataToConvert)
        {
            return (uint)(((dataToConvert & 0x000000ff) << 24) + ((dataToConvert & 0x0000ff00) << 8)
                + ((dataToConvert & 0x00ff0000) >> 8) + ((dataToConvert & 0xff000000) >> 24));
        }

        /// <summary>
        /// Save the next request time for each NTP server.
        /// </summary>
        private void SaveRequestTimesForNtpServers()
        {
            // PlayerPref strings can be a max of 243 characters for Windows registry (255 total - 12 Unity suffix).
            int ntpServerCount = _ntpServers.Length;
            if (ntpServerCount > 12) {
                ntpServerCount = 12;

                // Debug log messages.
                #if UNITY_EDITOR
                if (_showDebugWarnings)
                    Debug.LogWarning($"NetworkTimeManager: Only 12 of the request times for {_ntpServers.Length} NTP servers were saved. "
                        + "PlayerPref strings can be a max of 243 characters for Windows registry.");
                #endif
            }

            // Add the request time for each NTP server to a string.
            StringBuilder requestTimesToSave = new();
            for (int i = 0; i < ntpServerCount; i++)
                requestTimesToSave.Append(_ntpServers[i].NextRequestTime.Ticks.ToString("D20"));

            // Save the next request times.
            PlayerPrefs.SetString(SaveNtpTimesName, requestTimesToSave.ToString());
            PlayerPrefs.Save();

            // Debug log messages.
            #if UNITY_EDITOR
            if (_showDebugMessages) {
                StringBuilder debugMessage = new();
                debugMessage.Append($"NetworkTimeManager: Saved request times for NTP servers: {requestTimesToSave}");
                for (int i = 0; i < ntpServerCount; i++)
                    debugMessage.Append($"\n{_ntpServers[i].DomainNameAddress} : {_ntpServers[i].NextRequestTime}");
                Debug.Log(debugMessage);
            }
            #endif
        }

        /// <summary>
        /// Load the next request time for each NTP server if the data exists.
        /// </summary>
        private void LoadRequestTimesForNtpServers()
        {
            // Read the request times.
            string savedRequestTimes = PlayerPrefs.GetString(SaveNtpTimesName, string.Empty);

            // No request times were found.
            if (savedRequestTimes == string.Empty)
                return;

            // Get number of NTP servers to load.
            int ntpServerCount = _ntpServers.Length;
            if (ntpServerCount > 12) ntpServerCount = 12;

            // Validate saved string length.
            if (savedRequestTimes.Length % 20 != 0) {
                string errorMessage = "NetworkTimeManager: Failed to load NTP server request times.";
                #if UNITY_EDITOR
                errorMessage += " The read string value is not the correct length.";
                #endif
                Debug.LogError(errorMessage);
                return;
            }

            // Parse the request times.
            long[] requestTimeTicks = new long[ntpServerCount];
            for (int i = 0; i < ntpServerCount; i++) {
                string parsedRequestTime = savedRequestTimes.Substring(i * 20, 20);
                requestTimeTicks[i] = long.Parse(parsedRequestTime);
            }

            // Assign the parsed request times to the NTP servers.
            for (int i = 0; i < ntpServerCount; i++) {
                DateTime nextRequestTime = new(requestTimeTicks[i]);

                // Validate the request time is not too far in the future.
                if ((nextRequestTime - DateTime.UtcNow).TotalSeconds > NtpServerCooldownSeconds) {
                    string errorMessage = "NetworkTimeManager: Failed to load a NTP server request time.";
                    #if UNITY_EDITOR
                    errorMessage += " The read time is greater than \"NtpServerCooldownSeconds\".";
                    #endif
                    Debug.LogError(errorMessage);
                    continue;
                }

                // Set the request time.
                _ntpServers[i].NextRequestTime = new DateTime(requestTimeTicks[i]);
            }

            // Debug log messages.
            #if UNITY_EDITOR
            if (_showDebugMessages) {
                StringBuilder debugMessage = new();
                debugMessage.Append($"NetworkTimeManager: Read request times for NTP servers: {savedRequestTimes}");
                for (int i = 0; i < ntpServerCount; i++)
                    debugMessage.Append($"\n{_ntpServers[i].DomainNameAddress} : {_ntpServers[i].NextRequestTime}");
                Debug.Log(debugMessage);
            }
            #endif
        }
    }
}