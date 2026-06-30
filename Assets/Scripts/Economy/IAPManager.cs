// Unity IAP v5: eski v4 compat API kullanılıyor — MVP öncesi yeni API'ye geçilecek
#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace CanavarZindanlari.Economy
{
    /// <summary>
    /// Google Play Billing / Apple StoreKit sarmalayıcı (Unity IAP 5.x).
    /// Maps to: design/gdd/iap-reklam-sistemi (Not Started — prototype stub)
    /// </summary>
    public class IAPManager : MonoBehaviour, IStoreListener
    {
        public static IAPManager Instance { get; private set; }

        // ── Ürün ID'leri — Google Play Console ile eşleşmeli ──────────────────
        public const string GEMS_100     = "gems_100";      // Tüketilebilir — $0.99
        public const string GEMS_600     = "gems_600";      // Tüketilebilir — $4.99
        public const string STARTER_PACK = "starter_pack";  // Tek seferlik  — $2.99

        private static readonly Dictionary<string, int> GemRewards = new()
        {
            { GEMS_100,     100 },
            { GEMS_600,     600 },
            { STARTER_PACK, 500 },
        };

        // ── Eventler ───────────────────────────────────────────────────────────
        /// <summary>Satın alma başarılı — EconomyManager bu eventi dinler.</summary>
        public event Action<string, int> GemsGranted;    // (productId, gemsAmount)
        public event Action<string>      PurchaseFailed; // (productId)
        public event Action              StoreReady;
        public event Action<string>      StoreInitFailed;

        public bool IsInitialized { get; private set; }

        private IStoreController   _controller;
        private IExtensionProvider _extensions;

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() => InitStore();

        // ── Başlatma ───────────────────────────────────────────────────────────

        private void InitStore()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(GEMS_100,     ProductType.Consumable);
            builder.AddProduct(GEMS_600,     ProductType.Consumable);
            builder.AddProduct(STARTER_PACK, ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder);
        }

        // ── Satın alma ─────────────────────────────────────────────────────────

        /// <summary>UI butonlarından çağrılır.</summary>
        public void BuyProduct(string productId)
        {
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
        }

        // ── IStoreListener ─────────────────────────────────────────────────────

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller   = controller;
            _extensions   = extensions;
            IsInitialized = true;
            Debug.Log("[IAPManager] Mağaza başlatıldı.");
            StoreReady?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
            => OnInitializeFailed(error, null);

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"[IAPManager] Başlatma başarısız: {error} — {message}");
            StoreInitFailed?.Invoke(error.ToString());
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            GrantReward(args.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.LogWarning($"[IAPManager] Satın alma başarısız: {product.definition.id} — {reason}");
            PurchaseFailed?.Invoke(product.definition.id);
        }

        // ── Ödül verme ─────────────────────────────────────────────────────────

        private void GrantReward(string productId)
        {
            if (GemRewards.TryGetValue(productId, out int gems))
            {
                Debug.Log($"[IAPManager] {gems} elmas verildi ({productId}).");
                GemsGranted?.Invoke(productId, gems);
            }
        }
    }
}
