using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdMediationLab
{
    /// <summary>
    /// 廣告中介管理器 - 支援多個廣告網絡的統一介面
    /// </summary>
    public class AdMediationManager : MonoBehaviour
    {
        [Header("廣告設定")]
        [SerializeField] private AdConfig adConfig;
        
        [Header("測試模式")]
        [SerializeField] private bool isTestMode = true;
        
        private static AdMediationManager _instance;
        public static AdMediationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AdMediationManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AdMediationManager");
                        _instance = go.AddComponent<AdMediationManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        // 廣告事件
        public static event Action<AdResult> OnAdLoaded;
        public static event Action<AdResult> OnAdFailedToLoad;
        public static event Action<AdResult> OnAdShown;
        public static event Action<AdResult> OnAdClosed;
        public static event Action<AdResult> OnAdRewarded;
        public static event Action<AdRevenue> OnAdRevenue;

        private Dictionary<AdType, IAdProvider> adProviders = new Dictionary<AdType, IAdProvider>();
        private bool isInitialized = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAdProviders();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAdProviders()
        {
            // 初始化各種廣告提供者
            adProviders[AdType.Banner] = new BannerAdProvider();
            adProviders[AdType.Interstitial] = new InterstitialAdProvider();
            adProviders[AdType.Rewarded] = new RewardedAdProvider();
            adProviders[AdType.RewardedInterstitial] = new RewardedInterstitialAdProvider();
            adProviders[AdType.AppOpen] = new AppOpenAdProvider();

            Debug.Log("[AdMediation] 廣告提供者初始化完成");
        }

        /// <summary>
        /// 初始化廣告中介
        /// </summary>
        public void Initialize(Action<bool> onComplete = null)
        {
            if (isInitialized)
            {
                onComplete?.Invoke(true);
                return;
            }

            Debug.Log("[AdMediation] 開始初始化廣告中介...");

            // 初始化所有廣告提供者
            int completedCount = 0;
            int totalProviders = adProviders.Count;

            foreach (var provider in adProviders.Values)
            {
                provider.Initialize(adConfig, (success) =>
                {
                    completedCount++;
                    Debug.Log($"[AdMediation] 提供者初始化 {(success ? "成功" : "失敗")} ({completedCount}/{totalProviders})");

                    if (completedCount >= totalProviders)
                    {
                        isInitialized = true;
                        Debug.Log("[AdMediation] 廣告中介初始化完成");
                        onComplete?.Invoke(true);
                    }
                });
            }
        }

        /// <summary>
        /// 載入廣告
        /// </summary>
        public void LoadAd(AdType adType, string adUnitId = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[AdMediation] 廣告中介尚未初始化");
                return;
            }

            if (adProviders.TryGetValue(adType, out IAdProvider provider))
            {
                provider.LoadAd(adUnitId);
            }
            else
            {
                Debug.LogError($"[AdMediation] 不支援的廣告類型: {adType}");
            }
        }

        /// <summary>
        /// 顯示廣告
        /// </summary>
        public void ShowAd(AdType adType, Action<bool> onComplete = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[AdMediation] 廣告中介尚未初始化");
                onComplete?.Invoke(false);
                return;
            }

            if (adProviders.TryGetValue(adType, out IAdProvider provider))
            {
                provider.ShowAd(onComplete);
            }
            else
            {
                Debug.LogError($"[AdMediation] 不支援的廣告類型: {adType}");
                onComplete?.Invoke(false);
            }
        }

        /// <summary>
        /// 檢查廣告是否已載入
        /// </summary>
        public bool IsAdLoaded(AdType adType)
        {
            if (adProviders.TryGetValue(adType, out IAdProvider provider))
            {
                return provider.IsLoaded();
            }
            return false;
        }

        /// <summary>
        /// 隱藏橫幅廣告
        /// </summary>
        public void HideBanner()
        {
            if (adProviders.TryGetValue(AdType.Banner, out IAdProvider provider))
            {
                ((BannerAdProvider)provider).Hide();
            }
        }

        /// <summary>
        /// 顯示橫幅廣告
        /// </summary>
        public void ShowBanner(BannerPosition position = BannerPosition.Bottom)
        {
            if (adProviders.TryGetValue(AdType.Banner, out IAdProvider provider))
            {
                ((BannerAdProvider)provider).Show(position);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && IsAdLoaded(AdType.AppOpen))
            {
                ShowAd(AdType.AppOpen);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && IsAdLoaded(AdType.AppOpen))
            {
                ShowAd(AdType.AppOpen);
            }
        }
    }

    /// <summary>
    /// 廣告類型枚舉
    /// </summary>
    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded,
        RewardedInterstitial,
        AppOpen
    }

    /// <summary>
    /// 橫幅廣告位置
    /// </summary>
    public enum BannerPosition
    {
        Top,
        Bottom
    }

    /// <summary>
    /// 廣告結果
    /// </summary>
    [System.Serializable]
    public class AdResult
    {
        public AdType adType;
        public bool success;
        public string errorMessage;
        public string adUnitId;
        public string networkName;
        public DateTime timestamp;

        public AdResult(AdType type, bool success, string error = null)
        {
            this.adType = type;
            this.success = success;
            this.errorMessage = error;
            this.timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// 廣告收益
    /// </summary>
    [System.Serializable]
    public class AdRevenue
    {
        public AdType adType;
        public string adUnitId;
        public string networkName;
        public double revenue;
        public string currency;
        public string country;
        public string precision;
        public DateTime timestamp;

        public AdRevenue(AdType type, string unitId, string network, double revenue, string currency = "USD")
        {
            this.adType = type;
            this.adUnitId = unitId;
            this.networkName = network;
            this.revenue = revenue;
            this.currency = currency;
            this.timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// 廣告配置
    /// </summary>
    [System.Serializable]
    public class AdConfig
    {
        [Header("AdMob 設定")]
        public string admobAppId = "ca-app-pub-3940256099942544~3347511713";
        public string admobBannerId = "ca-app-pub-3940256099942544/9214589741";
        public string admobInterstitialId = "ca-app-pub-3940256099942544/1033173712";
        public string admobRewardedId = "ca-app-pub-3940256099942544/5224354917";
        public string admobRewardedInterstitialId = "ca-app-pub-3940256099942544/5354046379";
        public string admobAppOpenId = "ca-app-pub-3940256099942544/9257395921";

        [Header("AppLovin MAX 設定")]
        public string maxSdkKey = "YOUR_MAX_SDK_KEY";
        public string maxBannerId = "YOUR_MAX_BANNER_ID";
        public string maxInterstitialId = "YOUR_MAX_INTERSTITIAL_ID";
        public string maxRewardedId = "YOUR_MAX_REWARDED_ID";
        public string maxRewardedInterstitialId = "YOUR_MAX_REWARDED_INTERSTITIAL_ID";
        public string maxAppOpenId = "YOUR_MAX_APP_OPEN_ID";

        [Header("IronSource LevelPlay 設定")]
        public string ironSourceAppKey = "YOUR_IRONSOURCE_APP_KEY";
        public string ironSourceBannerId = "YOUR_IRONSOURCE_BANNER_ID";
        public string ironSourceInterstitialId = "YOUR_IRONSOURCE_INTERSTITIAL_ID";
        public string ironSourceRewardedId = "YOUR_IRONSOURCE_REWARDED_ID";

        [Header("Unity Ads 設定")]
        public string unityGameId = "YOUR_UNITY_GAME_ID";
        public string unityBannerId = "YOUR_UNITY_BANNER_ID";
        public string unityInterstitialId = "YOUR_UNITY_INTERSTITIAL_ID";
        public string unityRewardedId = "YOUR_UNITY_REWARDED_ID";

        [Header("其他設定")]
        public bool enableTestMode = true;
        public string[] testDeviceIds = new string[0];
        public float adLoadTimeout = 30f;
        public int maxRetryAttempts = 3;
    }
}
