using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Oyun başında gösterilen sınıf seçim ekranı.
    /// Seçim PlayerPrefs'e kaydedilir; bir kez seçildikten sonra tekrar açılmaz.
    /// </summary>
    public class ClassSelectHUD : MonoBehaviour
    {
        private static readonly Color ColBg    = new Color(0.05f, 0.04f, 0.10f, 1f);
        private static readonly Color ColGold  = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColCard  = new Color(0.12f, 0.11f, 0.20f, 1f);
        private static readonly Color ColPhys  = new Color(0.90f, 0.45f, 0.20f);
        private static readonly Color ColMagic = new Color(0.40f, 0.65f, 1.00f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleSmall;
        private GUIStyle _styleBtn;
        private bool     _stylesReady;

        private ClassData[] _classes;
        private Vector2     _scroll;

        private static readonly string[] ClassOrder = { "Savasco", "Buyucu", "Hirsiz", "Sifaci" };

        private void Awake()
        {
            var loaded = Resources.LoadAll<ClassData>("Classes");
            var ordered = new System.Collections.Generic.List<ClassData>();
            foreach (var id in ClassOrder)
                foreach (var c in loaded)
                    if (c.name == id) { ordered.Add(c); break; }
            // Sırada olmayanları sona ekle
            foreach (var c in loaded)
                if (!ordered.Contains(c)) ordered.Add(c);
            _classes = ordered.ToArray();
        }

        private void OnGUI()
        {
            if (ScreenNavigator.Current != GameScreen.ClassSelect) return;
            BuildStyles();

            float sw  = Screen.width;
            float sh  = Screen.height;
            float pad = sw * 0.05f;
            float w   = sw - pad * 2f;

            // Arka plan
            GUI.color = ColBg;
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float y = sh * 0.05f;

            // Başlık
            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w, sh * 0.07f), "⚔  SINIFINI SEÇ", _styleTitle);
            y += sh * 0.06f;

            _styleSmall.normal.textColor = Color.gray;
            GUI.Label(new Rect(pad, y, w, sh * 0.04f),
                "Sınıfın oyunun tamamında sabit kalır. Dikkatlice seç!", _styleSmall);
            y += sh * 0.05f;

            if (_classes == null || _classes.Length == 0)
            {
                _styleLabel.normal.textColor = new Color(1f, 0.4f, 0.3f);
                GUI.Label(new Rect(pad, y, w, sh * 0.07f),
                    "Sınıf verisi bulunamadı!\nResources/Classes/ klasörünü kontrol et.", _styleLabel);
                return;
            }

            float cardH = sh * 0.27f;
            float gap   = sh * 0.018f;
            float viewH = sh - y - sh * 0.03f;
            var   viewR = new Rect(pad, y, w, viewH);
            var   contR = new Rect(0, 0, w - 16f, _classes.Length * (cardH + gap));
            _scroll = GUI.BeginScrollView(viewR, _scroll, contR, false, false);

            for (int i = 0; i < _classes.Length; i++)
            {
                var   cls = _classes[i];
                float cy  = i * (cardH + gap);

                // Kart arka planı
                GUI.color = ColCard;
                GUI.DrawTexture(new Rect(0, cy, w - 16f, cardH), Texture2D.whiteTexture);
                GUI.color = Color.white;

                float ip = w * 0.04f;
                float lh = sh * 0.034f;

                // İsim + hasar türü
                bool isMagic = cls.DamageType == DamageType.Magic;
                _styleLabel.normal.textColor = isMagic ? ColMagic : ColPhys;
                GUI.Label(new Rect(ip, cy + ip, w * 0.65f, lh * 1.2f), cls.ClassName, _styleLabel);
                _styleSmall.normal.textColor = isMagic ? ColMagic : ColPhys;
                GUI.Label(new Rect(ip, cy + ip + lh * 1.25f, w * 0.65f, lh),
                    isMagic ? "— Büyü Hasarı —" : "— Fiziksel Hasar —", _styleSmall);

                // Statlar
                _styleSmall.normal.textColor = Color.white;
                float sy = cy + ip + lh * 2.5f;
                GUI.Label(new Rect(ip, sy, w * 0.45f, lh),
                    $"HP: {cls.BaseHP}   ATK: {cls.BaseATK}   DEF: {cls.BaseDEF}   HIZ: {cls.BaseSPD}",
                    _styleSmall);

                // Yetenekler
                _styleSmall.normal.textColor = Color.gray;
                float skillY = sy + lh * 1.1f;
                for (int s = 0; s < cls.Skills.Length && s < 4; s++)
                {
                    var sk = cls.Skills[s];
                    if (sk == null) continue;
                    string cdText = s == 0 ? "Hazır" : ("CD:" + (s == 1 ? 3 : s == 2 ? 5 : 8));
                    GUI.Label(new Rect(ip, skillY, w * 0.70f, lh),
                        $"• {sk.SkillName}  [{cdText}]", _styleSmall);
                    skillY += lh * 0.95f;
                }

                // Seç butonu
                float bw = w * 0.28f;
                float bx = w - 16f - bw - ip;
                float bby = cy + (cardH - sh * 0.085f) * 0.5f;
                GUI.color = isMagic ? ColMagic : ColPhys;
                if (GUI.Button(new Rect(bx, bby, bw, sh * 0.085f), "Seç", _styleBtn))
                    SelectClass(cls);
                GUI.color = Color.white;
            }

            GUI.EndScrollView();
        }

        private void SelectClass(ClassData data)
        {
            var dungeon = UnityEngine.Object.FindFirstObjectByType<DungeonManager>();
            dungeon?.SetClass(data);
            ScreenNavigator.GoToHub();
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
                alignment = TextAnchor.MiddleLeft,
            };

            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs + 1,
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
                fontSize  = fs + 1,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
            };
        }
    }
}
