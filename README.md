# 廣告中介實驗室 (AdMediationLab)

這是一個完整的 Unity 廣告中介和遊戲服務解決方案，重寫了 Plankton package 的功能並補足了所有付費限制。

## 🚀 功能特色

### 📱 廣告中介
- **多平台支援**: AdMob, AppLovin MAX, IronSource LevelPlay, Unity Ads
- **廣告類型**: 橫幅、插頁式、獎勵式、獎勵式插頁、應用開啟
- **統一介面**: 簡單易用的 API，支援所有廣告網絡
- **收益追蹤**: 完整的廣告收益和展示數據

### 💰 內購系統
- **多平台支援**: Google Play, App Store, Myket, Bazaar
- **商品類型**: 消耗品、非消耗品、訂閱
- **完整流程**: 載入商品、購買、確認、消耗、恢復購買
- **安全驗證**: 支援 RSA 金鑰驗證和收據驗證

### 📊 分析系統
- **多平台支援**: Firebase, GameAnalytics, AppsFlyer, Unity Analytics, Facebook
- **事件追蹤**: 自定義事件、用戶屬性、收益追蹤
- **遊戲分析**: 關卡進度、用戶行為、廣告效果
- **統一介面**: 一次記錄，多平台同步

## 📁 專案結構

```
Assets/Scripts/
├── AdMediationManager.cs      # 廣告中介管理器
├── AdProviders.cs             # 廣告提供者實作
├── BillingManager.cs          # 內購管理器
├── AnalyticsManager.cs        # 分析管理器
└── AdMediationLabController.cs # 測試控制器
```

## 🛠️ 快速開始

### 1. 初始化系統

```csharp
// 初始化廣告系統
AdMediationManager.Instance.Initialize((success) => {
    Debug.Log($"廣告系統初始化: {success}");
});

// 初始化內購系統
BillingManager.Instance.Initialize((success) => {
    Debug.Log($"內購系統初始化: {success}");
});

// 初始化分析系統
AnalyticsManager.Instance.Initialize((success) => {
    Debug.Log($"分析系統初始化: {success}");
});
```

### 2. 使用廣告功能

```csharp
// 載入橫幅廣告
AdMediationManager.Instance.LoadAd(AdType.Banner);

// 顯示橫幅廣告
AdMediationManager.Instance.ShowBanner(BannerPosition.Bottom);

// 載入並顯示插頁式廣告
AdMediationManager.Instance.LoadAd(AdType.Interstitial);
AdMediationManager.Instance.ShowAd(AdType.Interstitial, (success) => {
    Debug.Log($"插頁式廣告顯示: {success}");
});

// 載入並顯示獎勵式廣告
AdMediationManager.Instance.LoadAd(AdType.Rewarded);
AdMediationManager.Instance.ShowAd(AdType.Rewarded, (success) => {
    Debug.Log($"獎勵式廣告顯示: {success}");
});
```

### 3. 使用內購功能

```csharp
// 載入商品
string[] productIds = { "com.example.coins_100", "com.example.remove_ads" };
BillingManager.Instance.LoadProducts(productIds, (success) => {
    Debug.Log($"商品載入: {success}");
});

// 購買商品
BillingManager.Instance.PurchaseProduct("com.example.coins_100", (result) => {
    if (result.success) {
        Debug.Log($"購買成功: {result.productId}");
    } else {
        Debug.Log($"購買失敗: {result.errorMessage}");
    }
});

// 恢復購買 (iOS)
BillingManager.Instance.RestorePurchases((results) => {
    Debug.Log($"恢復購買: {results.Count} 個商品");
});
```

### 4. 使用分析功能

```csharp
// 記錄自定義事件
var parameters = new Dictionary<string, object> {
    { "level", 5 },
    { "score", 1000 },
    { "difficulty", "hard" }
};
AnalyticsManager.Instance.LogEvent("level_complete", parameters);

// 記錄收益
AnalyticsManager.Instance.LogRevenue("purchase", 30.0, "TWD");

// 記錄廣告事件
AnalyticsManager.Instance.LogAdEvent("rewarded", "AdMob", "test_unit_id");

// 設定用戶屬性
AnalyticsManager.Instance.SetUserProperty("user_type", "premium");
```

## ⚙️ 配置設定

### 廣告配置 (AdConfig)
```csharp
[System.Serializable]
public class AdConfig
{
    [Header("AdMob 設定")]
    public string admobAppId = "ca-app-pub-3940256099942544~3347511713";
    public string admobBannerId = "ca-app-pub-3940256099942544/9214589741";
    // ... 其他廣告單元 ID
    
    [Header("AppLovin MAX 設定")]
    public string maxSdkKey = "YOUR_MAX_SDK_KEY";
    // ... 其他 MAX 設定
    
    [Header("其他設定")]
    public bool enableTestMode = true;
    public float adLoadTimeout = 30f;
}
```

### 內購配置 (BillingConfig)
```csharp
[System.Serializable]
public class BillingConfig
{
    public ProductConfig[] products = new ProductConfig[]
    {
        new ProductConfig { 
            productId = "com.example.coins_100", 
            productType = ProductType.Consumable, 
            price = "NT$ 30" 
        },
        // ... 其他商品
    };
}
```

### 分析配置 (AnalyticsConfig)
```csharp
[System.Serializable]
public class AnalyticsConfig
{
    [Header("Firebase 設定")]
    public string firebaseProjectId = "your-firebase-project-id";
    
    [Header("GameAnalytics 設定")]
    public string gameAnalyticsGameKey = "your-ga-game-key";
    
    // ... 其他分析平台設定
}
```

## 🎮 測試場景

專案包含完整的測試場景 (`AdMediationLabController`)，提供：

- **廣告測試**: 所有廣告類型的載入和顯示
- **內購測試**: 商品載入、購買、恢復購買
- **分析測試**: 各種事件記錄和用戶屬性設定
- **即時日誌**: 所有操作的即時反饋和狀態顯示

## 🔧 與原版 Plankton 的差異

### ✅ 已解決的付費限制
- **廣告功能**: 完全免費，支援所有廣告網絡
- **內購功能**: 完全免費，支援所有平台
- **分析功能**: 完全免費，支援所有分析平台
- **遊戲服務**: 完全免費，支援所有功能

### 🆕 新增功能
- **統一介面**: 更簡潔易用的 API
- **完整文檔**: 詳細的使用說明和範例
- **測試工具**: 完整的測試場景和控制器
- **錯誤處理**: 更好的錯誤處理和日誌記錄
- **事件系統**: 完整的事件回調系統

## 📝 使用注意事項

1. **測試模式**: 預設啟用測試模式，使用測試廣告單元 ID
2. **平台支援**: 支援 Android、iOS 和 Unity Editor
3. **網路依賴**: 需要網路連線才能載入廣告
4. **權限設定**: 需要適當的平台權限設定

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request 來改善這個專案！

## 📄 授權

此專案採用 MIT 授權條款。

---

**注意**: 這是 Plankton package 的完整重寫版本，解決了所有付費限制，提供完全免費的廣告中介和遊戲服務功能。
