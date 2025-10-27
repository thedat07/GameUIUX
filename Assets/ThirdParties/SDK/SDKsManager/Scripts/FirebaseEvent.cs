using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseEvent
{
    public static void LogEvent(string eventType)
    {
        LogAnalytics(eventType);
    }

    public static void LogEvent(string eventType, string paramName, string paramValue)
    {
        if (GameManager.Instance.IsDoneFirebase() == false) return;
        LogAnalytics(eventType, paramName, paramValue.ToLower());
    }

    public static void LogEvent(string eventType, string paramName, string paramValue, string paramName2, string paramValue2)
    {
        var p = new Parameter[]
        {
            new Parameter(paramName, paramValue.ToLower()),
            new Parameter(paramName2, paramValue2.ToLower())
        };
        LogAnalytics(eventType, p);
    }

    public static void LogEvent(string eventType, string paramName, string paramValue, string paramName2, string paramValue2, string paramName3, string paramValue3)
    {
        var p = new Parameter[]
        {
            new Parameter(paramName, paramValue.ToLower()),
            new Parameter(paramName2, paramValue2.ToLower()),
            new Parameter(paramName3, paramValue3.ToLower())
        };
        LogAnalytics(eventType, p);

    }

    public static void LogEventReward(string paramValue2)
    {
        int level = GameManager.Instance.GetMasterData().GetData(MasterDataType.Stage);
        string eventType = "af_rewarded";
        string paramName = "level";
        string paramValue = level.ToString();
        string paramName2 = "reward_type";

        var p = new Parameter[]
        {
            new Parameter(paramName, paramValue.ToLower()),
            new Parameter(paramName2, paramValue2.ToLower()),
        };
        LogAnalytics(eventType, p);

    }

    public static void LogEvent(string eventType, string paramName, string paramValue, string paramName2, string paramValue2, string paramName3, string paramValue3, string paramName4, string paramValue4)
    {
        var p = new Parameter[]
        {
            new Parameter(paramName, paramValue.ToLower()),
            new Parameter(paramName2, paramValue2.ToLower()),
            new Parameter(paramName3, paramValue3.ToLower()),
            new Parameter(paramName4, paramValue4.ToLower())
        };

        LogAnalytics(eventType, p);
    }

    public static void LogAnalytics(string name, params Parameter[] parameters)
    {
        if (!IsFirebaseReady()) return;
        FirebaseAnalytics.LogEvent(name, parameters);
    }

    public static void LogAnalytics(string name, string parameterName, string parameterValue)
    {
        if (!IsFirebaseReady()) return;
        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
    }

    private static bool IsFirebaseReady()
    {
        if (GameManager.Instance.IsDoneFirebase() == false)
        {
            Debug.LogWarning("[FirebaseEvents] ❌ Firebase chưa sẵn sàng, bỏ qua LogEvent.");
            return false;
        }
        return true;
    }
}

public class FirebaseEventLogger
{
    public enum Category
    {
        Home,
        InGame
    }

    public static Category GetCategory()
    {
        return GameManager.Instance.GetMasterModelView().IsPlay ? Category.InGame : Category.Home;
    }

    private static bool IsFirebaseReady()
    {
        if (GameManager.Instance.IsDoneFirebase() == false)
        {
            Debug.LogWarning("[FirebaseEvents] ❌ Firebase chưa sẵn sàng, bỏ qua LogEvent.");
            return false;
        }
        return true;
    }

    public static void LogLevelStart(GamePlayInfo gamePlayInfo)
    {
        if (!IsFirebaseReady()) return;

        int levelId = gamePlayInfo.levelId;

        FirebaseAnalytics.LogEvent(
            "level_start",
            new Parameter("level_id", levelId)
        );

        Console.Log($"[FirebaseEvents] ✅ LogLevelStart | level_id={levelId}");
    }

    public static void LogFeatureUnLock(string name)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "feature_unlock",
            new Parameter("name", name)
        );

        Console.Log($"[FirebaseEvents] ✅ FeatureUnlock | name={name}");
    }

    public static void LogLevelEnd(int result, GamePlayInfo gamePlayInfo)
    {
        if (!IsFirebaseReady()) return;

        int levelId = gamePlayInfo.levelId;
        int duration = gamePlayInfo.GetDuration();
        int retry = gamePlayInfo.GetRetry(result == 4 ? true : false);
        string itemJson = gamePlayInfo.GetInfo();

        FirebaseAnalytics.LogEvent(
            "level_end",
            new Parameter("level_id", levelId),
            new Parameter("result", result),
            new Parameter("duration", duration),
            new Parameter("retry", retry),
            new Parameter("item", itemJson)
        );

        Console.Log($"[FirebaseEvents] ✅ LogLevelEnd | level_id={levelId}, result={result}, retry={retry}, duration={duration}, item={itemJson}");
    }

    public static void LogMax(string format)
    {
        if (!IsFirebaseReady()) return;

        int levelId = GameManager.Instance.GetMasterData().dataStage.Get();
        string eventName = string.Format(format, levelId);
        FirebaseAnalytics.LogEvent(eventName);
    }

    public static void LogPurchaseClick(string productId, int status)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "purchase_click",
            new Parameter("product_id", productId),
            new Parameter("source", GetCategory().ToString()),
            new Parameter("status", status)
        );

        Console.Log($"[FirebaseEvents] ✅ LogPurchaseClick | product_id={productId}, source={GetCategory().ToString()}, status={status}");
    }

    public static void LogButtonClick(string name, string desc)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "button_click",
            new Parameter("category", GetCategory().ToString()),
            new Parameter("name", name),
            new Parameter("desc", desc)
        );

        Console.Log($"[FirebaseEvents] ✅ LogButtonClick | category={GetCategory()}, name={name}, desc={desc}");
    }

    public static void LogEarnResource(
       string itemCategory,
       string itemId,
       string source,
       string sourceId,
       int value,
       double remainingValue,
       double totalEarnedValue)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "earn_resource",
            new Parameter("item_category", itemCategory),
            new Parameter("item_id", itemId),
            new Parameter("source", source),
            new Parameter("source_id", sourceId),
            new Parameter("value", value),
            new Parameter("remaining_value", remainingValue),
            new Parameter("total_earned_value", totalEarnedValue)
        );

        Console.Log($"[FirebaseEvents] ✅ LogEarnResource | item_category={itemCategory}, item_id={itemId}, source={source}, source_id={sourceId}, value={value}, remaining_value={remainingValue}, total_earned_value={totalEarnedValue}");
    }

    public static void LogBuyResource(
       string itemCategory,
       string itemId,
       string source,
       string sourceId,
       int value,
       double remainingValue,
       double totalBoughtValue)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "buy_resource",
            new Parameter("item_category", itemCategory),
            new Parameter("item_id", itemId),
            new Parameter("source", source),
            new Parameter("source_id", sourceId),
            new Parameter("value", value),
            new Parameter("remaining_value", remainingValue),
            new Parameter("total_bought_value", totalBoughtValue)
        );

        Console.Log($"[FirebaseEvents] ✅ LogBuyResource | item_category={itemCategory}, item_id={itemId}, source={source}, source_id={sourceId}, value={value}, remaining_value={remainingValue}, total_bought_value={totalBoughtValue}");
    }

    public static void LogSpendResource(
        string itemCategory,
        string itemId,
        string itemName,
        string source,
        string sourceId,
        int value,
        double remainingValue,
        double totalSpentValue)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "spend_resource",
            new Parameter("item_category", itemCategory),
            new Parameter("item_id", itemId),
            new Parameter("item_name", itemName),
            new Parameter("source", source),
            new Parameter("source_id", sourceId),
            new Parameter("value", value),
            new Parameter("remaining_value", remainingValue),
            new Parameter("total_spent_value", totalSpentValue)
        );

        Console.Log($"[FirebaseEvents] ✅ LogSpendResource | item_category={itemCategory}, item_id={itemId}, item_name={itemName}, source={source}, source_id={sourceId}, value={value}, remaining_value={remainingValue}, total_spent_value={totalSpentValue}");
    }
}

public static class FirebaseUserProperties
{
    private static bool IsFirebaseReady()
    {
        if (GameManager.Instance.IsDoneFirebase() == false)
        {
            Console.LogWarning("[FirebaseUserProperties] ❌ Firebase chưa sẵn sàng, bỏ qua SetUserProperty.");
            return false;
        }
        return true;
    }

    public static void SetUserId(string userId)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserId(userId);
        Console.Log($"[FirebaseUserProperties] ✅ SetUserId: {userId}");
    }

    /// <summary>
    /// Đặt 1 user property bất kỳ lên Firebase (có kiểm tra trạng thái).
    /// </summary>
    public static void Set(string key, string value)
    {
        if (!IsFirebaseReady())
            return;

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            return;

        FirebaseAnalytics.SetUserProperty(key, value);
#if UNITY_EDITOR
        Debug.Log($"[FirebaseUserProperties] ✅ SetUserProperty: {key} = {value}");
#endif
    }

    /// <summary>
    /// Gán các thông tin UA (từ Adjust / AppsFlyer).
    /// </summary>
    public static void SetUA(string attributionId, string network, string campaign, string adgroup = null, string creative = null)
    {
        Set("attribution_id", attributionId);
        Set("ua_network", network);
        Set("ua_campaign", campaign);
        Set("ua_adgroup", adgroup);
        Set("ua_creative", creative);
    }

    /// <summary>
    /// Gán ID riêng của Adjust hoặc AppsFlyer.
    /// </summary>
    public static void SetAttributionId(string adid)
    {
        Set("attribution_id", adid);
    }

    /// <summary>
    /// Gán biến A/B testing variant.
    /// </summary>
    public static void SetABVariant(string variant)
    {
        Set("ab_variant", variant);
    }

    /// <summary>
    /// Gán phiên bản ứng dụng (App Version).
    /// </summary>
    public static void SetAppVersion(string version)
    {
        Set("app_version", version);
    }

    public static void SetAdId(string adId)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("ad_id", adId);
        Console.Log($"[FirebaseUserProperties] ✅ SetAdId: {adId}");
    }

    public static void SetCreatedTimestamp(string timestamp)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("created_timestamp", timestamp);
        Console.Log($"[FirebaseUserProperties] ✅ SetCreatedTimestamp: {timestamp}");
    }

    public static void SetOnlineTime(long seconds)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("online_time", seconds.ToString());
        Console.Log($"[FirebaseUserProperties] ✅ SetOnlineTime: {seconds}");
    }

    public static void SetMaxLevel(int level)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("max_level", level.ToString());
        Console.Log($"[FirebaseUserProperties] ✅ SetMaxLevel: {level}");
    }

    public static void SetIapCount(int count)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("iap_count", count.ToString());
        Console.Log($"[FirebaseUserProperties] ✅ SetIapCount: {count}");
    }

    public static void SetIaaCount(int count)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("iaa_count", count.ToString());
        Console.Log($"[FirebaseUserProperties] ✅ SetIaaCount: {count}");
    }

    public static void SetRemainingGold(int gold)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("remaining_gold", gold.ToString());
        Console.Log($"[FirebaseUserProperties] ✅ SetRemainingGold: {gold}");
    }

    public static void SetTotalGoldEarn(int gold)
    {
        if (!IsFirebaseReady()) return;

        FirebaseAnalytics.SetUserProperty("total_gold_earn", gold.ToString());
        Console.Log($"[FirebaseUserProperties] ✅ SetTotalGoldEarn: {gold}");
    }
}