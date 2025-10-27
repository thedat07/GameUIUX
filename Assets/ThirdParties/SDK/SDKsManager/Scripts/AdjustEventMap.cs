using System.Collections.Generic;
using UnityEngine;
using AdjustSdk;
using GoogleMobileAds.Api;

public static class AdjustEventMap
{
    private static readonly Dictionary<string, string> EventTokenMap = new Dictionary<string, string>
    {
        { "aj_first_open", "wrfb06" },
        { "aj_ad_complete", "lddxa9" },
        { "aj_purchase", "jgw2bq" },

        { "aj_complete_1", "rc4qi9" },
        { "aj_complete_5", "wnyrxy" },
        { "aj_complete_10", "twxioa" },
        { "aj_complete_20", "5a5db5" },
        { "aj_complete_30", "yojl8" },
        { "aj_complete_40", "h9xy9k" },
        { "aj_complete_50", "ss5tav" },
        { "aj_complete_60", "hefd2j" },
        { "aj_complete_70", "8cadra" },
        { "aj_complete_80", "euuh2h" },
        { "aj_complete_90", "uk6mf7" },
        { "aj_complete_100", "evkhm3" },
        { "aj_complete_110", "5yxuf7" },
        { "aj_complete_120", "36sv5t" },
    };

    public static string GetToken(string eventName)
    {
        if (EventTokenMap.TryGetValue(eventName, out var token))
        {
            return token;
        }

        Debug.LogWarning($"[Adjust] Event token not found for: {eventName}");
        return null;
    }

    private static void TrackEvent(string eventName, Dictionary<string, string> parameters = null, double? revenue = null, string currency = "USD")
    {
        string token = AdjustEventMap.GetToken(eventName);
        if (string.IsNullOrEmpty(token)) return;

        AdjustEvent adjustEvent = new AdjustEvent(token);

        // Add callback params
        if (parameters != null)
        {
            foreach (var kv in parameters)
                adjustEvent.AddCallbackParameter(kv.Key, kv.Value);
        }

        // Revenue nếu có
        if (revenue.HasValue)
            adjustEvent.SetRevenue(revenue.Value, currency);

        Adjust.TrackEvent(adjustEvent);
        Debug.Log($"[Adjust] TrackEvent: {eventName} ({token})");
    }


    public static void LogFirstOpen()
    {
        TrackEvent("aj_first_open", new Dictionary<string, string>
        {
            { "player_id", StaticData.GetPlayerId() }
        });
    }

    // User xem xong quảng cáo
    static void LogAdComplete(string adType, string adNetwork, string adLocation, double revenue = 0)
    {
        var param = new Dictionary<string, string>
        {
            { "player_id", StaticData.GetPlayerId() },
            { "ad_type", adType },
            { "ad_network", adNetwork },
            { "ad_location", adLocation }
        };

        TrackEvent("aj_ad_complete", param, revenue > 0 ? revenue : (double?)null);
    }

    public static void LogAdComplete(AdValue impressionData, string adType)
    {
        string adNetwork = "AdMob";
        string adLocation = impressionData.CurrencyCode;
        double revenue = impressionData.Value / 1_000_000.0;

        LogAdComplete(adType, adNetwork, adLocation, revenue);
    }

    // User mua IAP
    public static void LogPurchase(string productId, double revenue, string currency = "USD")
    {
        var param = new Dictionary<string, string>
        {
            { "player_id", StaticData.GetPlayerId() },
            { "product_id", productId }
        };

        TrackEvent("aj_purchase", param, revenue, currency);
    }

    // Hoàn thành level
    public static void LogCompleteLevel(int level)
    {
        // Chỉ check milestone level
        int[] milestoneLevels = { 1, 5, 10, 20, 30, 40, 50, 60, 70, 90, 100, 110, 120 };

        if (System.Array.IndexOf(milestoneLevels, level) < 0)
        {
            Debug.Log($"[Adjust] Skip LogCompleteLevel - Level {level} không nằm trong milestone.");
            return;
        }

        string eventName = $"aj_complete_{level}";
        var param = new Dictionary<string, string>
        {
            { "player_id", StaticData.GetPlayerId()  }
        };

        TrackEvent(eventName, param);
    }
}
