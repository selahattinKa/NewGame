using System;
using System.Collections;
using UnityEngine;

namespace CanavarZindanlari.Economy
{
    /// <summary>
    /// Rewarded reklam yöneticisi.
    /// Stub modu: Google Mobile Ads paketi olmadan çalışır, test edilebilir.
    /// Gerçek AdMob için: ShowRewardedAd_Real() içini doldur, ShowRewardedAd()'dan çağır.
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        // ── Sabitler ───────────────────────────────────────────────────────────
        public const int  RewardGems      = 30;   // izleme başına elmas
        public const int  DailyAdLimit    = 10;   // günlük maksimum reklam
        public const float CooldownSeconds = 60f; // reklamlar arası bekleme

        // Test Ad Unit ID'leri — gerçek ID'lerle değiştir (AdMob Console)
        public const string AppIdAndroid      = "ca-app-pub-3940256099942544~3347511713";
        public const string RewardedAdAndroid = "ca-app-pub-3940256099942544/5224354917";
        public const string AppIdIos          = "ca-app-pub-3940256099942544~1458002511";
        public const string RewardedAdIos     = "ca-app-pub-3940256099942544/1712485313";

        // ── Eventler ───────────────────────────────────────────────────────────
        /// <summary>Reklam başarıyla izlendi — EconomyManager dinler.</summary>
        public event Action<int> RewardGranted; // (gemAmount)
        public event Action      AdFailed;
        public event Action      AdLimitReached;

        // ── Durum ──────────────────────────────────────────────────────────────
        public bool IsAdReady     { get; private set; } = true; // stub: her zaman hazır
        public int  AdsWatchedToday => GetTodayCount();
        public bool CanShowAd => AdsWatchedToday < DailyAdLimit && !_onCooldown;

        private bool  _onCooldown;
        private float _cooldownRemaining;

        private const string PrefKeyPrefix = "ad_count_";

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!_onCooldown) return;
            _cooldownRemaining -= Time.deltaTime;
            if (_cooldownRemaining <= 0f)
                _onCooldown = false;
        }

        // ── Ana metot — UI'dan çağrılır ────────────────────────────────────────

        public void ShowRewardedAd()
        {
            if (!CanShowAd)
            {
                if (AdsWatchedToday >= DailyAdLimit)
                {
                    Debug.Log("[AdManager] Günlük reklam limiti doldu.");
                    AdLimitReached?.Invoke();
                }
                else
                {
                    Debug.Log($"[AdManager] Bekleme süresi devam ediyor: {_cooldownRemaining:F0}s");
                    AdFailed?.Invoke();
                }
                return;
            }

            // Gerçek AdMob paketi kurulunca burayı değiştir:
            // ShowRewardedAd_Real();
            StartCoroutine(StubAd());
        }

        // ── Stub uygulama ──────────────────────────────────────────────────────

        private IEnumerator StubAd()
        {
            IsAdReady = false;
            Debug.Log("[AdManager] STUB: Reklam gösteriliyor...");

            // Gerçek reklam süresini simüle eder
            yield return new WaitForSeconds(2f);

            RecordAdWatched();
            StartCooldown();
            IsAdReady = true;

            Debug.Log($"[AdManager] STUB: Tamamlandı — {RewardGems} elmas verildi. " +
                      $"Bugün: {AdsWatchedToday}/{DailyAdLimit}");
            RewardGranted?.Invoke(RewardGems);
        }

        // ── Gerçek AdMob buraya gelecek ────────────────────────────────────────
        // private void ShowRewardedAd_Real()
        // {
        //     // Google Mobile Ads paketi kurulduktan sonra doldur:
        //     // using GoogleMobileAds.Api;
        //     // _rewardedAd.Show(reward => {
        //     //     RecordAdWatched();
        //     //     StartCooldown();
        //     //     RewardGranted?.Invoke(RewardGems);
        //     // });
        // }

        // ── Yardımcılar ────────────────────────────────────────────────────────

        private void RecordAdWatched()
        {
            string key = PrefKeyPrefix + TodayKey();
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, 0) + 1);
            PlayerPrefs.Save();
        }

        private void StartCooldown()
        {
            _onCooldown        = true;
            _cooldownRemaining = CooldownSeconds;
        }

        private int GetTodayCount()
        {
            return PlayerPrefs.GetInt(PrefKeyPrefix + TodayKey(), 0);
        }

        private static string TodayKey()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        /// <summary>Debug için — editörde günlük sayacı sıfırla.</summary>
        [ContextMenu("Günlük Sayacı Sıfırla")]
        private void ResetDailyCount()
        {
            PlayerPrefs.DeleteKey(PrefKeyPrefix + TodayKey());
            Debug.Log("[AdManager] Günlük sayaç sıfırlandı.");
        }
    }
}
