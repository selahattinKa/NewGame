using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace CanavarZindanlari.Economy
{
    /// <summary>
    /// Google Play Billing / Apple StoreKit sarmalayıcı.
    /// Paket yüklü değilse UNITY_PURCHASING tanımsız kalır — stub olarak çalışır.
    /// Maps to: design/gdd/iap-reklam-sistemi (Not Started — prototype stub)
    /// </summary>
    public class IAPManager : MonoBehaviour
#if UNITY_PURCHASING
        , IStoreListener
#endif
    {
        public static IAPManager Instance { get; private set; }

        // ── Ürün ID'leri — Google Play Console ile eşleşmeli ──────────────────
        public const string GEMS_100     = "gems_100";      // Tüketilebilir — $0.99
        public const string GEMS_600     = "gems_600";      // Tüketilebilir — $4.99
        public const string STARTER_PACK = "starter_pack";  // Tek seferlik  — $2.99

        // Ürün başına elmas miktarı
        private static readonly Dictionary<string, int> GemRewards = new()
        {
            { GEMS_100,     100 },
            { GEMS_600,     600 },
            { STARTER_PACK, 500 },
        };

        // ── Eventler ───────────────────────────────────────────────────────────
        /// <summary>Satın alma başarılı — EconomyManager bu eventi dinler.</summary>
        public event Action<string, int> OnGemsGranted;  // (productId, gemsAmount)
        public event Action<string>      OnPurchaseFailed;
        public event Action              OnStoreReady;
        public event Action<string>      OnStoreInitFailed;

        public bool IsInitialized { get; private set; }

#if UNITY_PURCHASING
        private IStoreController   _controller;
        private IExtensionProvider _extensions;
#endif

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
#if UNITY_PURCHASING
            InitStore();
#else
            Debug.LogWarning("[IAPManager] com.unity.purchasing paketi yüklü değil — stub modunda çalışıyor.");
#endif
        }

        // ── Başlatma ───────────────────────────────────────────────────────────

#if UNITY_PURCHASING
        private void InitStore()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(GEMS_100,     ProductType.Consumable);
            builder.AddProduct(GEMS_600,     ProductType.Consumable);
            builder.AddProduct(STARTER_PACK, ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder);
        }
#endif

        // ── Satın alma ─────────────────────────────────────────────────────────

        /// <summary>UI butonlarından çağrılır.</summary>
        public void BuyProduct(string productId)
        {
#if UNITY_PURCHASING
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAPManager] Mağaza henüz hazır değil.");
                return;
            }
            var product = _controller.products.WithID(productId);
            if (product != null && product.availableToPurchase)
                _controller.InitiatePurchase(product);
            else
                Debug.LogWarning($"[IAPManager] Ürün bulunamadı veya satın alınamaz: {productId}");
#else
            // Stub: geliştirme sırasında ödül doğrudan verilir
            SimulatePurchase(productId);
#endif
        }

        // ── IStoreListener ─────────────────────────────────────────────────────

#if UNITY_PURCHASING
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller  = controller;
            _extensions  = extensions;
            IsInitialized = true;
            Debug.Log("[IAPManager] Mağaza başlatıldı.");
            OnStoreReady?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
            => OnInitializeFailed(error, null);

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"[IAPManager] Başlatma başarısız: {error} — {message}");
            OnStoreInitFailed?.Invoke(error.ToString());
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string id = args.purchasedProduct.definition.id;
            GrantReward(id);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.LogWarning($"[IAPManager] Satın alma başarısız: {product.definition.id} — {reason}");
            OnPurchaseFailed?.Invoke(product.definition.id);
        }
#endif

        // ── Ödül verme ─────────────────────────────────────────────────────────

        private void GrantReward(string productId)
        {
            if (GemRewards.TryGetValue(productId, out int gems))
            {
                Debug.Log($"[IAPManager] {gems} elmas verildi ({productId}).");
                OnGemsGranted?.Invoke(productId, gems);
            }
        }

        // Stub modunda editörde test için
        private void SimulatePurchase(string productId)
        {
            Debug.Log($"[IAPManager] STUB satın alma simüle edildi: {productId}");
            GrantReward(productId);
        }
    }
}
