using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Economy;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Başlangıç kasabası — oyunun ana navigasyon ekranı.
    /// Tüm diğer ekranlar buradan açılır.
    /// </summary>
    public class HubHUD : MonoBehaviour
    {
        // ── Renkler ──────────────────────────────────────────────────────────

        private static readonly Color ColBg       = new Color(0.05f, 0.04f, 0.10f, 1.00f);
        private static readonly Color ColGold      = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColBlue      = new Color(0.25f, 0.50f, 0.88f);
        private static readonly Color ColGreen     = new Color(0.25f, 0.75f, 0.35f);
        private static readonly Color ColRed       = new Color(0.80f, 0.25f, 0.25f);
        private static readonly Color ColDisabled  = new Color(0.35f, 0.35f, 0.40f);
        private static readonly Color ColCard      = new Color(0.12f, 0.11f, 0.18f, 1.00f);

        // ── Stiller ──────────────────────────────────────────────────────────

        private GUIStyle _styleTitle;
        private GUIStyle _styleSubtitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnDisabled;
        private bool     _stylesReady;

        // ── Bağlantılar ──────────────────────────────────────────────────────

        private DungeonManager    _dungeon;
        private MonsterCollection _collection;

        private void Awake()
        {
            _dungeon    = FindFirstObjectOfType<DungeonManager>();
            _collection = FindFirstObjectOfType<MonsterCollection>();
        }

        private void OnGUI()
        {
            if (ScreenNavigator.Current != GameScreen.Hub) return;
            BuildStyles();

            float sw = Screen.width;
            float sh = Screen.height;
            float pad = sw * 0.05f;
            float w   = sw - pad * 2f;

            // Arka plan
            GUI.color = ColBg;
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float y = sh * 0.06f;

            // ── Başlık ────────────────────────────────────────────────────────
            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w, sh * 0.08f), "🏰 BAŞLANGIÇ KASABASI", _styleTitle);
            y += sh * 0.10f;

            // ── Oyuncu bilgi kartı ────────────────────────────────────────────
            float cardH = sh * 0.13f;
            GUI.color = ColCard;
            GUI.DrawTexture(new Rect(pad, y, w, cardH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float lh = cardH * 0.32f;
            string classText = _dungeon?.PlayerClass != null
                ? $"Sınıf: {_dungeon.PlayerClass.ClassName}"
                : "Sınıf: Seçilmedi";
            var selPet = _collection?.SelectedPet;
            string petText = selPet != null
                ? $"🐾 {selPet.DisplayName}  [{selPet.Tier}]"
                : "🐾 Pet: Seçilmedi";

            int gold    = EconomyManager.Instance?.Gold    ?? 0;
            int diamond = EconomyManager.Instance?.Diamond ?? 0;
            string ecoText = $"🪙 {gold:N0}   💎 {diamond:N0}";

            _styleLabel.normal.textColor = Color.white;
            GUI.Label(new Rect(pad + 8, y + 2,          w - 8, lh), classText, _styleLabel);
            _styleLabel.normal.textColor = selPet != null ? ColGold : Color.gray;
            GUI.Label(new Rect(pad + 8, y + 2 + lh,     w - 8, lh), petText,   _styleLabel);
            _styleLabel.normal.textColor = new Color(0.95f, 0.85f, 0.40f);
            GUI.Label(new Rect(pad + 8, y + 2 + lh * 2, w - 8, lh), ecoText,   _styleLabel);
            y += cardH + sh * 0.04f;

            // ── Navigasyon butonları ──────────────────────────────────────────
            float btnH = sh * 0.10f;
            float gap  = sh * 0.018f;

            DrawNavBtn(new Rect(pad, y, w, btnH), "🗺  Zindan Keşfi",   ColBlue,  GameScreen.Dungeon);
            y += btnH + gap;
            DrawNavBtn(new Rect(pad, y, w, btnH), "⚔  Arena",           ColRed,   GameScreen.Arena);
            y += btnH + gap;
            DrawNavBtn(new Rect(pad, y, w, btnH), "🐾  Petimi Seç",     ColGreen, GameScreen.PetSelect);
            y += btnH + gap;
            DrawNavBtn(new Rect(pad, y, w, btnH), "🛒  Mağaza", ColGold, GameScreen.Shop);
            y += btnH + gap;
            DrawNavBtnDisabled(new Rect(pad, y, w, btnH), "📖  Hikaye  (Yakında)");
        }

        // ── Yardımcılar ──────────────────────────────────────────────────────

        private void DrawNavBtn(Rect r, string label, Color col, GameScreen target)
        {
            GUI.color = col;
            if (GUI.Button(r, label, _styleBtn))
                ScreenNavigator.GoTo(target);
            GUI.color = Color.white;
        }

        private void DrawNavBtnDisabled(Rect r, string label)
        {
            GUI.color = ColDisabled;
            GUI.Button(r, label, _styleBtnDisabled);
            GUI.color = Color.white;
        }

        private void BuildStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            int fs = Mathf.Max(14, Screen.width / 22);

            _styleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs + 8,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };

            _styleSubtitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs - 2,
                alignment = TextAnchor.MiddleCenter,
            };

            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs - 1,
                alignment = TextAnchor.MiddleLeft,
            };

            _styleBtn = new GUIStyle(GUI.skin.button)
            {
                fontSize  = fs + 1,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
            };

            _styleBtnDisabled = new GUIStyle(_styleBtn)
            {
                normal = { textColor = new Color(0.6f, 0.6f, 0.6f) },
            };
        }

        private static T FindFirstObjectOfType<T>() where T : UnityEngine.Object =>
            UnityEngine.Object.FindFirstObjectByType<T>();
    }
}
