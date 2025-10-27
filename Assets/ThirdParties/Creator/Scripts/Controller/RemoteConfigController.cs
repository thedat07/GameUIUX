using Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class RemoteConfigController
{
    private const string CACHE_FILE = "RemoteConfigCache";

    public static bool IsFetchDataNow
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public static bool IsInitSuccess
    {
        get
        {
            return _isInitSuccess;
        }
    }

    private static bool _isInitSuccess = false;

    public static bool IsRemoteConfigLoaded { get; private set; } = false;

    private static Dictionary<string, string> _cachedConfig = new Dictionary<string, string>();

    public static void FetchData()
    {
        IsRemoteConfigLoaded = false;
        _isInitSuccess = true;

        LoadCache();

        if (IsFetchDataNow)
            FetchDataNow();
        else
            FetchDataReal();
    }

    private static void LoadCache()
    {
        if (!ES3.FileExists(CACHE_FILE)) return;

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(
            _cachedConfig.ToDictionary(kv => kv.Key, kv => (object)kv.Value));

        IsRemoteConfigLoaded = true;
    }

    public static void LoadCacheConfig()
    {
        if (!ES3.FileExists(CACHE_FILE)) return;
        _cachedConfig = ES3.Load<Dictionary<string, string>>(CACHE_FILE, CACHE_FILE, new Dictionary<string, string>());
    }

    private static string GetValueFromCacheOrFirebase(string key)
    {
        if (_cachedConfig != null && _cachedConfig.TryGetValue(key, out var cachedValue))
        {
            return cachedValue;
        }

        if (IsInitSuccess && FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key))
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        }

        return null;
    }

    public static string GetStringConfig(string key, string defaultValue)
    {
        return GetValueFromCacheOrFirebase(key) ?? defaultValue;
    }

    public static bool GetBoolConfig(string key, bool defaultValue)
    {
        var val = GetValueFromCacheOrFirebase(key);
        if (bool.TryParse(val, out var result)) return result;
        return defaultValue;
    }

    public static long GetLongConfig(string key, long defaultValue)
    {
        var val = GetValueFromCacheOrFirebase(key);
        if (long.TryParse(val, out var result)) return result;
        return defaultValue;
    }

    public static double GetDoubleConfig(string key, double defaultValue)
    {
        var val = GetValueFromCacheOrFirebase(key);
        if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)) return result;
        return defaultValue;
    }

    public static float GetFloatConfig(string key, float defaultValue)
    {
        var val = GetValueFromCacheOrFirebase(key);
        if (float.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)) return result;
        return defaultValue;
    }

    public static int GetIntConfig(string key, int defaultValue)
    {
        var val = GetValueFromCacheOrFirebase(key);
        if (int.TryParse(val, out var result)) return result;
        return defaultValue;
    }

    public static bool GetJsonConfig<T>(string key, out T result)
    {
        var val = GetValueFromCacheOrFirebase(key);
        if (string.IsNullOrEmpty(val))
        {
            result = default;
            return false;
        }

        try
        {
            result = JsonUtility.FromJson<T>(val);
            return true;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"RemoteConfig.GetJsonConfig {typeof(T)} key {key}, exception: {ex.Message}");
            result = default;
            return false;
        }
    }

    private static void FetchDataNow()
    {
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                       TimeSpan.FromSeconds(0));
        fetchTask.ContinueWith(FetchComplete);
    }

    private static void FetchDataReal()
    {
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                        TimeSpan.FromHours(6));
        fetchTask.ContinueWith(FetchComplete);

    }

    private static void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            DebugLog("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            DebugLog("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            DebugLog("Fetch completed successfully!");
        }

        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWith(_ =>
                {
                    IsRemoteConfigLoaded = true;
                    SaveCache();
                    DebugLog(string.Format("Remote data loaded and ready (last fetch time) {0}", info.FetchTime));
                });
                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
                        DebugLog("Fetch failed for unknown reason");
                        break;

                    case FetchFailureReason.Throttled:
                        DebugLog("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;

            case LastFetchStatus.Pending:
                DebugLog("Latest Fetch call still pending.");
                break;
        }
    }

    private static void DebugLog(string log)
    {
        UnityEngine.Console.Log("RemoteConfig", log);
    }

    private static void SaveCache()
    {
        _cachedConfig = FirebaseRemoteConfig.DefaultInstance.Keys.ToDictionary(key => key, key => FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue);

        ES3.Save(CACHE_FILE, _cachedConfig, CACHE_FILE);
        
        StaticDataFeature.Refresh();
        GameManager.Instance.GetFeatureData().Refresh();
    }
}
