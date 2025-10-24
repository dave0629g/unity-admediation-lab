using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdMediationLab
{
    /// <summary>
    /// 分析管理器 - 支援多個分析平台
    /// </summary>
    public class AnalyticsManager : MonoBehaviour
    {
        [Header("分析設定")]
        [SerializeField] private AnalyticsConfig analyticsConfig;
        
        private static AnalyticsManager _instance;
        public static AnalyticsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AnalyticsManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AnalyticsManager");
                        _instance = go.AddComponent<AnalyticsManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private Dictionary<AnalyticsProvider, IAnalyticsProvider> providers = new Dictionary<AnalyticsProvider, IAnalyticsProvider>();
        private bool isInitialized = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeProviders();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeProviders()
        {
            // 初始化各種分析提供者
            providers[AnalyticsProvider.Firebase] = new FirebaseAnalyticsProvider();
            providers[AnalyticsProvider.GameAnalytics] = new GameAnalyticsProvider();
            providers[AnalyticsProvider.AppsFlyer] = new AppsFlyerProvider();
            providers[AnalyticsProvider.UnityAnalytics] = new UnityAnalyticsProvider();
            providers[AnalyticsProvider.Facebook] = new FacebookAnalyticsProvider();

            Debug.Log("[Analytics] 分析提供者初始化完成");
        }

        /// <summary>
        /// 初始化分析系統
        /// </summary>
        public void Initialize(Action<bool> onComplete = null)
        {
            if (isInitialized)
            {
                onComplete?.Invoke(true);
                return;
            }

            Debug.Log("[Analytics] 開始初始化分析系統...");

            int completedCount = 0;
            int totalProviders = providers.Count;

            foreach (var provider in providers.Values)
            {
                provider.Initialize(analyticsConfig, (success) =>
                {
                    completedCount++;
                    Debug.Log($"[Analytics] 提供者初始化 {(success ? "成功" : "失敗")} ({completedCount}/{totalProviders})");

                    if (completedCount >= totalProviders)
                    {
                        isInitialized = true;
                        Debug.Log("[Analytics] 分析系統初始化完成");
                        onComplete?.Invoke(true);
                    }
                });
            }
        }

        /// <summary>
        /// 記錄事件
        /// </summary>
        public void LogEvent(string eventName, Dictionary<string, object> parameters = null, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Analytics] 分析系統尚未初始化");
                return;
            }

            var targetProviders = GetTargetProviders(provider);
            
            foreach (var targetProvider in targetProviders)
            {
                if (providers.TryGetValue(targetProvider, out IAnalyticsProvider analyticsProvider))
                {
                    analyticsProvider.LogEvent(eventName, parameters);
                }
            }
        }

        /// <summary>
        /// 記錄事件 (簡化版本)
        /// </summary>
        public void LogEvent(string eventName, string parameterName, object parameterValue, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { parameterName, parameterValue }
            };
            LogEvent(eventName, parameters, provider);
        }

        /// <summary>
        /// 記錄收益事件
        /// </summary>
        public void LogRevenue(string eventName, double amount, string currency = "USD", AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { "amount", amount },
                { "currency", currency }
            };
            LogEvent(eventName, parameters, provider);
        }

        /// <summary>
        /// 設定用戶屬性
        /// </summary>
        public void SetUserProperty(string propertyName, string propertyValue, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Analytics] 分析系統尚未初始化");
                return;
            }

            var targetProviders = GetTargetProviders(provider);
            
            foreach (var targetProvider in targetProviders)
            {
                if (providers.TryGetValue(targetProvider, out IAnalyticsProvider analyticsProvider))
                {
                    analyticsProvider.SetUserProperty(propertyName, propertyValue);
                }
            }
        }

        /// <summary>
        /// 記錄用戶等級
        /// </summary>
        public void LogLevelUp(int level, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            LogEvent("level_up", "level", level, provider);
        }

        /// <summary>
        /// 記錄關卡開始
        /// </summary>
        public void LogLevelStart(string levelName, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            LogEvent("level_start", "level_name", levelName, provider);
        }

        /// <summary>
        /// 記錄關卡完成
        /// </summary>
        public void LogLevelComplete(string levelName, int score = 0, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { "level_name", levelName },
                { "score", score }
            };
            LogEvent("level_complete", parameters, provider);
        }

        /// <summary>
        /// 記錄關卡失敗
        /// </summary>
        public void LogLevelFail(string levelName, string reason = "", AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { "level_name", levelName },
                { "reason", reason }
            };
            LogEvent("level_fail", parameters, provider);
        }

        /// <summary>
        /// 記錄購買事件
        /// </summary>
        public void LogPurchase(string productId, double amount, string currency = "USD", AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { "product_id", productId },
                { "amount", amount },
                { "currency", currency }
            };
            LogEvent("purchase", parameters, provider);
        }

        /// <summary>
        /// 記錄廣告事件
        /// </summary>
        public void LogAdEvent(string adType, string adNetwork, string adUnitId, AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { "ad_type", adType },
                { "ad_network", adNetwork },
                { "ad_unit_id", adUnitId }
            };
            LogEvent("ad_event", parameters, provider);
        }

        /// <summary>
        /// 記錄廣告收益
        /// </summary>
        public void LogAdRevenue(string adType, string adNetwork, double revenue, string currency = "USD", AnalyticsProvider provider = AnalyticsProvider.All)
        {
            var parameters = new Dictionary<string, object>
            {
                { "ad_type", adType },
                { "ad_network", adNetwork },
                { "revenue", revenue },
                { "currency", currency }
            };
            LogEvent("ad_revenue", parameters, provider);
        }

        private List<AnalyticsProvider> GetTargetProviders(AnalyticsProvider provider)
        {
            var targetProviders = new List<AnalyticsProvider>();
            
            if (provider == AnalyticsProvider.All)
            {
                targetProviders.AddRange(providers.Keys);
            }
            else
            {
                targetProviders.Add(provider);
            }
            
            return targetProviders;
        }
    }

    /// <summary>
    /// 分析提供者介面
    /// </summary>
    public interface IAnalyticsProvider
    {
        void Initialize(AnalyticsConfig config, Action<bool> onComplete);
        void LogEvent(string eventName, Dictionary<string, object> parameters = null);
        void SetUserProperty(string propertyName, string propertyValue);
    }

    /// <summary>
    /// Firebase 分析提供者
    /// </summary>
    public class FirebaseAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize(AnalyticsConfig config, Action<bool> onComplete)
        {
            Debug.Log("[FirebaseAnalytics] 初始化 Firebase 分析");
            // 模擬初始化
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[FirebaseAnalytics] 記錄事件: {eventName} - {FormatParameters(parameters)}");
        }

        public void SetUserProperty(string propertyName, string propertyValue)
        {
            Debug.Log($"[FirebaseAnalytics] 設定用戶屬性: {propertyName} = {propertyValue}");
        }

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private string FormatParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0) return "無參數";
            
            var paramStrings = new List<string>();
            foreach (var param in parameters)
            {
                paramStrings.Add($"{param.Key}: {param.Value}");
            }
            return string.Join(", ", paramStrings);
        }
    }

    /// <summary>
    /// GameAnalytics 提供者
    /// </summary>
    public class GameAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize(AnalyticsConfig config, Action<bool> onComplete)
        {
            Debug.Log("[GameAnalytics] 初始化 GameAnalytics");
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[GameAnalytics] 記錄事件: {eventName} - {FormatParameters(parameters)}");
        }

        public void SetUserProperty(string propertyName, string propertyValue)
        {
            Debug.Log($"[GameAnalytics] 設定用戶屬性: {propertyName} = {propertyValue}");
        }

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private string FormatParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0) return "無參數";
            
            var paramStrings = new List<string>();
            foreach (var param in parameters)
            {
                paramStrings.Add($"{param.Key}: {param.Value}");
            }
            return string.Join(", ", paramStrings);
        }
    }

    /// <summary>
    /// AppsFlyer 提供者
    /// </summary>
    public class AppsFlyerProvider : IAnalyticsProvider
    {
        public void Initialize(AnalyticsConfig config, Action<bool> onComplete)
        {
            Debug.Log("[AppsFlyer] 初始化 AppsFlyer");
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[AppsFlyer] 記錄事件: {eventName} - {FormatParameters(parameters)}");
        }

        public void SetUserProperty(string propertyName, string propertyValue)
        {
            Debug.Log($"[AppsFlyer] 設定用戶屬性: {propertyName} = {propertyValue}");
        }

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private string FormatParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0) return "無參數";
            
            var paramStrings = new List<string>();
            foreach (var param in parameters)
            {
                paramStrings.Add($"{param.Key}: {param.Value}");
            }
            return string.Join(", ", paramStrings);
        }
    }

    /// <summary>
    /// Unity Analytics 提供者
    /// </summary>
    public class UnityAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize(AnalyticsConfig config, Action<bool> onComplete)
        {
            Debug.Log("[UnityAnalytics] 初始化 Unity Analytics");
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[UnityAnalytics] 記錄事件: {eventName} - {FormatParameters(parameters)}");
        }

        public void SetUserProperty(string propertyName, string propertyValue)
        {
            Debug.Log($"[UnityAnalytics] 設定用戶屬性: {propertyName} = {propertyValue}");
        }

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private string FormatParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0) return "無參數";
            
            var paramStrings = new List<string>();
            foreach (var param in parameters)
            {
                paramStrings.Add($"{param.Key}: {param.Value}");
            }
            return string.Join(", ", paramStrings);
        }
    }

    /// <summary>
    /// Facebook Analytics 提供者
    /// </summary>
    public class FacebookAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize(AnalyticsConfig config, Action<bool> onComplete)
        {
            Debug.Log("[FacebookAnalytics] 初始化 Facebook Analytics");
            UnityEngine.Object.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                DelayedCallback(() => onComplete?.Invoke(true), 0.1f));
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[FacebookAnalytics] 記錄事件: {eventName} - {FormatParameters(parameters)}");
        }

        public void SetUserProperty(string propertyName, string propertyValue)
        {
            Debug.Log($"[FacebookAnalytics] 設定用戶屬性: {propertyName} = {propertyValue}");
        }

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private string FormatParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0) return "無參數";
            
            var paramStrings = new List<string>();
            foreach (var param in parameters)
            {
                paramStrings.Add($"{param.Key}: {param.Value}");
            }
            return string.Join(", ", paramStrings);
        }
    }

    /// <summary>
    /// 分析提供者枚舉
    /// </summary>
    public enum AnalyticsProvider
    {
        All,
        Firebase,
        GameAnalytics,
        AppsFlyer,
        UnityAnalytics,
        Facebook
    }

    /// <summary>
    /// 分析配置
    /// </summary>
    [System.Serializable]
    public class AnalyticsConfig
    {
        [Header("Firebase 設定")]
        public string firebaseProjectId = "your-firebase-project-id";
        public string firebaseApiKey = "your-firebase-api-key";

        [Header("GameAnalytics 設定")]
        public string gameAnalyticsGameKey = "your-ga-game-key";
        public string gameAnalyticsSecretKey = "your-ga-secret-key";

        [Header("AppsFlyer 設定")]
        public string appsFlyerDevKey = "your-appsflyer-dev-key";

        [Header("Unity Analytics 設定")]
        public string unityAnalyticsGameId = "your-unity-game-id";

        [Header("Facebook 設定")]
        public string facebookAppId = "your-facebook-app-id";

        [Header("其他設定")]
        public bool enableDebugMode = true;
        public bool enableTestMode = true;
    }
}
