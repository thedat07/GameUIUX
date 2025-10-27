using UnityEngine;
using AdjustSdk;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using Firebase.Analytics;

public class AdjustController : MonoBehaviour
{
    [SerializeField] private string appToken;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
#if UNITY_EDITOR
        AdjustConfig adjustConfig = new AdjustConfig(appToken, AdjustEnvironment.Sandbox);
#else
        AdjustConfig adjustConfig = new AdjustConfig(appToken, AdjustEnvironment.Production);
#endif

        // Thiết lập mức log
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        adjustConfig.LogLevel = AdjustLogLevel.Verbose;
#else
        adjustConfig.LogLevel = AdjustLogLevel.Info;
#endif

        // Cho phép gửi event khi app chạy nền
        adjustConfig.IsSendingInBackgroundEnabled = true;

        // Callback khi có thông tin attribution
        adjustConfig.AttributionChangedDelegate = OnAttributionChanged;

        // Callback khi có deferred deeplink
        adjustConfig.DeferredDeeplinkDelegate = OnDeferredDeeplink;

        // Khởi tạo Adjust SDK
        Adjust.InitSdk(adjustConfig);

        Console.Log("[Adjust] SDK initialized");

        AdjustEventMap.LogFirstOpen();
    }

    private void OnAttributionChanged(AdjustAttribution attribution)
    {
        if (attribution == null)
        {
            Console.LogWarning("[Adjust] Attribution is null");
            return;
        }

        // Lấy AdID (unique user từ Adjust)
        Adjust.GetAdid(adid =>
        {
            FirebaseUserProperties.SetUA(
                attributionId: adid,
                network: attribution.Network,
                campaign: attribution.Campaign,
                adgroup: attribution.Adgroup,
                creative: attribution.Creative
            );
        });

        // Gán các trường UA vào Firebase user properties
        SetFirebaseUserProperty("ua_network", attribution.Network);
        SetFirebaseUserProperty("ua_campaign", attribution.Campaign);
        SetFirebaseUserProperty("ua_adgroup", attribution.Adgroup);
        SetFirebaseUserProperty("ua_creative", attribution.Creative);

        // Log sự kiện first_open để tracking khởi đầu UA
        FirebaseAnalytics.LogEvent("adjust_first_open");

        Console.Log($"[Adjust] Attribution received → Network: {attribution.Network}, Campaign: {attribution.Campaign}");
    }

    private void SetFirebaseUserProperty(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            FirebaseAnalytics.SetUserProperty(key, value);
        }
    }

    private void OnDeferredDeeplink(string deeplinkURL)
    {
        if (!string.IsNullOrEmpty(deeplinkURL))
        {
            Console.Log("[Adjust] Deferred Deeplink: " + deeplinkURL);
            FirebaseAnalytics.LogEvent("adjust_deferred_deeplink", new Parameter("url", deeplinkURL));
        }
    }
}

public static class AnalyticsTracker
{
    private const string EVENT_IAP = "af_iap"; // hoặc token bạn setup trên Adjust dashboard

    public static void TrackIAPWithVerify(Product product)
    {
        if (product == null) return;

        AdjustEventMap.LogPurchase(product.definition.id, (double)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);

        double localizedPrice = (double)product.metadata.localizedPrice * StaticData.RateRev;
        string currency = product.metadata.isoCurrencyCode;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AdjustEvent adjustEvent = new AdjustEvent(EVENT_IAP);
            adjustEvent.SetRevenue(localizedPrice, currency);
            adjustEvent.TransactionId = product.transactionID;
            adjustEvent.ProductId = product.definition.id;

            Adjust.VerifyAndTrackAppStorePurchase(adjustEvent, verificationResult =>
            {
                Console.Log("Adjus", "iOS Verify status: " + verificationResult.VerificationStatus);
                Console.Log("Adjus", "Code: " + verificationResult.Code);
                Console.Log("Adjus", "Message: " + verificationResult.Message);
            });
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string purchaseToken = ExtractPurchaseToken(product);

            AdjustEvent adjustEvent = new AdjustEvent(EVENT_IAP);
            adjustEvent.SetRevenue(localizedPrice, currency);
            adjustEvent.ProductId = product.definition.id;

            if (!string.IsNullOrEmpty(purchaseToken))
                adjustEvent.PurchaseToken = purchaseToken;

            Adjust.VerifyAndTrackPlayStorePurchase(adjustEvent, verificationResult =>
            {
                Console.Log("Adjus", "Android Verify status: " + verificationResult.VerificationStatus);
                Console.Log("Adjus", "Code: " + verificationResult.Code);
                Console.Log("Adjus", "Message: " + verificationResult.Message);
            });
        }
        else
        {
            Console.LogWarning("Adjus", "Platform không hỗ trợ IAP verify");
        }
    }

    private static string ExtractPurchaseToken(Product product)
    {
        try
        {
            var receiptWrapper = MiniJson.JsonDecode(product.receipt) as Dictionary<string, object>;
            if (receiptWrapper != null && receiptWrapper.ContainsKey("Payload"))
            {
                var payload = MiniJson.JsonDecode(receiptWrapper["Payload"].ToString()) as Dictionary<string, object>;
                if (payload != null && payload.ContainsKey("json"))
                {
                    var originalJson = MiniJson.JsonDecode(payload["json"].ToString()) as Dictionary<string, object>;
                    if (originalJson != null && originalJson.ContainsKey("purchaseToken"))
                    {
                        return originalJson["purchaseToken"].ToString();
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("ExtractPurchaseToken error: " + e.Message);
        }
        return null;
    }
}
