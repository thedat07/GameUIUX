# 📦 Unity Packages Used

Danh sách các package cài đặt qua Git URL hoặc Unity Asset Store được sử dụng trong dự án này.

---

### ✅ [Simple Screen Manager (SSM)](https://github.com/AnhPham/Simple-Screen-Manager-for-Unity-aka-SS.git)
- **Description**: Reactive Extensions for Unity
- **Install (UPM Git)**:
  ```json
  "com.anhpham.simplescreenmanager": "https://github.com/AnhPham/Simple-Screen-Manager-for-Unity-aka-SS.git"

---

### ✅ [UniRx](https://github.com/neuecc/UniRx)
- **Description**: Reactive Extensions for Unity
- **Install (UPM Git)**:
  ```json
  "com.github.neuecc.uniRx": "https://github.com/neuecc/UniRx.git"

---

### ✅ [SoftMaskForUGUI](https://github.com/mob-sakai/SoftMaskForUGUI)
- **Description**: Reactive Extensions for Unity
- **Install (UPM Git)**:
  ```json
  "com.mobsakai.softmask": "https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src#3.2.0"

---

### ✅ [Animation Sequencer](https://github.com/brunomikoski/Animation-Sequencer.git)
- **Description**: Reactive Extensions for Unity
- **Install (UPM Git)**:
  ```json
  "com.brunomikoski.animationsequencer": "https://github.com/brunomikoski/Animation-Sequencer.git"

---

## ✅ Mobile Ads v2.0 (Integration Layer)

- **Package**: [Mobile Ads v2.0 - Unity Asset Store](https://assetstore.unity.com/packages/tools/integration/mobile-ads-v2-0-266331)
- **Purpose**:
  - Acts as a wrapper layer for multiple ad networks (Applovin, Unity Ads, AdMob...)
- **Features**:
  - Unified AdsManager
  - Pre-built reward/interstitial/banner logic
- **Code References**:
  - `AdsModelView.cs`
- **Notes**:
  - Makes integration modular and swappable
  - Includes Ad Revenue forwarding and analytics integration hooks

---

## ✅ Easy IAP v2.0 (In-App Purchase System)

- **Package**: [Easy IAP v2.0 on Unity Asset Store](https://assetstore.unity.com/packages/tools/integration/easy-iap-in-app-purchase-v2-0-264594)
- **Purpose**: Simplify and unify IAP across platforms (Google Play, iOS)
- **Features**:
  - Simple purchase API
  - Cross-platform support
  - Receipt validation support
- **Code References**:
  - `ShopModelView.cs`
- **Notes**:
  - Designed to be plug-and-play
  - Can extend with custom listeners for analytics or special logic

---

## ✅ DOTween (Tweening Library)

- **Version**: `1.2.765` (hoặc mới nhất)
- **Asset Store**: [DOTween on Unity Asset Store](https://assetstore.unity.com/publishers/19336)
- **Features**:
  - Tweening animations for transforms, UI, sequences
- **Code References**:
  - `DOFade`, `DOMove`, `DOScale`, etc.
- **Notes**:
  - Setup required via `Tools → Demigiant → DOTween Utility Panel`
  - Highly optimized and used for animation sequences in UI and gameplay