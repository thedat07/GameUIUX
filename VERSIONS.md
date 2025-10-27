# 📦 SDK Versions & Integration Notes

This file tracks the versions and integration details of key SDKs used in the GameTemp Unity project.

---

## ✅ AppLovin MAX SDK

- **Version**: `8.2.0`
- **Integration Tool**: AppLovin Integration Manager
- **Features**:
  - Interstitial Ads
  - Rewarded Video Ads
  - Banner Ads
  - Ad Revenue Tracking (`OnAdRevenuePaidEvent`)
- **Code References**:
  - `AdsModelView.cs`
  - `AdsModel.cs`
- **Notes**:
  - Auto-load on close is supported
  - `MaxSdk.SetUserId` used for user tracking
---

## ✅ Firebase SDK

- **Version**:
  - Firebase Core: `12.5.0`
  - Firebase Analytics: `12.5.0`
- **Integration**: `.unitypackage`
- **Features**:
  - Analytics tracking (`LogEvent`)
- **Code References**:
  - `FirebaseEvent.cs`, `FirebaseController.cs`
- **Notes**:
  - Uses wrapper class for easier extension
  - Consider enabling Crashlytics if needed

---

## ✅ Appsflyer SDK

- **Version**: `6.15.2`
- **Integration**: Manual import
- **Features**:
  - Attribution Tracking
  - Conversion Data Handling
- **Code References**:
  - `AppsFlyerObjectScript.cs`
- **Notes**:
  - `devKey` and `appId` are configured at runtime
  - Handles `OnConversionDataSuccess` and `OnAppOpenAttribution`

---

## ✅ Facebook SDK

- **Version**: `17.0.0`
- **Integration**: Unity `.unitypackage`
- **Features**:
  - App Activation Tracking
  - Analytics Events (`LogAppEvent`)
- **Code References**:
  - `FacebookController.cs`
- **Notes**:
  - Facebook SDK is initialized and activated on app start
  - Facebook Login is not currently used, but can be added

---

## 🛠 Unity & Build Info

- **Unity Version**: `2022.3.x LTS`
- **Supported Platforms**:
  - Android (Gradle)
  - iOS (Xcode)

---

## 📌 Update Tips

- AppLovin MAX SDK: check [latest releases](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration#step-1)
- Firebase: use Unity SDK updater or download `.unitypackage`
- Appsflyer: check [AppsFlyer Unity GitHub](https://github.com/AppsFlyerSDK/AppsFlyerUnityPlugin)
- Facebook: use Unity Package Manager or official `.unitypackage`
- DOTween: update via Asset Store or GitHub (Demigiant)
- UniRx: recommend GitHub latest commit if not from UPM

