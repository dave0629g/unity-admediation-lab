using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdMediationLab
{
    /// <summary>
    /// 內購管理器 - 支援多平台內購功能
    /// </summary>
    public class BillingManager : MonoBehaviour
    {
        [Header("內購設定")]
        [SerializeField] private BillingConfig billingConfig;
        
        private static BillingManager _instance;
        public static BillingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BillingManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("BillingManager");
                        _instance = go.AddComponent<BillingManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        // 內購事件
        public static event Action<PurchaseResult> OnPurchaseSuccess;
        public static event Action<PurchaseResult> OnPurchaseFailed;
        public static event Action<PurchaseResult> OnPurchaseRestored;
        public static event Action<List<ProductInfo>> OnProductsLoaded;
        public static event Action<List<PurchaseInfo>> OnPurchasesLoaded;

        private Dictionary<string, ProductInfo> products = new Dictionary<string, ProductInfo>();
        private Dictionary<string, PurchaseInfo> purchases = new Dictionary<string, PurchaseInfo>();
        private bool isInitialized = false;
        private bool isInitializing = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初始化內購系統
        /// </summary>
        public void Initialize(Action<bool> onComplete = null)
        {
            if (isInitialized)
            {
                onComplete?.Invoke(true);
                return;
            }

            if (isInitializing)
            {
                Debug.LogWarning("[Billing] 內購系統正在初始化中...");
                return;
            }

            isInitializing = true;
            Debug.Log("[Billing] 開始初始化內購系統...");

            // 模擬初始化過程
            StartCoroutine(DelayedCallback(() =>
            {
                isInitialized = true;
                isInitializing = false;
                Debug.Log("[Billing] 內購系統初始化完成");
                onComplete?.Invoke(true);
            }, 1f));
        }

        /// <summary>
        /// 載入商品資訊
        /// </summary>
        public void LoadProducts(string[] productIds, Action<bool> onComplete = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Billing] 內購系統尚未初始化");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log($"[Billing] 載入商品資訊: {string.Join(", ", productIds)}");

            // 模擬載入商品資訊
            StartCoroutine(DelayedCallback(() =>
            {
                var productList = new List<ProductInfo>();
                
                foreach (string productId in productIds)
                {
                    var product = new ProductInfo
                    {
                        productId = productId,
                        title = $"商品 {productId}",
                        description = $"這是商品 {productId} 的描述",
                        price = "NT$ 30",
                        priceAmount = 30.0m,
                        priceCurrency = "TWD",
                        type = ProductType.Consumable
                    };
                    
                    products[productId] = product;
                    productList.Add(product);
                }

                OnProductsLoaded?.Invoke(productList);
                onComplete?.Invoke(true);
            }, 0.5f));
        }

        /// <summary>
        /// 購買商品
        /// </summary>
        public void PurchaseProduct(string productId, Action<PurchaseResult> onComplete = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Billing] 內購系統尚未初始化");
                onComplete?.Invoke(new PurchaseResult { success = false, errorMessage = "內購系統尚未初始化" });
                return;
            }

            if (!products.ContainsKey(productId))
            {
                Debug.LogWarning($"[Billing] 商品不存在: {productId}");
                onComplete?.Invoke(new PurchaseResult { success = false, errorMessage = "商品不存在" });
                return;
            }

            Debug.Log($"[Billing] 開始購買商品: {productId}");

            // 模擬購買過程
            StartCoroutine(DelayedCallback(() =>
            {
                var result = new PurchaseResult
                {
                    success = true,
                    productId = productId,
                    purchaseToken = Guid.NewGuid().ToString(),
                    transactionId = Guid.NewGuid().ToString(),
                    purchaseTime = DateTime.Now,
                    productInfo = products[productId]
                };

                // 記錄購買
                var purchaseInfo = new PurchaseInfo
                {
                    productId = productId,
                    purchaseToken = result.purchaseToken,
                    transactionId = result.transactionId,
                    purchaseTime = result.purchaseTime,
                    status = PurchaseStatus.Purchased
                };

                purchases[result.purchaseToken] = purchaseInfo;

                OnPurchaseSuccess?.Invoke(result);
                onComplete?.Invoke(result);
            }, 2f));
        }

        /// <summary>
        /// 恢復購買 (iOS)
        /// </summary>
        public void RestorePurchases(Action<List<PurchaseResult>> onComplete = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Billing] 內購系統尚未初始化");
                onComplete?.Invoke(new List<PurchaseResult>());
                return;
            }

            Debug.Log("[Billing] 恢復購買");

            // 模擬恢復購買
            StartCoroutine(DelayedCallback(() =>
            {
                var restoredPurchases = new List<PurchaseResult>();
                
                foreach (var purchase in purchases.Values)
                {
                    if (purchase.status == PurchaseStatus.Purchased)
                    {
                        var result = new PurchaseResult
                        {
                            success = true,
                            productId = purchase.productId,
                            purchaseToken = purchase.purchaseToken,
                            transactionId = purchase.transactionId,
                            purchaseTime = purchase.purchaseTime,
                            productInfo = products.ContainsKey(purchase.productId) ? products[purchase.productId] : null
                        };
                        restoredPurchases.Add(result);
                    }
                }

                OnPurchaseRestored?.Invoke(restoredPurchases[0]);
                onComplete?.Invoke(restoredPurchases);
            }, 1f));
        }

        /// <summary>
        /// 確認購買 (Android)
        /// </summary>
        public void AcknowledgePurchase(string purchaseToken, Action<bool> onComplete = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Billing] 內購系統尚未初始化");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log($"[Billing] 確認購買: {purchaseToken}");

            if (purchases.ContainsKey(purchaseToken))
            {
                purchases[purchaseToken].status = PurchaseStatus.Acknowledged;
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogWarning($"[Billing] 找不到購買記錄: {purchaseToken}");
                onComplete?.Invoke(false);
            }
        }

        /// <summary>
        /// 消耗商品 (Android)
        /// </summary>
        public void ConsumePurchase(string purchaseToken, Action<bool> onComplete = null)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("[Billing] 內購系統尚未初始化");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log($"[Billing] 消耗商品: {purchaseToken}");

            if (purchases.ContainsKey(purchaseToken))
            {
                purchases.Remove(purchaseToken);
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogWarning($"[Billing] 找不到購買記錄: {purchaseToken}");
                onComplete?.Invoke(false);
            }
        }

        /// <summary>
        /// 獲取商品資訊
        /// </summary>
        public ProductInfo GetProductInfo(string productId)
        {
            return products.ContainsKey(productId) ? products[productId] : null;
        }

        /// <summary>
        /// 獲取所有商品
        /// </summary>
        public List<ProductInfo> GetAllProducts()
        {
            return new List<ProductInfo>(products.Values);
        }

        /// <summary>
        /// 獲取所有購買記錄
        /// </summary>
        public List<PurchaseInfo> GetAllPurchases()
        {
            return new List<PurchaseInfo>(purchases.Values);
        }

        /// <summary>
        /// 檢查是否已購買商品
        /// </summary>
        public bool HasPurchased(string productId)
        {
            foreach (var purchase in purchases.Values)
            {
                if (purchase.productId == productId && purchase.status == PurchaseStatus.Purchased)
                    return true;
            }
            return false;
        }

        private System.Collections.IEnumerator DelayedCallback(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 內購配置
    /// </summary>
    [System.Serializable]
    public class BillingConfig
    {
        [Header("商品設定")]
        public ProductConfig[] products = new ProductConfig[]
        {
            new ProductConfig { productId = "com.example.coins_100", productType = ProductType.Consumable, price = "NT$ 30" },
            new ProductConfig { productId = "com.example.coins_500", productType = ProductType.Consumable, price = "NT$ 150" },
            new ProductConfig { productId = "com.example.remove_ads", productType = ProductType.NonConsumable, price = "NT$ 120" },
            new ProductConfig { productId = "com.example.premium", productType = ProductType.Subscription, price = "NT$ 60/月" }
        };

        [Header("測試設定")]
        public bool enableTestMode = true;
        public string[] testDeviceIds = new string[0];
    }

    /// <summary>
    /// 商品配置
    /// </summary>
    [System.Serializable]
    public class ProductConfig
    {
        public string productId;
        public ProductType productType;
        public string price;
        public string title;
        public string description;
    }

    /// <summary>
    /// 商品類型
    /// </summary>
    public enum ProductType
    {
        Consumable,     // 消耗品
        NonConsumable,  // 非消耗品
        Subscription    // 訂閱
    }

    /// <summary>
    /// 購買狀態
    /// </summary>
    public enum PurchaseStatus
    {
        Purchased,
        Acknowledged,
        Consumed,
        Failed,
        Cancelled,
        Pending
    }

    /// <summary>
    /// 商品資訊
    /// </summary>
    [System.Serializable]
    public class ProductInfo
    {
        public string productId;
        public string title;
        public string description;
        public string price;
        public decimal priceAmount;
        public string priceCurrency;
        public ProductType type;
    }

    /// <summary>
    /// 購買資訊
    /// </summary>
    [System.Serializable]
    public class PurchaseInfo
    {
        public string productId;
        public string purchaseToken;
        public string transactionId;
        public DateTime purchaseTime;
        public PurchaseStatus status;
    }

    /// <summary>
    /// 購買結果
    /// </summary>
    [System.Serializable]
    public class PurchaseResult
    {
        public bool success;
        public string productId;
        public string purchaseToken;
        public string transactionId;
        public DateTime purchaseTime;
        public string errorMessage;
        public ProductInfo productInfo;
    }
}
