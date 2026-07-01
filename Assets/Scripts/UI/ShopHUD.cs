using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;
using CanavarZindanlari.Equipment;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Mağaza ekranı — ayrı pet ve ekipman gacha kutuları, iksir satışı.
    /// </summary>
    public class ShopHUD : MonoBehaviour
    {
        private static readonly Color ColBg      = new Color(0.05f, 0.04f, 0.10f, 1.00f);
        private static readonly Color ColGold    = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColDiamond = new Color(0.40f, 0.80f, 1.00f);
        private static readonly Color ColCard    = new Color(0.12f, 0.11f, 0.20f, 1.00f);
        private static readonly Color ColSection = new Color(0.18f, 0.16f, 0.28f, 1.00f);
        private static readonly Color ColGreen   = new Color(0.25f, 0.72f, 0.40f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleSmall;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnX;
        private bool     _stylesReady;

        private MonsterCollection _collection;

        // Pity sayaçları — pet ve ekipman ayrı
        private int _pityPetGold;
        private int _pityPetDiamond;
        private int _pityEqGold;
        private int _pityEqDiamond;
        private const int PityLimit = 10;

        private const string KeyPityPetGold    = "gacha_pity_pet_gold";
        private const string KeyPityPetDiamond = "gacha_pity_pet_diamond";
        private const string KeyPityEqGold     = "gacha_pity_eq_gold";
        private const string KeyPityEqDiamond  = "gacha_pity_eq_diamond";

        // Çekiş sonucu
        private string _pullResult;
        private float  _pullResultExpiry;
        private Color  _pullResultColor;

        // Scroll
        private Vector2 _scroll;

        private void Awake()
        {
            _collection    = UnityEngine.Object.FindFirstObjectByType<MonsterCollection>();
            _pityPetGold   = PlayerPrefs.GetInt(KeyPityPetGold,   0);
            _pityPetDiamond= PlayerPrefs.GetInt(KeyPityPetDiamond,0);
            _pityEqGold    = PlayerPrefs.GetInt(KeyPityEqGold,    0);
            _pityEqDiamond = PlayerPrefs.GetInt(KeyPityEqDiamond, 0);
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

            float y = sh * 0.04f;
            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w * 0.80f, sh * 0.07f), "🛒 MAĞAZA", _styleTitle);
            y += sh * 0.07f;

            // Kaynak satırı
            var eco = EconomyManager.Instance;
            _styleSmall.normal.textColor = Color.white;
            GUI.Label(new Rect(pad, y, w, sh * 0.038f),
                $"🪙 {(eco?.Gold ?? 0):N0}   💎 {(eco?.Diamond ?? 0):N0}   🧪 {(eco?.PotCount ?? 0)}/5",
                _styleSmall);
            y += sh * 0.048f;

            // Çekiş sonucu
            if (!string.IsNullOrEmpty(_pullResult) && Time.realtimeSinceStartup < _pullResultExpiry)
            {
                GUI.color = new Color(0.12f, 0.10f, 0.20f, 1f);
                GUI.DrawTexture(new Rect(pad, y, w, sh * 0.055f), Texture2D.whiteTexture);
                GUI.color = Color.white;
                _styleSmall.normal.textColor = _pullResultColor;
                GUI.Label(new Rect(pad + 8f, y + 4f, w - 8f, sh * 0.050f), _pullResult, _styleSmall);
                y += sh * 0.065f;
            }
            else y += sh * 0.005f;

            // ── Scroll ────────────────────────────────────────────────────────
            float cardH  = sh * 0.195f;
            float halfW  = (w - sw * 0.025f) * 0.5f;
            float secH   = sh * 0.042f;
            float potH   = sh * 0.130f;
            float totalH = (secH + cardH + sh * 0.020f) * 2 + secH + potH + sh * 0.010f;

            float viewH = sh - y - sh * 0.02f;
            _scroll = GUI.BeginScrollView(
                new Rect(pad, y, w, viewH), _scroll,
                new Rect(0, 0, w - 16f, totalH), false, false);

            float ry = 0f;

            // ── Pet Kutuları ─────────────────────────────────────────────────
            DrawSectionHeader(0, ry, w - 16f, "🐾 PET KUTULARI", sh);
            ry += secH;

            DrawGachaCard(0, ry, halfW, cardH, sh,
                "🪙 Altın Pet Kutusu", "F / D / C / B tier", "10'da bir C+ garantili",
                _pityPetGold, ColGold, "1.000 🪙 Çek", PullPetGold);

            DrawGachaCard(halfW + sw * 0.025f, ry, halfW, cardH, sh,
                "💎 Elmas Pet Kutusu", "D / C / B / A tier", "10'da bir B+ garantili",
                _pityPetDiamond, ColDiamond, "50 💎 Çek", PullPetDiamond);

            ry += cardH + sh * 0.020f;

            // ── Ekipman Kutuları ──────────────────────────────────────────────
            DrawSectionHeader(0, ry, w - 16f, "⚔ EKİPMAN KUTULARI", sh);
            ry += secH;

            DrawGachaCard(0, ry, halfW, cardH, sh,
                "🪙 Altın Ekipman Kutusu", "F / D / C / B tier", "10'da bir C+ garantili",
                _pityEqGold, ColGold, "1.000 🪙 Çek", PullEqGold);

            DrawGachaCard(halfW + sw * 0.025f, ry, halfW, cardH, sh,
                "💎 Elmas Ekipman Kutusu", "D / C / B / A tier", "10'da bir B+ garantili",
                _pityEqDiamond, ColDiamond, "50 💎 Çek", PullEqDiamond);

            ry += cardH + sh * 0.020f;

            // ── İksir ─────────────────────────────────────────────────────────
            DrawSectionHeader(0, ry, w - 16f, "🧪 İKSİR", sh);
            ry += secH;
            DrawPotSection(sw, sh, w, ry, eco);

            GUI.EndScrollView();
        }

        // ── Kart çizimi ───────────────────────────────────────────────────────

        private void DrawGachaCard(float x, float y, float w, float h, float sh,
            string title, string tierLine, string pityLine,
            int pityCount, Color titleColor, string btnLabel, System.Action onPull)
        {
            GUI.color = ColCard;
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float ip = w * 0.05f;
            float lh = sh * 0.034f;

            _styleSmall.normal.textColor = titleColor;
            GUI.Label(new Rect(x + ip, y + ip, w - ip * 2f, lh * 1.1f), title, _styleSmall);

            _styleSmall.normal.textColor = Color.white;
            GUI.Label(new Rect(x + ip, y + ip + lh * 1.2f, w - ip * 2f, lh), tierLine, _styleSmall);

            _styleSmall.normal.textColor = Color.gray;
            GUI.Label(new Rect(x + ip, y + ip + lh * 2.2f, w - ip * 2f, lh), pityLine, _styleSmall);

            _styleSmall.normal.textColor = pityCount >= PityLimit - 1
                ? new Color(1f, 0.5f, 0.2f) : new Color(0.6f, 0.6f, 0.8f);
            GUI.Label(new Rect(x + ip, y + ip + lh * 3.2f, w - ip * 2f, lh),
                $"Pity: {pityCount}/{PityLimit}", _styleSmall);

            GUI.color = titleColor;
            float bh = h * 0.28f;
            if (GUI.Button(new Rect(x + ip, y + h - bh - ip, w - ip * 2f, bh), btnLabel, _styleBtn))
                onPull?.Invoke();
            GUI.color = Color.white;
        }

        private void DrawPotSection(float sw, float sh, float w, float ry, EconomyManager eco)
        {
            GUI.color = ColCard;
            GUI.DrawTexture(new Rect(0, ry, w - 16f, sh * 0.120f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int pots = eco?.PotCount ?? 0;
            _styleSmall.normal.textColor = Color.gray;
            GUI.Label(new Rect(8f, ry + sh * 0.008f, (w - 16f) * 0.62f, sh * 0.038f),
                "HP %30 altında otomatik · +%40 HP · 3 tur bekleme", _styleSmall);
            _styleSmall.normal.textColor = pots >= 5 ? Color.gray : Color.white;
            GUI.Label(new Rect(8f, ry + sh * 0.048f, (w - 16f) * 0.50f, sh * 0.038f),
                pots >= 5 ? "Doldu (5/5)" : $"Mevcut: {pots}/5", _styleSmall);

            bool canBuy = pots < 5 && (eco?.Gold ?? 0) >= 500;
            GUI.enabled = canBuy;
            GUI.color   = canBuy ? ColGreen : new Color(0.35f, 0.35f, 0.40f);
            float bw = (w - 16f) * 0.36f;
            float bx = (w - 16f) - bw - 8f;
            if (GUI.Button(new Rect(bx, ry + sh * 0.018f, bw, sh * 0.085f), "500 🪙\n1 İksir", _styleBtn))
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

        private void DrawSectionHeader(float x, float y, float w, string text, float sh)
        {
            GUI.color = ColSection;
            GUI.DrawTexture(new Rect(x, y, w, sh * 0.036f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            _styleSmall.normal.textColor = new Color(0.7f, 0.7f, 0.9f);
            GUI.Label(new Rect(x + 8f, y + 2f, w - 8f, sh * 0.034f), text, _styleSmall);
        }

        // ── Pet gacha mantığı ──────────────────────────────────────────────────

        private void PullPetGold()
        {
            var eco = EconomyManager.Instance;
            if (eco == null || !eco.SpendGold(1000)) { ShowResult("✗ Yeterli altın yok!", Color.red); return; }

            _pityPetGold++;
            Rarity tier;
            if (_pityPetGold >= PityLimit)
            {
                tier = UnityEngine.Random.value < 0.8f ? Rarity.C : Rarity.B;
                _pityPetGold = 0;
            }
            else
            {
                float r = UnityEngine.Random.value;
                tier = r < 0.55f ? Rarity.F : r < 0.85f ? Rarity.D : r < 0.97f ? Rarity.C : Rarity.B;
                if (tier >= Rarity.C) _pityPetGold = 0;
            }
            PlayerPrefs.SetInt(KeyPityPetGold, _pityPetGold);
            PlayerPrefs.Save();
            FinishPetPull(tier);
        }

        private void PullPetDiamond()
        {
            var eco = EconomyManager.Instance;
            if (eco == null || !eco.SpendDiamonds(50)) { ShowResult("✗ Yeterli elmas yok!", Color.red); return; }

            _pityPetDiamond++;
            Rarity tier;
            if (_pityPetDiamond >= PityLimit)
            {
                tier = UnityEngine.Random.value < 0.8f ? Rarity.B : Rarity.A;
                _pityPetDiamond = 0;
            }
            else
            {
                float r = UnityEngine.Random.value;
                tier = r < 0.40f ? Rarity.D : r < 0.75f ? Rarity.C : r < 0.95f ? Rarity.B : Rarity.A;
                if (tier >= Rarity.B) _pityPetDiamond = 0;
            }
            PlayerPrefs.SetInt(KeyPityPetDiamond, _pityPetDiamond);
            PlayerPrefs.Save();
            FinishPetPull(tier);
        }

        private void FinishPetPull(Rarity tier)
        {
            if (_collection == null) { ShowResult("✗ Koleksiyon bulunamadı!", Color.red); return; }
            var pet = _collection.PullFromGacha(tier);
            ShowResult($"✓ [{tier}] {pet.DisplayName} yakalandı!  (Max: {pet.MaxEvolutionTier})", TierColor(tier));
        }

        // ── Ekipman gacha mantığı ──────────────────────────────────────────────

        private void PullEqGold()
        {
            var eco = EconomyManager.Instance;
            if (eco == null || !eco.SpendGold(1000)) { ShowResult("✗ Yeterli altın yok!", Color.red); return; }

            _pityEqGold++;
            Rarity tier;
            if (_pityEqGold >= PityLimit)
            {
                tier = UnityEngine.Random.value < 0.8f ? Rarity.C : Rarity.B;
                _pityEqGold = 0;
            }
            else
            {
                float r = UnityEngine.Random.value;
                tier = r < 0.55f ? Rarity.F : r < 0.85f ? Rarity.D : r < 0.97f ? Rarity.C : Rarity.B;
                if (tier >= Rarity.C) _pityEqGold = 0;
            }
            PlayerPrefs.SetInt(KeyPityEqGold, _pityEqGold);
            PlayerPrefs.Save();
            FinishEqPull(tier);
        }

        private void PullEqDiamond()
        {
            var eco = EconomyManager.Instance;
            if (eco == null || !eco.SpendDiamonds(50)) { ShowResult("✗ Yeterli elmas yok!", Color.red); return; }

            _pityEqDiamond++;
            Rarity tier;
            if (_pityEqDiamond >= PityLimit)
            {
                tier = UnityEngine.Random.value < 0.8f ? Rarity.B : Rarity.A;
                _pityEqDiamond = 0;
            }
            else
            {
                float r = UnityEngine.Random.value;
                tier = r < 0.40f ? Rarity.D : r < 0.75f ? Rarity.C : r < 0.95f ? Rarity.B : Rarity.A;
                if (tier >= Rarity.B) _pityEqDiamond = 0;
            }
            PlayerPrefs.SetInt(KeyPityEqDiamond, _pityEqDiamond);
            PlayerPrefs.Save();
            FinishEqPull(tier);
        }

        private void FinishEqPull(Rarity tier)
        {
            bool isArmor = UnityEngine.Random.value < 0.5f;
            var slot = isArmor
                ? EquipmentManager.RandomArmorSlot()
                : EquipmentManager.RandomAccessorySlot();
            var eq = EquipmentManager.CreateEquipment(slot, tier);
            EquipmentManager.Instance.AddEquipment(eq);

            string stats = StatsShort(eq);
            string icon  = isArmor ? "🛡" : "💍";
            ShowResult($"{icon} [{tier}] {eq.DisplayName}  ({stats}) düştü!", TierColor(tier));
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
            _pullResult      = msg;
            _pullResultColor = col;
            _pullResultExpiry = Time.realtimeSinceStartup + 4f;
        }

        private static Color TierColor(Rarity t) => t switch
        {
            Rarity.SS => new Color(1.00f, 0.55f, 0.10f),
            Rarity.S  => new Color(0.80f, 0.30f, 1.00f),
            Rarity.A  => new Color(0.40f, 0.75f, 1.00f),
            Rarity.B  => new Color(0.30f, 0.90f, 0.50f),
            Rarity.C  => new Color(0.55f, 0.90f, 0.40f),
            Rarity.D  => new Color(0.95f, 0.80f, 0.30f),
            _         => new Color(0.80f, 0.78f, 0.80f),
        };

        // ── Yardımcılar ───────────────────────────────────────────────────────

        private void DrawCloseBtn(float sw, float sh)
        {
            float bw = sw * 0.12f;
            float bh = sh * 0.050f;
            GUI.color = new Color(0.70f, 0.20f, 0.20f);
            if (GUI.Button(new Rect(sw - bw - 8f, 8f, bw, bh), "✕", _styleBtnX))
                ScreenNavigator.GoToHub();
            GUI.color = Color.white;
        }

        private void BuildStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            int fs = Mathf.Max(13, Screen.width / 22);

            _styleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs + 6,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };
            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
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
