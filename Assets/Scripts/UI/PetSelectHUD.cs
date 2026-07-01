using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Pet seçim ekranı — Hub'dan açılır, X ile Hub'a döner.
    /// </summary>
    public class PetSelectHUD : MonoBehaviour
    {
        private static readonly Color ColBg   = new Color(0.05f, 0.04f, 0.10f, 1.00f);
        private static readonly Color ColGold = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColCard = new Color(0.12f, 0.11f, 0.18f, 1.00f);
        private static readonly Color ColSel  = new Color(0.20f, 0.35f, 0.55f, 1.00f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleSmall;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnX;
        private bool     _stylesReady;

        private MonsterCollection _collection;
        private Vector2           _scroll;

        private void Awake() =>
            _collection = UnityEngine.Object.FindFirstObjectByType<MonsterCollection>();

        private void OnGUI()
        {
            if (ScreenNavigator.Current != GameScreen.PetSelect) return;
            BuildStyles();

            float sw  = Screen.width;
            float sh  = Screen.height;
            float pad = sw * 0.05f;
            float w   = sw - pad * 2f;

            // Arka plan
            GUI.color = ColBg;
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // X butonu
            DrawCloseBtn(sw, sh);

            float y = sh * 0.06f;

            // Başlık
            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w * 0.80f, sh * 0.07f), "🐾 Pet Seç", _styleTitle);
            y += sh * 0.09f;

            if (_collection == null || _collection.Monsters.Count == 0)
            {
                _styleLabel.normal.textColor = Color.gray;
                GUI.Label(new Rect(pad, y, w, sh * 0.07f),
                    "Henüz petın yok — zindanda canavar yakala!", _styleLabel);
                return;
            }

            var   monsters = _collection.Monsters;
            string selId   = _collection.SelectedPetId;

            // Aktif pet özeti
            var selPet = _collection.SelectedPet;
            if (selPet != null)
            {
                _styleSmall.normal.textColor = TierColor(selPet.Tier);
                GUI.Label(new Rect(pad, y, w, sh * 0.04f),
                    $"Aktif Pet: {selPet.DisplayName}  [{selPet.Tier}]", _styleSmall);
                y += sh * 0.045f;
            }

            // Pet listesi (scroll)
            float rowH  = sh * 0.095f;
            float viewH = sh - y - sh * 0.06f;
            var   viewR = new Rect(pad, y, w, viewH);
            var   contR = new Rect(0, 0, w - 16f, monsters.Count * rowH);
            _scroll = GUI.BeginScrollView(viewR, _scroll, contR, false, false);

            for (int i = 0; i < monsters.Count; i++)
            {
                var  m   = monsters[i];
                float ry = i * rowH;
                bool isSel = m.InstanceId == selId;

                // Kart arka planı
                GUI.color = isSel ? ColSel : ColCard;
                GUI.DrawTexture(new Rect(0, ry, w - 16f, rowH - 4f), Texture2D.whiteTexture);
                GUI.color = Color.white;

                float ip = w * 0.03f;

                // Tier + isim
                GUI.color = TierColor(m.Tier);
                _styleLabel.normal.textColor = TierColor(m.Tier);
                GUI.Label(new Rect(ip, ry + 6f, w * 0.58f, rowH * 0.46f),
                    $"[{m.Tier}]  {m.DisplayName}", _styleLabel);

                // Bonus
                var (hpM, atkM) = MonsterCollection.BonusForTier(m.Tier);
                int bonusPct = Mathf.RoundToInt((hpM - 1f) * 100);
                _styleSmall.normal.textColor = Color.gray;
                GUI.Label(new Rect(ip, ry + rowH * 0.50f, w * 0.58f, rowH * 0.40f),
                    $"Kat {m.FloorCaptured}  •  +{bonusPct}% HP & ATK", _styleSmall);

                GUI.color = Color.white;

                // Seç butonu
                float bw = w * 0.30f;
                float bx = w - 16f - bw - ip;
                float by = ry + (rowH - 4f - sh * 0.048f) * 0.5f;

                if (isSel)
                {
                    GUI.color = new Color(0.3f, 0.8f, 0.3f);
                    _styleBtn.normal.textColor = new Color(0.3f, 0.8f, 0.3f);
                    GUI.Label(new Rect(bx, by, bw, sh * 0.048f), "✓ Aktif", _styleBtn);
                    GUI.color = Color.white;
                }
                else
                {
                    if (GUI.Button(new Rect(bx, by, bw, sh * 0.048f), "Seç", _styleBtn))
                        _collection.SelectPet(m.InstanceId);
                }
            }

            GUI.EndScrollView();
            GUI.color = Color.white;
        }

        private void DrawCloseBtn(float sw, float sh)
        {
            float bw = sw * 0.12f;
            float bh = sh * 0.055f;
            GUI.color = new Color(0.70f, 0.20f, 0.20f);
            if (GUI.Button(new Rect(sw - bw - 8f, 8f, bw, bh), "✕", _styleBtnX))
                ScreenNavigator.GoToHub();
            GUI.color = Color.white;
        }

        private static Color TierColor(Rarity tier) => tier switch
        {
            Rarity.B => new Color(0.40f, 0.75f, 1.00f),
            Rarity.C => new Color(0.55f, 0.90f, 0.40f),
            Rarity.D => new Color(0.95f, 0.80f, 0.30f),
            _        => new Color(0.80f, 0.78f, 0.80f),
        };

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
