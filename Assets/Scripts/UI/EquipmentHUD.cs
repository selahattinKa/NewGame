using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;
using CanavarZindanlari.Equipment;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Ekipman yönetim ekranı — kuşanılan slotlar ve envanter.
    /// İki sekme: Kuşanılan / Envanter.
    /// </summary>
    public class EquipmentHUD : MonoBehaviour
    {
        private static readonly Color ColBg       = new Color(0.05f, 0.04f, 0.10f, 1f);
        private static readonly Color ColGold      = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColCard      = new Color(0.12f, 0.11f, 0.20f, 1f);
        private static readonly Color ColSection   = new Color(0.18f, 0.16f, 0.28f, 1f);
        private static readonly Color ColEmpty     = new Color(0.08f, 0.08f, 0.14f, 1f);
        private static readonly Color ColEquipBtn  = new Color(0.25f, 0.65f, 0.35f);
        private static readonly Color ColRemoveBtn = new Color(0.70f, 0.22f, 0.22f);
        private static readonly Color ColTabActive = new Color(0.22f, 0.20f, 0.38f, 1f);
        private static readonly Color ColTabIdle   = new Color(0.10f, 0.09f, 0.16f, 1f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleSmall;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnX;
        private bool     _stylesReady;

        private Vector2 _scroll;
        private int     _tab; // 0=Kuşanılan  1=Envanter

        private static readonly (EquipmentSlotType Slot, string Icon, string Label)[] SlotDefs =
        {
            (EquipmentSlotType.Weapon,   "⚔",  "Silah"),
            (EquipmentSlotType.Head,     "🪖", "Kask"),
            (EquipmentSlotType.Chest,    "🛡",  "Göğüslük"),
            (EquipmentSlotType.Gloves,   "🧤", "Eldiven"),
            (EquipmentSlotType.Legs,     "👖", "Pantolon"),
            (EquipmentSlotType.Boots,    "👢", "Çizme"),
            (EquipmentSlotType.Necklace, "📿", "Kolye"),
            (EquipmentSlotType.Ring1,    "💍", "Yüzük 1"),
            (EquipmentSlotType.Ring2,    "💍", "Yüzük 2"),
            (EquipmentSlotType.Earring1, "💎", "Küpe 1"),
            (EquipmentSlotType.Earring2, "💎", "Küpe 2"),
        };

        private void OnGUI()
        {
            if (ScreenNavigator.Current != GameScreen.Equipment) return;
            BuildStyles();

            float sw  = Screen.width;
            float sh  = Screen.height;
            float pad = sw * 0.04f;
            float w   = sw - pad * 2f;

            // Arka plan
            GUI.color = ColBg;
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float y = sh * 0.03f;

            // Başlık + kapat
            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w * 0.75f, sh * 0.065f), "🎒 EKİPMAN", _styleTitle);

            float xbw = sw * 0.12f;
            float xbh = sh * 0.048f;
            GUI.color = ColRemoveBtn;
            if (GUI.Button(new Rect(sw - xbw - 8f, y + 4f, xbw, xbh), "✕", _styleBtnX))
                ScreenNavigator.GoToHub();
            GUI.color = Color.white;
            y += sh * 0.07f;

            // Bonus özeti
            var eq = EquipmentManager.Instance;
            var (bonATK, bonHP, bonDEF, bonSPD) = eq.GetTotalBonuses();
            _styleSmall.normal.textColor = new Color(0.85f, 0.85f, 0.60f);
            GUI.Label(new Rect(pad, y, w, sh * 0.038f),
                $"Toplam Bonus → ATK: +{bonATK}  HP: +{bonHP}  DEF: +{bonDEF}  HIZ: +{bonSPD}",
                _styleSmall);
            y += sh * 0.048f;

            // Sekmeler
            float tabW = w * 0.5f;
            float tabH = sh * 0.055f;
            DrawTab(pad,          y, tabW, tabH, "⚔ Kuşanılan", 0);
            DrawTab(pad + tabW,   y, tabW, tabH, "📦 Envanter",  1);
            y += tabH + sh * 0.012f;

            // Sekme içeriği
            float contentH = sh - y - sh * 0.02f;
            if (_tab == 0)
                DrawEquippedTab(sw, sh, pad, w, y, contentH, eq);
            else
                DrawInventoryTab(sw, sh, pad, w, y, contentH, eq);
        }

        // ── Sekme butonları ───────────────────────────────────────────────────

        private void DrawTab(float x, float y, float w, float h, string label, int idx)
        {
            GUI.color = _tab == idx ? ColTabActive : ColTabIdle;
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;
            _styleSmall.normal.textColor = _tab == idx ? Color.white : new Color(0.5f, 0.5f, 0.6f);
            if (GUI.Button(new Rect(x, y, w, h), label, _styleSmall))
            {
                _tab = idx;
                _scroll = Vector2.zero;
            }
        }

        // ── Kuşanılan sekmesi ─────────────────────────────────────────────────

        private void DrawEquippedTab(float sw, float sh, float pad, float w,
            float y, float contentH, EquipmentManager eq)
        {
            float rowH = sh * 0.070f;
            float gap  = sh * 0.007f;
            float totalH = SlotDefs.Length * (rowH + gap);

            _scroll = GUI.BeginScrollView(
                new Rect(pad, y, w, contentH),
                _scroll,
                new Rect(0, 0, w - 16f, totalH),
                false, false);

            float ry = 0f;
            foreach (var (slot, icon, label) in SlotDefs)
            {
                var item = eq.GetEquipped(slot);
                DrawEquippedRow(sw, sh, w, ry, rowH, slot, icon, label, item, eq);
                ry += rowH + gap;
            }

            GUI.EndScrollView();
        }

        private void DrawEquippedRow(float sw, float sh, float w, float ry, float rowH,
            EquipmentSlotType slot, string icon, string label,
            OwnedEquipment item, EquipmentManager eq)
        {
            GUI.color = item != null ? ColCard : ColEmpty;
            GUI.DrawTexture(new Rect(0, ry, w - 16f, rowH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float ip = w * 0.03f;

            // Slot etiketi
            _styleSmall.normal.textColor = new Color(0.55f, 0.55f, 0.65f);
            GUI.Label(new Rect(ip, ry + 2f, sw * 0.22f, rowH - 4f), $"{icon} {label}", _styleSmall);

            if (item != null)
            {
                // İtem adı
                _styleSmall.normal.textColor = TierColor(item.Tier);
                GUI.Label(new Rect(sw * 0.24f, ry + 2f, w * 0.44f, rowH * 0.50f),
                    item.DisplayName, _styleSmall);

                // Stat bonus
                _styleSmall.normal.textColor = Color.white;
                GUI.Label(new Rect(sw * 0.24f, ry + rowH * 0.52f, w * 0.44f, rowH * 0.46f),
                    StatsStr(item), _styleSmall);

                // Çıkar butonu
                float bw = sw * 0.20f;
                float bx = w - 16f - bw - ip;
                GUI.color = ColRemoveBtn;
                if (GUI.Button(new Rect(bx, ry + rowH * 0.18f, bw, rowH * 0.64f), "Çıkar", _styleSmall))
                    eq.Unequip(slot);
                GUI.color = Color.white;
            }
            else
            {
                _styleSmall.normal.textColor = new Color(0.35f, 0.35f, 0.45f);
                GUI.Label(new Rect(sw * 0.24f, ry, w * 0.70f, rowH), "— Boş —", _styleSmall);
            }
        }

        // ── Envanter sekmesi ──────────────────────────────────────────────────

        private void DrawInventoryTab(float sw, float sh, float pad, float w,
            float y, float contentH, EquipmentManager eq)
        {
            var all = eq.Inventory;

            if (all.Count == 0)
            {
                _styleSmall.normal.textColor = Color.gray;
                GUI.Label(new Rect(pad, y + sh * 0.04f, w, sh * 0.06f),
                    "Envanteriniz boş.\nGacha kutularından ekipman kazanın!", _styleSmall);
                return;
            }

            float rowH   = sh * 0.082f;
            float gap    = sh * 0.008f;
            float totalH = all.Count * (rowH + gap);

            _scroll = GUI.BeginScrollView(
                new Rect(pad, y, w, contentH),
                _scroll,
                new Rect(0, 0, w - 16f, totalH),
                false, false);

            float ry = 0f;
            foreach (var item in all)
            {
                bool equipped = eq.IsEquipped(item.Id);
                DrawInventoryRow(sw, sh, w, ry, rowH, item, equipped, eq);
                ry += rowH + gap;
            }

            GUI.EndScrollView();
        }

        private void DrawInventoryRow(float sw, float sh, float w, float ry, float rowH,
            OwnedEquipment item, bool equipped, EquipmentManager eq)
        {
            GUI.color = equipped ? new Color(0.15f, 0.18f, 0.12f, 1f) : ColCard;
            GUI.DrawTexture(new Rect(0, ry, w - 16f, rowH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float bw = sw * 0.22f;
            float ip = w * 0.03f;

            // İtem adı + kuşanıldı etiketi
            _styleSmall.normal.textColor = TierColor(item.Tier);
            string nameStr = item.DisplayName + (equipped ? "  ✓" : "");
            GUI.Label(new Rect(ip, ry + 2f, w - bw - ip * 2f, rowH * 0.50f), nameStr, _styleSmall);

            // Stat bonus
            _styleSmall.normal.textColor = Color.white;
            GUI.Label(new Rect(ip, ry + rowH * 0.52f, w - bw - ip * 2f, rowH * 0.46f),
                StatsStr(item), _styleSmall);

            // Kuşan / Çıkar butonu
            float bx = w - 16f - bw - ip;
            if (equipped)
            {
                GUI.color = ColRemoveBtn;
                if (GUI.Button(new Rect(bx, ry + rowH * 0.18f, bw, rowH * 0.64f), "Çıkar", _styleSmall))
                    eq.Unequip(item.Slot);
            }
            else
            {
                GUI.color = ColEquipBtn;
                if (GUI.Button(new Rect(bx, ry + rowH * 0.18f, bw, rowH * 0.64f), "Kuşan", _styleSmall))
                    eq.Equip(item.Id);
            }
            GUI.color = Color.white;
        }

        // ── Yardımcılar ───────────────────────────────────────────────────────

        private static string StatsStr(OwnedEquipment item)
        {
            var parts = new System.Text.StringBuilder();
            if (item.BonusATK > 0) parts.Append($"+{item.BonusATK} ATK  ");
            if (item.BonusHP  > 0) parts.Append($"+{item.BonusHP} HP  ");
            if (item.BonusDEF > 0) parts.Append($"+{item.BonusDEF} DEF  ");
            if (item.BonusSPD > 0) parts.Append($"+{item.BonusSPD} HIZ");
            return parts.Length > 0 ? parts.ToString() : "—";
        }

        private static Color TierColor(Rarity t) => t switch
        {
            Rarity.SS => new Color(1.00f, 0.55f, 0.10f),
            Rarity.S  => new Color(0.80f, 0.30f, 1.00f),
            Rarity.A  => new Color(0.40f, 0.75f, 1.00f),
            Rarity.B  => new Color(0.30f, 0.90f, 0.50f),
            Rarity.C  => new Color(0.55f, 0.90f, 0.40f),
            Rarity.D  => new Color(0.95f, 0.80f, 0.30f),
            _         => new Color(0.75f, 0.73f, 0.75f),
        };

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
