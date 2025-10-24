using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdMediationLab
{
    /// <summary>
    /// 廣告中介實驗室測試控制器
    /// </summary>
    public class AdMediationLabController : MonoBehaviour
    {
        [Header("UI 控制")]
        [SerializeField] private Button initializeButton;
        [SerializeField] private Button loadBannerButton;
        [SerializeField] private Button showBannerButton;
        [SerializeField] private Button hideBannerButton;
        [SerializeField] private Button loadInterstitialButton;
        [SerializeField] private Button showInterstitialButton;
        [SerializeField] private Button loadRewardedButton;
        [SerializeField] private Button showRewardedButton;
        [SerializeField] private Button loadAppOpenButton;
        [SerializeField] private Button showAppOpenButton;
        
        [Header("內購測試")]
        [SerializeField] private Button initializeBillingButton;
        [SerializeField] private Button loadProductsButton;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button restoreButton;
        [SerializeField] private InputField productIdInput;
        
        [Header("分析測試")]
        [SerializeField] private Button initializeAnalyticsButton;
        [SerializeField] private Button logEventButton;
        [SerializeField] private Button logLevelUpButton;
        [SerializeField] private Button logPurchaseButton;
        [SerializeField] private Button logAdEventButton;
        
        [Header("狀態顯示")]
        [SerializeField] private Text statusText;
        [SerializeField] private ScrollRect logScrollRect;
        [SerializeField] private Text logText;
        
        private bool isAdInitialized = false;
        private bool isBillingInitialized = false;
        private bool isAnalyticsInitialized = false;
        
        private void Start()
        {
            SetupUI();
            SetupEventListeners();
            LogMessage("廣告中介實驗室啟動");
        }

        private void SetupUI()
        {
            // 設定預設商品 ID
            if (productIdInput != null)
                productIdInput.text = "com.example.coins_100";
        }

        private void SetupEventListeners()
        {
            // 廣告按鈕
            if (initializeButton != null)
                initializeButton.onClick.AddListener(InitializeAds);
            
            if (loadBannerButton != null)
                loadBannerButton.onClick.AddListener(LoadBanner);
            
            if (showBannerButton != null)
                showBannerButton.onClick.AddListener(() => ShowBanner(BannerPosition.Bottom));
            
            if (hideBannerButton != null)
                hideBannerButton.onClick.AddListener(HideBanner);
            
            if (loadInterstitialButton != null)
                loadInterstitialButton.onClick.AddListener(LoadInterstitial);
            
            if (showInterstitialButton != null)
                showInterstitialButton.onClick.AddListener(ShowInterstitial);
            
            if (loadRewardedButton != null)
                loadRewardedButton.onClick.AddListener(LoadRewarded);
            
            if (showRewardedButton != null)
                showRewardedButton.onClick.AddListener(ShowRewarded);
            
            if (loadAppOpenButton != null)
                loadAppOpenButton.onClick.AddListener(LoadAppOpen);
            
            if (showAppOpenButton != null)
                showAppOpenButton.onClick.AddListener(ShowAppOpen);

            // 內購按鈕
            if (initializeBillingButton != null)
                initializeBillingButton.onClick.AddListener(InitializeBilling);
            
            if (loadProductsButton != null)
                loadProductsButton.onClick.AddListener(LoadProducts);
            
            if (purchaseButton != null)
                purchaseButton.onClick.AddListener(PurchaseProduct);
            
            if (restoreButton != null)
                restoreButton.onClick.AddListener(RestorePurchases);

            // 分析按鈕
            if (initializeAnalyticsButton != null)
                initializeAnalyticsButton.onClick.AddListener(InitializeAnalytics);
            
            if (logEventButton != null)
                logEventButton.onClick.AddListener(LogTestEvent);
            
            if (logLevelUpButton != null)
                logLevelUpButton.onClick.AddListener(LogLevelUp);
            
            if (logPurchaseButton != null)
                logPurchaseButton.onClick.AddListener(LogPurchase);
            
            if (logAdEventButton != null)
                logAdEventButton.onClick.AddListener(LogAdEvent);

            // 訂閱事件
            AdMediationManager.OnAdLoaded += OnAdLoaded;
            AdMediationManager.OnAdShown += OnAdShown;
            AdMediationManager.OnAdClosed += OnAdClosed;
            AdMediationManager.OnAdRewarded += OnAdRewarded;
            AdMediationManager.OnAdRevenue += OnAdRevenue;

            BillingManager.OnPurchaseSuccess += OnPurchaseSuccess;
            BillingManager.OnPurchaseFailed += OnPurchaseFailed;
            BillingManager.OnProductsLoaded += OnProductsLoaded;
        }

        #region 廣告功能

        private void InitializeAds()
        {
            LogMessage("開始初始化廣告系統...");
            AdMediationManager.Instance.Initialize((success) =>
            {
                isAdInitialized = success;
                LogMessage($"廣告系統初始化 {(success ? "成功" : "失敗")}");
                UpdateStatus();
            });
        }

        private void LoadBanner()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("載入橫幅廣告...");
            AdMediationManager.Instance.LoadAd(AdType.Banner);
        }

        private void ShowBanner(BannerPosition position)
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage($"顯示橫幅廣告 ({position})");
            AdMediationManager.Instance.ShowBanner(position);
        }

        private void HideBanner()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("隱藏橫幅廣告");
            AdMediationManager.Instance.HideBanner();
        }

        private void LoadInterstitial()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("載入插頁式廣告...");
            AdMediationManager.Instance.LoadAd(AdType.Interstitial);
        }

        private void ShowInterstitial()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("顯示插頁式廣告");
            AdMediationManager.Instance.ShowAd(AdType.Interstitial, (success) =>
            {
                LogMessage($"插頁式廣告顯示 {(success ? "成功" : "失敗")}");
            });
        }

        private void LoadRewarded()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("載入獎勵式廣告...");
            AdMediationManager.Instance.LoadAd(AdType.Rewarded);
        }

        private void ShowRewarded()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("顯示獎勵式廣告");
            AdMediationManager.Instance.ShowAd(AdType.Rewarded, (success) =>
            {
                LogMessage($"獎勵式廣告顯示 {(success ? "成功" : "失敗")}");
            });
        }

        private void LoadAppOpen()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("載入應用開啟廣告...");
            AdMediationManager.Instance.LoadAd(AdType.AppOpen);
        }

        private void ShowAppOpen()
        {
            if (!isAdInitialized)
            {
                LogMessage("請先初始化廣告系統");
                return;
            }
            
            LogMessage("顯示應用開啟廣告");
            AdMediationManager.Instance.ShowAd(AdType.AppOpen, (success) =>
            {
                LogMessage($"應用開啟廣告顯示 {(success ? "成功" : "失敗")}");
            });
        }

        #endregion

        #region 內購功能

        private void InitializeBilling()
        {
            LogMessage("開始初始化內購系統...");
            BillingManager.Instance.Initialize((success) =>
            {
                isBillingInitialized = success;
                LogMessage($"內購系統初始化 {(success ? "成功" : "失敗")}");
                UpdateStatus();
            });
        }

        private void LoadProducts()
        {
            if (!isBillingInitialized)
            {
                LogMessage("請先初始化內購系統");
                return;
            }
            
            string[] productIds = {
                "com.example.coins_100",
                "com.example.coins_500",
                "com.example.remove_ads",
                "com.example.premium"
            };
            
            LogMessage("載入商品資訊...");
            BillingManager.Instance.LoadProducts(productIds, (success) =>
            {
                LogMessage($"商品載入 {(success ? "成功" : "失敗")}");
            });
        }

        private void PurchaseProduct()
        {
            if (!isBillingInitialized)
            {
                LogMessage("請先初始化內購系統");
                return;
            }
            
            string productId = productIdInput != null ? productIdInput.text : "com.example.coins_100";
            LogMessage($"購買商品: {productId}");
            
            BillingManager.Instance.PurchaseProduct(productId, (result) =>
            {
                LogMessage($"購買結果: {(result.success ? "成功" : "失敗")} - {result.errorMessage}");
            });
        }

        private void RestorePurchases()
        {
            if (!isBillingInitialized)
            {
                LogMessage("請先初始化內購系統");
                return;
            }
            
            LogMessage("恢復購買...");
            BillingManager.Instance.RestorePurchases((results) =>
            {
                LogMessage($"恢復購買完成，共 {results.Count} 個商品");
            });
        }

        #endregion

        #region 分析功能

        private void InitializeAnalytics()
        {
            LogMessage("開始初始化分析系統...");
            AnalyticsManager.Instance.Initialize((success) =>
            {
                isAnalyticsInitialized = success;
                LogMessage($"分析系統初始化 {(success ? "成功" : "失敗")}");
                UpdateStatus();
            });
        }

        private void LogTestEvent()
        {
            if (!isAnalyticsInitialized)
            {
                LogMessage("請先初始化分析系統");
                return;
            }
            
            var parameters = new Dictionary<string, object>
            {
                { "test_param1", "test_value1" },
                { "test_param2", 123 },
                { "test_param3", true }
            };
            
            LogMessage("記錄測試事件");
            AnalyticsManager.Instance.LogEvent("test_event", parameters);
        }

        private void LogLevelUp()
        {
            if (!isAnalyticsInitialized)
            {
                LogMessage("請先初始化分析系統");
                return;
            }
            
            LogMessage("記錄等級提升事件");
            AnalyticsManager.Instance.LogLevelUp(5);
        }

        private void LogPurchase()
        {
            if (!isAnalyticsInitialized)
            {
                LogMessage("請先初始化分析系統");
                return;
            }
            
            LogMessage("記錄購買事件");
            AnalyticsManager.Instance.LogPurchase("com.example.coins_100", 30.0, "TWD");
        }

        private void LogAdEvent()
        {
            if (!isAnalyticsInitialized)
            {
                LogMessage("請先初始化分析系統");
                return;
            }
            
            LogMessage("記錄廣告事件");
            AnalyticsManager.Instance.LogAdEvent("rewarded", "AdMob", "test_ad_unit_id");
        }

        #endregion

        #region 事件處理

        private void OnAdLoaded(AdResult result)
        {
            LogMessage($"廣告載入成功: {result.adType}");
        }

        private void OnAdShown(AdResult result)
        {
            LogMessage($"廣告顯示: {result.adType}");
        }

        private void OnAdClosed(AdResult result)
        {
            LogMessage($"廣告關閉: {result.adType}");
        }

        private void OnAdRewarded(AdResult result)
        {
            LogMessage($"廣告獎勵獲得: {result.adType}");
        }

        private void OnAdRevenue(AdRevenue revenue)
        {
            LogMessage($"廣告收益: {revenue.adType} - {revenue.revenue} {revenue.currency}");
        }

        private void OnPurchaseSuccess(PurchaseResult result)
        {
            LogMessage($"購買成功: {result.productId}");
        }

        private void OnPurchaseFailed(PurchaseResult result)
        {
            LogMessage($"購買失敗: {result.errorMessage}");
        }

        private void OnProductsLoaded(List<ProductInfo> products)
        {
            LogMessage($"商品載入成功，共 {products.Count} 個商品");
            foreach (var product in products)
            {
                LogMessage($"  - {product.productId}: {product.title} ({product.price})");
            }
        }

        #endregion

        #region UI 更新

        private void UpdateStatus()
        {
            if (statusText != null)
            {
                statusText.text = $"廣告: {(isAdInitialized ? "✓" : "✗")} | " +
                                $"內購: {(isBillingInitialized ? "✓" : "✗")} | " +
                                $"分析: {(isAnalyticsInitialized ? "✓" : "✗")}";
            }
        }

        private void LogMessage(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            string logMessage = $"[{timestamp}] {message}";
            
            Debug.Log(logMessage);
            
            if (logText != null)
            {
                logText.text += logMessage + "\n";
                
                // 自動滾動到底部
                if (logScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
                }
            }
        }

        #endregion

        private void OnDestroy()
        {
            // 取消訂閱事件
            AdMediationManager.OnAdLoaded -= OnAdLoaded;
            AdMediationManager.OnAdShown -= OnAdShown;
            AdMediationManager.OnAdClosed -= OnAdClosed;
            AdMediationManager.OnAdRewarded -= OnAdRewarded;
            AdMediationManager.OnAdRevenue -= OnAdRevenue;

            BillingManager.OnPurchaseSuccess -= OnPurchaseSuccess;
            BillingManager.OnPurchaseFailed -= OnPurchaseFailed;
            BillingManager.OnProductsLoaded -= OnProductsLoaded;
        }
    }
}
