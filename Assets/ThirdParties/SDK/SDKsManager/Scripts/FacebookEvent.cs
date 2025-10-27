using Facebook.Unity;
using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using System;

public class FacebookEvent
{
    public static void LogEvent(string eventName, string paramName, string paramValue, string paramName2, string paramValue2)
    {
        if (GameManager.Instance.IsDoneFacebook())
        {
            try
            {
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add(paramName, paramValue);
                p.Add(paramName2, paramValue2);
                FB.LogAppEvent(
                    eventName,
                    parameters: p
                );
            }
            catch (Exception e)
            {
                UnityEngine.Console.LogException(e);
            }
        }
    }

    public static void LogEvent(string eventName, string paramName, string paramValue)
    {
        if (GameManager.Instance.IsDoneFacebook())
        {
            try
            {
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add(paramName, paramValue);
                FB.LogAppEvent(
                    eventName,
                    parameters: p
                );
            }
            catch (Exception e)
            {
                UnityEngine.Console.LogException(e);
            }
        }
    }

    public static void LogEvent(string eventName)
    {
        if (GameManager.Instance.IsDoneFacebook())
        {
            try
            {
                FB.LogAppEvent(
                    eventName
                );
            }
            catch (Exception e)
            {
                UnityEngine.Console.LogException(e);
            }
        }

    }
}