using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;
using CanavarZindanlari.Equipment;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Mağaza ekranı — gacha kutuları ve iksir satışı.
    /// Hub'dan açılır, X ile Hub'a döner.
    /// </summary>
    public class ShopHUD : MonoBehaviour
    {
        private static readonly Color ColBg      = new Color(0.05f, 0.04f, 0.10f, 1.00f);
        private static readonly Color ColGold    = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColDiamond = new Color(0.40f, 0.80f, 1.00f);
        private static readonly Color ColCard    = new Color(0.12f, 0.11f, 0.20f, 1.00f);
        private static readonly Color ColSection = new Color(0.18f, 0.16f, 0.28f, 1.00f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleSmall;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnX;
        private bool     _stylesReady;

        private MonsterCollection _collection;

        // Pity sayaçları
        private int _pityGold;
        private int _pityDiamond;
        private const int PityLimit = 10;
        private const string KeyPityGold    = "gacha_pity_gold";
        private const string KeyPityDiamond = "gacha_pity_diamond";

        // Çekiş sonucu
        private string _pullResult;
        private float  _pullResultExpiry;
        private Color  _pullResultColor;

        private void Awake()
        {
            _collection   = UnityEngine.Object.FindFirstObjectByType<MonsterCollection>();
            _pityGold     = PlayerPrefs.GetInt(KeyPityGold,    0);
            _pityDiamond  = PlayerPrefs.GetInt(KeyPityDiamond, 0);
        }

        private void OnGUI()
        {
            if (ScreenNavigator.Current != GameScreen.Shop) return;
            BuildStyles();

            float sw  = Screen.width;
            float sh  = Screen.height;
            float pad = sw * 0.05f;
            float w   = sw - pad * 2f;

            GUI.color = ColBg;
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            DrawCloseBtn(sw, sh);

            float y = sh * 0.05f;

            // Başlık
            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w * 0.80f, sh * 0.07f), "🛒 MAĞAZA", _styleTitle);
            y += sh * 0.08f;

            // Kaynak satırı
            var eco = EconomyManager.Instance;
            int gold    = eco?.Gold    ?? 0;
            int diamond = eco?.Diamond ?? 0;
            int pots    = eco?.PotCount ?? 0;
            _styleSmall.normal.textColor = Color.white;
            GUI.Label(new Rect(pad, y, w, sh * 0.04f),
                $"🪙 {gold:N0}   💎 {diamond:N0}   🧪 {pots}/5", _styleSmall);
            y += sh * 0.05f;

            // ── Gacha kutuları ────────────────────────────────────────────────
            DrawSectionHeader(pad, y, w, "GACHA KUTULARI", sh);
            y += sh * 0.05f;

            float halfW = (w - sw * 0.03f) * 0.5f;
            float cardH = sh * 0.22f;

            // Altın Kutu
            DrawGachaCard(pad, y, halfW, cardH, sh,
                "🪙 ALTIN KUTU",
                "F / D / C / B tier",
                "10'da bir C+ garantili",
                _pityGold, ColGold,
                "1.000 🪙 Çek",
                () => PullGold());

            // Elmas Kutu
            DrawGachaCard(pad + halfW + sw * 0.03f, y, halfW, cardH, sh,
                "💎 ELMAS KUTU",
                "D / C / B / A tier",
                "10'da bir B+ garantili",
                _pityDiamond, ColDiamond,
                "50 💎 Çek",
                () => PullDiamond());

            y += cardH + sh * 0.03f;

            // ── Çekiş sonucu ──────────────────────────────────────────────────
            if (!string.IsNullOrEmpty(_pullResult) && Time.realtimeSinceStartup < _pullResultExpiry)
            {
                GUI.color = new Color(0.12f, 0.10f, 0.20f, 1f);
                GUI.DrawTexture(new Rect(pad, y, w, sh * 0.06f), Texture2D.whiteTexture);
                GUI.color = Color.white;
                _styleLabel.normal.textColor = _pullResultColor;
                GUI.Label(new Rect(pad + 8f, y + 4f, w - 8f, sh * 0.055f), _pullResult, _styleLabel);
                y += sh * 0.075f;
            }
            else
            {
                y += sh * 0.01f;
            }

            // ── İksir ─────────────────────────────────────────────────────────
            DrawSectionHeader(pad, y, w, "İKSİR", sh);
            y += sh * 0.05f;

            GUI.color = ColCard;
            GUI.DrawTexture(new Rect(pad, y, w, sh * 0.13f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            _styleSmall.normal.textColor = Color.gray;
            GUI.Label(new Rect(pad + 8f, y + sh * 0.008f, w * 0.65f, sh * 0.04f),
                "HP %30 altında otomatik kullanılır · +%40 HP · 3 tur bekleme", _styleSmall);

            _styleSmall.normal.textColor = pots >= 5 ? Color.gray : Color.white;
            GUI.Label(new Rect(pad + 8f, y + sh * 0.05f, w * 0.50f, sh * 0.04f),
                pots >= 5 ? "Doldu (5/5)" : $"Mevcut: {pots}/5", _styleSmall);

            bool canBuyPot = pots < 5 && (eco?.Gold ?? 0) >= 500;
            GUI.enabled = canBuyPot;
            GUI.color   = canBuyPot ? new Color(0.30f, 0.75f, 0.45f) : new Color(0.35f, 0.35f, 0.40f);
            if (GUI.Button(new Rect(pad + w * 0.55f, y + sh * 0.018f, w * 0.42f, sh * 0.09f),
                "500 🪙 — 1 İksir Al", _styleBtn))
            {
                if (eco != null && eco.SpendGold(500))
                {
                    eco.AddPots(1);
                    ShowResult("✓ 1 İksir satın alındı!", ColGold);
                }
            }
            GUI.enabled = true;
            GUI.color   = Color.white;
        }

        // ── Gacha kart çizimi ─────────────────────────────────────────────────

        private void DrawGachaCard(float x, float y, float w, float h, float sh,
            string title, string tierLine, string pityLine, int pityCount,
            Color titleColor, string btnLabel, System.Action onPull)
        {
            GUI.color = ColCard;
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float ip = w * 0.05f;
            float lh = sh * 0.038f;

            _styleLabel.normal.textColor = titleColor;
            GUI.Label(new Rect(x + ip, y + ip, w - ip * 2, lh), title, _styleLabel);

            _styleSmall.normal.textColor = Color.white;
            GUI.Label(new Rect(x + ip, y + ip + lh * 1.1f, w - ip * 2, lh), tierLine, _styleSmall);

            _styleSmall.normal.textColor = Color.gray;
            GUI.Label(new Rect(x + ip, y + ip + lh * 2.1f, w - ip * 2, lh), pityLine, _styleSmall);

            // Pity göstergesi
            _styleSmall.normal.textColor = pityCount >= PityLimit - 1
                ? new Color(1f, 0.5f, 0.2f)
                : new Color(0.6f, 0.6f, 0.8f);
            GUI.Label(new Rect(x + ip, y + ip + lh * 3.1f, w - ip * 2, lh),
                $"Pity: {pityCount}/{PityLimit}", _styleSmall);

            GUI.color = titleColor;
            if (GUI.Button(new Rect(x + ip, y + h - sh * 0.085f, w - ip * 2, sh * 0.075f),
                btnLabel, _styleBtn))
                onPull?.Invoke();
            GUI.color = Color.white;
        }

        private void DrawSectionHeader(float x, float y, float w, string text, float sh)
        {
            GUI.color = ColSection;
            GUI.DrawTexture(new Rect(x, y, w, sh * 0.038f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            _styleSmall.normal.textColor = new Color(0.7f, 0.7f, 0.9f);
            GUI.Label(new Rect(x + 8f, y + 2f, w - 8f, sh * 0.036f), text, _styleSmall);
        }

        // ── Gacha mantığı ─────────────────────────────────────────────────────

        private void PullGold()
        {
            var eco = EconomyManager.Instance;
            if (eco == null || !eco.SpendGold(1000)) { ShowResult("✗ Yeterli altın yok!", Color.red); return; }

            _pityGold++;
            Rarity tier;

            if (_pityGold >= PityLimit)
            {
                tier = UnityEngine.Random.value < 0.8f ? Rarity.C : Rarity.B;
                _pityGold = 0;
            }
            else
            {
                float r = UnityEngine.Random.value;
                tier = r < 0.55f ? Rarity.F : r < 0.85f ? Rarity.D : r < 0.97f ? Rarity.C : Rarity.B;
                if (tier >= Rarity.C) _pityGold = 0;
            }

            PlayerPrefs.SetInt(KeyPityGold, _pityGold);
            PlayerPrefs.Save();
            FinishPull(tier, goldBox: true);
        }

        private void PullDiamond()
        {
            var eco = EconomyManager.Instance;
            if (eco == null || !eco.SpendDiamonds(50)) { ShowResult("✗ Yeterli elmas yok!", Color.red); return; }

            _pityDiamond++;
            Rarity tier;

            if (_pityDiamond >= PityLimit)
            {
                tier = UnityEngine.Random.value < 0.8f ? Rarity.B : Rarity.A;
                _pityDiamond = 0;
            }
            else
            {
                float r = UnityEngine.Random.value;
                tier = r < 0.40f ? Rarity.D : r < 0.75f ? Rarity.C : r < 0.95f ? Rarity.B : Rarity.A;
                if (tier >= Rarity.B) _pityDiamond = 0;
            }

            PlayerPrefs.SetInt(KeyPityDiamond, _pityDiamond);
            PlayerPrefs.Save();
            FinishPull(tier, goldBox: false);
        }

        private void FinishPull(Rarity tier, bool goldBox)
        {
            Color col = tier switch
            {
                Rarity.SS => new Color(1.00f, 0.55f, 0.10f),
                Rarity.S  => new Color(0.80f, 0.30f, 1.00f),
                Rarity.A  => new Color(0.40f, 0.75f, 1.00f),
                Rarity.B  => new Color(0.30f, 0.90f, 0.50f),
                Rarity.C  => new Color(0.55f, 0.90f, 0.40f),
                Rarity.D  => new Color(0.95f, 0.80f, 0.30f),
                _         => new Color(0.80f, 0.78f, 0.80f),
            };

            // Ekipman şansı: altın kutu %35 zırh, elmas kutu %30 zırh + %20 takı
            float eqRoll = UnityEngine.Random.value;
            float armorChance     = goldBox ? 0.35f : 0.30f;
            float accessoryChance = goldBox ? 0.00f : 0.20f;

            if (eqRoll < armorChance)
            {
                var slot = EquipmentManager.RandomArmorSlot();
                var eq   = EquipmentManager.CreateEquipment(slot, tier);
                EquipmentManager.Instance.AddEquipment(eq);
                ShowResult($"⚔ [{tier}] {eq.DisplayName}  ({StatsShort(eq)}) düştü!", col);
                return;
            }
            if (eqRoll < armorChance + accessoryChance)
            {
                var slot = EquipmentManager.RandomAccessorySlot();
                var eq   = EquipmentManager.CreateEquipment(slot, tier);
                EquipmentManager.Instance.AddEquipment(eq);
                ShowResult($"💍 [{tier}] {eq.DisplayName}  ({StatsShort(eq)}) düştü!", col);
                return;
            }

            // Pet
            if (_collection == null) { ShowResult("✗ Koleksiyon bulunamadı!", Color.red); return; }
            var pet = _collection.PullFromGacha(tier);
            ShowResult($"✓ [{tier}] {pet.DisplayName} yakalandı!  (Max: {pet.MaxEvolutionTier})", col);
        }

        private static string StatsShort(OwnedEquipment eq)
        {
            var sb = new System.Text.StringBuilder();
            if (eq.BonusATK > 0) sb.Append($"+{eq.BonusATK}ATK ");
            if (eq.BonusHP  > 0) sb.Append($"+{eq.BonusHP}HP ");
            if (eq.BonusDEF > 0) sb.Append($"+{eq.BonusDEF}DEF ");
            if (eq.BonusSPD > 0) sb.Append($"+{eq.BonusSPD}HIZ");
            return sb.ToString().Trim();
        }

        private void ShowResult(string msg, Color col)
        {
            _pullResult       = msg;
            _pullResultColor  = col;
            _pullResultExpiry = Time.realtimeSinceStartup + 4f;
        }

        // ── Yardımcılar ───────────────────────────────────────────────────────

        private void DrawCloseBtn(float sw, float sh)
        {
            float bw = sw * 0.12f;
            float bh = sh * 0.055f;
            GUI.color = new Color(0.70f, 0.20f, 0.20f);
            if (GUI.Button(new Rect(sw - bw - 8f, 8f, bw, bh), "✕", _styleBtnX))
                ScreenNavigator.GoToHub();
            GUI.color = Color.white;
        }

        private void BuildStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            int fs = Mathf.Max(14, Screen.width / 22);

            _styleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs + 6,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };

            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };

            _styleSmall = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs - 2,
                alignment = TextAnchor.MiddleLeft,
            };

            _styleBtn = new GUIStyle(GUI.skin.button)
            {
                fontSize  = fs - 1,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
            };

            _styleBtnX = new GUIStyle(GUI.skin.button)
            {
                fontSize  = fs + 2,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
            };
        }
    }
}
