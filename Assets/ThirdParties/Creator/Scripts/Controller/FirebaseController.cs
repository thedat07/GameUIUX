using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;

enum VerifyFirebase
{
    Verifying,
    Done,
    Error
}

public class FirebaseController : MonoBehaviour, IInitializable
{
    public bool active;

    public static string FirebaseID;

    private bool _firebaseInitialized = false;

    private VerifyFirebase firebaseReady = VerifyFirebase.Verifying;

    public bool IsDone() => active ? firebaseReady == VerifyFirebase.Done : false;

    public void Initialize()
    {
        if (active)
        {
            try
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    DependencyStatus dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        UnityEngine.Console.LogSuccess("Firebase", "Firebase is ready for use.");
                        firebaseReady = VerifyFirebase.Done;
                        FirebaseUserProperties.SetCreatedTimestamp(NetworkTime.UTC.ToString());
                        FirebaseUserProperties.SetUserId(StaticData.GetPlayerId());
                    }
                    else
                    {
                        UnityEngine.Console.LogError("Firebase", "Firebase is not ready for use.");
                        firebaseReady = VerifyFirebase.Error;
                        UnityEngine.Console.LogError("Firebase", "firebase Ready  Error");
                    }
                });
            }
            catch (Exception e)
            {
                firebaseReady = VerifyFirebase.Error;
                UnityEngine.Console.LogError("Firebase", "firebase Ready Error: " + e.ToString());
            }

            StartCoroutine(InitFirebase());
        }
    }

    void InitializeFirebase()
    {
        try
        {
            _firebaseInitialized = true;
            RemoteConfigController.FetchData();
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        }
        catch (Exception e)
        {
            UnityEngine.Console.LogError("Firebase", "Init Firebase Error: " + e.ToString());
            _firebaseInitialized = false;
        }
    }

    IEnumerator InitFirebase()
    {
        int _num = 0;
        while (firebaseReady == VerifyFirebase.Verifying)
        {
            _num++;
            if (_num > 5)
            {
                UnityEngine.Console.LogError("Firebase", "Init Firebase Error");
                break;
            }
            yield return new WaitForSeconds(1);
        }
        if (firebaseReady == VerifyFirebase.Done)
        {
            InitializeFirebase();
            yield return new WaitForSeconds(1);
            if (_firebaseInitialized)
            {
                try
                {
                    GetFirebaseID();
                }
                catch (Exception ex)
                {
                    UnityEngine.Console.LogError("Firebase", "GetFirebaseID Error: " + ex.ToString());
                }
            }
        }

    }
    public static Task<string> GetAnalyticsInstanceId()
    {
        return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                UnityEngine.Console.Log("Firebase", "App instance ID fetch was canceled.");
            }
            else if (task.IsFaulted)
            {
                UnityEngine.Console.Log("Firebase", String.Format("Encounted an error fetching app instance ID {0}",
                        task.Exception.ToString()));
            }
            else if (task.IsCompleted)
            {
                UnityEngine.Console.Log("Firebase", String.Format("App instance ID: {0}", task.Result));
            }
            return task;
        }).Unwrap();
    }

    private async void GetFirebaseID()
    {
        try
        {
            FirebaseID = await GetAnalyticsInstanceId();
        }
        catch (Exception ex)
        {
            UnityEngine.Console.LogError("Firebase", "GetFirebaseID Error: " + ex.ToString());
        }
    }
}