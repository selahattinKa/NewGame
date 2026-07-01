using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;
using CanavarZindanlari.Backend;
using CanavarZindanlari.Core;

namespace CanavarZindanlari.Arena
{
    /// <summary>
    /// Arena akışını yönetir: giriş → profil → eşleştirme → savaş simülasyonu → sonuç.
    /// FirebaseAuthManager ile aynı GameObject'te bulunur.
    /// </summary>
    public class ArenaManager : MonoBehaviour
    {
        public static ArenaManager Instance { get; private set; }

        // ── Durum makinesi ───────────────────────────────────────────────────

        public enum ArenaState { Idle, SigningIn, LoadingProfile, Matchmaking, BattleResult }
        public ArenaState State { get; private set; } = ArenaState.Idle;

        // ── Veri ────────────────────────────────────────────────────────────

        public ArenaProfile     MyProfile    { get; private set; }
        public ArenaProfile     LastOpponent { get; private set; }
        public bool             LastMatchWon { get; private set; }
        public string           StatusMessage { get; private set; } = "";

        public event Action OnStateChanged;

        // ── Lifecycle ────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            var auth = FirebaseAuthManager.Instance;
            if (auth == null) return;

            auth.OnSignInSuccess += HandleSignIn;
            auth.OnSignInFailed  += msg => { StatusMessage = $"Giriş hatası: {msg}"; SetState(ArenaState.Idle); };
        }

        private void OnDestroy()
        {
            if (FirebaseAuthManager.Instance != null)
            {
                FirebaseAuthManager.Instance.OnSignInSuccess -= HandleSignIn;
            }
        }

        // ── Giriş akışı ──────────────────────────────────────────────────────

        public void StartSignIn()
        {
            if (State != ArenaState.Idle) return;
            SetState(ArenaState.SigningIn);
            StatusMessage = "Google ile giriş yapılıyor...";
            FirebaseAuthManager.Instance.SignIn();
        }

        private async void HandleSignIn(FirebaseUser user)
        {
            SetState(ArenaState.LoadingProfile);
            StatusMessage = "Profil yükleniyor...";

            var collection = FindFirstObjectOfType<MonsterCollection>();
            int cp = PlayerProfileService.CalculateCombatPower(collection);

            try
            {
                string name = string.IsNullOrEmpty(user.DisplayName) ? $"Oyuncu_{user.UserId[..6]}" : user.DisplayName;
                MyProfile = await PlayerProfileService.LoadOrCreate(user.UserId, name, cp);
                await PlayerProfileService.UpdateCombatPower(user.UserId, cp);
                MyProfile.CombatPower = cp;

                StatusMessage = "Profil hazır.";
                SetState(ArenaState.Idle);
            }
            catch (Exception e)
            {
                StatusMessage = $"Profil yüklenemedi: {e.Message}";
                SetState(ArenaState.Idle);
            }
        }

        // ── Maç arama ────────────────────────────────────────────────────────

        public async void FindMatch()
        {
            if (MyProfile == null || State != ArenaState.Idle) return;

            SetState(ArenaState.Matchmaking);
            StatusMessage = "Rakip aranıyor...";

            try
            {
                LastOpponent = await PlayerProfileService.FindOpponent(MyProfile);

                if (LastOpponent == null)
                {
                    // Rakip bulunamadı — bot oluştur
                    LastOpponent = BuildBot(MyProfile);
                    StatusMessage = $"Bot rakip: {LastOpponent.DisplayName}";
                }
                else
                {
                    StatusMessage = $"Rakip bulundu: {LastOpponent.DisplayName}";
                }

                await Task.Delay(800); // kısa bekleme (görsel efekt için)
                await SimulateBattle();
            }
            catch (Exception e)
            {
                StatusMessage = $"Eşleştirme hatası: {e.Message}";
                SetState(ArenaState.Idle);
            }
        }

        // ── Savaş simülasyonu ────────────────────────────────────────────────

        private async Task SimulateBattle()
        {
            // CP farkına göre kazanma olasılığı (±30 CP = nötr %50)
            float ratio    = (float)MyProfile.CombatPower / Mathf.Max(1, LastOpponent.CombatPower);
            float winChance = Mathf.Clamp(0.25f + (ratio - 1f) * 0.5f, 0.1f, 0.9f);
            LastMatchWon    = UnityEngine.Random.value < winChance;

            MyProfile = await PlayerProfileService.ApplyMatchResult(MyProfile, LastMatchWon);

            StatusMessage = LastMatchWon
                ? $"Zafer! +25 puan → {MyProfile.ArenaPoints}"
                : $"Yenilgi. -15 puan → {MyProfile.ArenaPoints}";

            SetState(ArenaState.BattleResult);
        }

        public void BackToIdle()
        {
            if (State == ArenaState.BattleResult)
                SetState(ArenaState.Idle);
        }

        // ── Yardımcılar ──────────────────────────────────────────────────────

        private static ArenaProfile BuildBot(ArenaProfile player)
        {
            int cpVariance = UnityEngine.Random.Range(-20, 21);
            return new ArenaProfile
            {
                Uid         = "bot",
                DisplayName = BotName(),
                CombatPower = Mathf.Max(50, player.CombatPower + cpVariance),
                ArenaPoints = player.ArenaPoints,
                Wins        = UnityEngine.Random.Range(0, 50),
                Losses      = UnityEngine.Random.Range(0, 30),
            };
        }

        private static readonly string[] BotNames =
        {
            "Ejder Avcısı", "Gölge Savaşçı", "Ateş Efendisi",
            "Buz Kalkanı",  "Fırtına Lordu",  "Karanlık Şövalye",
        };

        private static string BotName() =>
            BotNames[UnityEngine.Random.Range(0, BotNames.Length)] + " [BOT]";

        private void SetState(ArenaState s)
        {
            State = s;
            OnStateChanged?.Invoke();
        }

        // Unity 6 API
        private static T FindFirstObjectOfType<T>() where T : UnityEngine.Object =>
            UnityEngine.Object.FindFirstObjectByType<T>();
    }
}
