using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Savaş öncesi sınıf seçim ekranı. 2×2 kart grid, UI Toolkit.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ClassSelectionScreen : MonoBehaviour
    {
        public event Action<ClassData> OnClassSelected;

        // Görüntüleme sırası: Savaşçı / Büyücü / Hırsız / Şifacı
        private static readonly string[] ClassOrder =
            { "Savasco", "Buyucu", "Hirsiz", "Sifaci" };

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var grid = root.Q<VisualElement>("grid");

            var loaded = new Dictionary<string, ClassData>();
            foreach (var cd in Resources.LoadAll<ClassData>("Classes"))
                loaded[cd.name] = cd;

            var cards = new System.Collections.Generic.List<VisualElement>();
            foreach (var id in ClassOrder)
            {
                if (loaded.TryGetValue(id, out var data))
                    cards.Add(BuildCard(data));
            }

            // 2×2 grid — satır bazlı
            for (int i = 0; i < cards.Count; i += 2)
            {
                var row = new VisualElement();
                row.AddToClassList("grid-row");
                row.Add(cards[i]);
                if (i + 1 < cards.Count) row.Add(cards[i + 1]);
                grid.Add(row);
            }
        }

        private VisualElement BuildCard(ClassData data)
        {
            var card = new VisualElement();
            card.AddToClassList("card");

            // İsim
            var nameLabel = new Label(data.ClassName);
            nameLabel.AddToClassList("card-name");
            card.Add(nameLabel);

            // Hasar türü
            var typeLabel = new Label(data.DamageType == DamageType.Magic ? "— Büyü —" : "— Fiziksel —");
            typeLabel.AddToClassList("card-type");
            card.Add(typeLabel);

            // Çizgi
            var div = new VisualElement();
            div.AddToClassList("divider");
            card.Add(div);

            // Statlar — 2 sütun
            var row1 = new VisualElement();
            row1.AddToClassList("stats-row");
            row1.Add(MakeStat("HP", data.BaseHP.ToString()));
            row1.Add(MakeStat("ATK", data.BaseATK.ToString()));
            card.Add(row1);

            var row2 = new VisualElement();
            row2.AddToClassList("stats-row");
            row2.Add(MakeStat("DEF", data.BaseDEF.ToString()));
            row2.Add(MakeStat("HIZ", data.BaseSPD.ToString()));
            card.Add(row2);

            // Yetenekler
            var skills = new VisualElement();
            skills.AddToClassList("skills-container");
            foreach (var skill in data.Skills)
            {
                if (skill == null) continue;
                var sl = new Label($"• {skill.SkillName}");
                sl.AddToClassList("skill-item");
                skills.Add(sl);
            }
            card.Add(skills);

            // Seç butonu
            var btn = new Button(() => Select(data)) { text = "Seç" };
            btn.AddToClassList("card-btn");
            card.Add(btn);

            return card;
        }

        private static Label MakeStat(string key, string value)
        {
            var lbl = new Label($"{key}: {value}");
            lbl.AddToClassList("stat");
            return lbl;
        }

        private void Select(ClassData data)
        {
            GetComponent<UIDocument>().rootVisualElement
                .Q<VisualElement>("overlay").style.display = DisplayStyle.None;
            OnClassSelected?.Invoke(data);
        }
    }
}
