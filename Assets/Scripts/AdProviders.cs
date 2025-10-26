using System;
using UnityEngine;

namespace AdMediationLab
{
    /// <summary>
    /// 廣告提供者介面
    /// </summary>
    public interface IAdProvider
    {
        void Initialize(AdConfig config, Action<bool> onComplete);
        void LoadAd(string adUnitId = null);
        void ShowAd(Action<bool> onComplete = null);
        bool IsLoaded();
        void SetAdUnitId(string adUnitId);
    }

    /// <summary>
    /// 橫幅廣告提供者
    /// </summary>
    public class BannerAdProvider : IAdProvider
    {
        private AdConfig config;
        private bool isLoaded = false;
        private bool isShowing = false;
        private string currentAdUnitId;

        public void Initialize(AdConfig config, Action<bool> onComplete)
        {
            this.config = config;
            Debug.Log("[BannerAd] 初始化橫幅廣告提供者");
            
            // 模擬初始化過程
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LoadAd(string adUnitId = null)
        {
            currentAdUnitId = adUnitId ?? config.admobBannerId;
            Debug.Log($"[BannerAd] 載入橫幅廣告: {currentAdUnitId}");
            
            // 模擬載入過程
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    isLoaded = true;
                    AdMediationManager.TriggerAdLoaded(new AdResult(AdType.Banner, true));
                }, 1f));
        }

        public void ShowAd(Action<bool> onComplete = null)
        {
            if (!isLoaded)
            {
                Debug.LogWarning("[BannerAd] 廣告尚未載入");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("[BannerAd] 顯示橫幅廣告");
            isShowing = true;
            AdMediationManager.TriggerAdShown(new AdResult(AdType.Banner, true));
            onComplete?.Invoke(true);
        }

        public void Show(BannerPosition position)
        {
            ShowAd();
            Debug.Log($"[BannerAd] 橫幅廣告顯示在: {position}");
        }

        public void Hide()
        {
            if (isShowing)
            {
                Debug.Log("[BannerAd] 隱藏橫幅廣告");
                isShowing = false;
                AdMediationManager.TriggerAdClosed(new AdResult(AdType.Banner, true));
            }
        }

        public bool IsLoaded() => isLoaded;
        public void SetAdUnitId(string adUnitId) => currentAdUnitId = adUnitId;

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 插頁式廣告提供者
    /// </summary>
    public class InterstitialAdProvider : IAdProvider
    {
        private AdConfig config;
        private bool isLoaded = false;
        private string currentAdUnitId;

        public void Initialize(AdConfig config, Action<bool> onComplete)
        {
            this.config = config;
            Debug.Log("[InterstitialAd] 初始化插頁式廣告提供者");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LoadAd(string adUnitId = null)
        {
            currentAdUnitId = adUnitId ?? config.admobInterstitialId;
            Debug.Log($"[InterstitialAd] 載入插頁式廣告: {currentAdUnitId}");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    isLoaded = true;
                    AdMediationManager.TriggerAdLoaded(new AdResult(AdType.Interstitial, true));
                }, 1.5f));
        }

        public void ShowAd(Action<bool> onComplete = null)
        {
            if (!isLoaded)
            {
                Debug.LogWarning("[InterstitialAd] 廣告尚未載入");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("[InterstitialAd] 顯示插頁式廣告");
            AdMediationManager.TriggerAdShown(new AdResult(AdType.Interstitial, true));
            
            // 模擬廣告顯示和關閉
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    AdMediationManager.TriggerAdClosed(new AdResult(AdType.Interstitial, true));
                    isLoaded = false; // 插頁式廣告使用後需要重新載入
                    onComplete?.Invoke(true);
                }, 3f));
        }

        public bool IsLoaded() => isLoaded;
        public void SetAdUnitId(string adUnitId) => currentAdUnitId = adUnitId;

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 獎勵式廣告提供者
    /// </summary>
    public class RewardedAdProvider : IAdProvider
    {
        private AdConfig config;
        private bool isLoaded = false;
        private string currentAdUnitId;

        public void Initialize(AdConfig config, Action<bool> onComplete)
        {
            this.config = config;
            Debug.Log("[RewardedAd] 初始化獎勵式廣告提供者");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LoadAd(string adUnitId = null)
        {
            currentAdUnitId = adUnitId ?? config.admobRewardedId;
            Debug.Log($"[RewardedAd] 載入獎勵式廣告: {currentAdUnitId}");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    isLoaded = true;
                    AdMediationManager.TriggerAdLoaded(new AdResult(AdType.Rewarded, true));
                }, 2f));
        }

        public void ShowAd(Action<bool> onComplete = null)
        {
            if (!isLoaded)
            {
                Debug.LogWarning("[RewardedAd] 廣告尚未載入");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("[RewardedAd] 顯示獎勵式廣告");
            AdMediationManager.TriggerAdShown(new AdResult(AdType.Rewarded, true));
            
            // 模擬獎勵式廣告流程
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    // 模擬用戶完成觀看獲得獎勵
                    AdMediationManager.TriggerAdRewarded(new AdResult(AdType.Rewarded, true));
                    AdMediationManager.TriggerAdClosed(new AdResult(AdType.Rewarded, true));
                    isLoaded = false;
                    onComplete?.Invoke(true);
                }, 4f));
        }

        public bool IsLoaded() => isLoaded;
        public void SetAdUnitId(string adUnitId) => currentAdUnitId = adUnitId;

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 獎勵式插頁廣告提供者
    /// </summary>
    public class RewardedInterstitialAdProvider : IAdProvider
    {
        private AdConfig config;
        private bool isLoaded = false;
        private string currentAdUnitId;

        public void Initialize(AdConfig config, Action<bool> onComplete)
        {
            this.config = config;
            Debug.Log("[RewardedInterstitialAd] 初始化獎勵式插頁廣告提供者");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LoadAd(string adUnitId = null)
        {
            currentAdUnitId = adUnitId ?? config.admobRewardedInterstitialId;
            Debug.Log($"[RewardedInterstitialAd] 載入獎勵式插頁廣告: {currentAdUnitId}");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    isLoaded = true;
                    AdMediationManager.TriggerAdLoaded(new AdResult(AdType.RewardedInterstitial, true));
                }, 2f));
        }

        public void ShowAd(Action<bool> onComplete = null)
        {
            if (!isLoaded)
            {
                Debug.LogWarning("[RewardedInterstitialAd] 廣告尚未載入");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("[RewardedInterstitialAd] 顯示獎勵式插頁廣告");
            AdMediationManager.TriggerAdShown(new AdResult(AdType.RewardedInterstitial, true));
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    AdMediationManager.TriggerAdRewarded(new AdResult(AdType.RewardedInterstitial, true));
                    AdMediationManager.TriggerAdClosed(new AdResult(AdType.RewardedInterstitial, true));
                    isLoaded = false;
                    onComplete?.Invoke(true);
                }, 4f));
        }

        public bool IsLoaded() => isLoaded;
        public void SetAdUnitId(string adUnitId) => currentAdUnitId = adUnitId;

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 應用開啟廣告提供者
    /// </summary>
    public class AppOpenAdProvider : IAdProvider
    {
        private AdConfig config;
        private bool isLoaded = false;
        private string currentAdUnitId;

        public void Initialize(AdConfig config, Action<bool> onComplete)
        {
            this.config = config;
            Debug.Log("[AppOpenAd] 初始化應用開啟廣告提供者");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LoadAd(string adUnitId = null)
        {
            currentAdUnitId = adUnitId ?? config.admobAppOpenId;
            Debug.Log($"[AppOpenAd] 載入應用開啟廣告: {currentAdUnitId}");
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    isLoaded = true;
                    AdMediationManager.TriggerAdLoaded(new AdResult(AdType.AppOpen, true));
                }, 1f));
        }

        public void ShowAd(Action<bool> onComplete = null)
        {
            if (!isLoaded)
            {
                Debug.LogWarning("[AppOpenAd] 廣告尚未載入");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("[AppOpenAd] 顯示應用開啟廣告");
            AdMediationManager.TriggerAdShown(new AdResult(AdType.AppOpen, true));
            
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => {
                    AdMediationManager.TriggerAdClosed(new AdResult(AdType.AppOpen, true));
                    isLoaded = false;
                    onComplete?.Invoke(true);
                }, 2f));
        }

        public bool IsLoaded() => isLoaded;
        public void SetAdUnitId(string adUnitId) => currentAdUnitId = adUnitId;

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}
