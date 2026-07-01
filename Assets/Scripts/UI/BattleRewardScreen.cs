using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Savaş sonu ödül ekranı. UIDocument üzerinden çalışır.
    /// CombatManager.OnBattleEnded'e abone olur; sonucu gösterir.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class BattleRewardScreen : MonoBehaviour
    {
        [SerializeField] private CombatManager _combat;

        // Sahne yöneticisi için çıkış eventi — bağlayan taraf dinler
        public event System.Action OnContinueClicked;

        private VisualElement _overlay;
        private Label _titleLabel;
        private Label _expValue;
        private VisualElement _firstClearRow;
        private Label _gemsValue;
        private VisualElement _lootContainer;
        private Button _continueBtn;

        private const float ExpAnimDuration = 1.4f;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _overlay       = root.Q<VisualElement>("overlay");
            _titleLabel    = root.Q<Label>("title-label");
            _expValue      = root.Q<Label>("exp-value");
            _firstClearRow = root.Q<VisualElement>("first-clear-row");
            _gemsValue     = root.Q<Label>("gems-value");
            _lootContainer = root.Q<VisualElement>("loot-container");
            _continueBtn   = root.Q<Button>("continue-btn");

            _continueBtn.clicked += Hide;
            _continueBtn.clicked += () =>
            {
                OnContinueClicked?.Invoke();
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            };

            Hide();
        }

        private void OnEnable()
        {
            if (_combat != null)
                _combat.OnBattleEnded += Show;
        }

        private void OnDisable()
        {
            if (_combat != null)
                _combat.OnBattleEnded -= Show;
        }

        // ── Göster / gizle ─────────────────────────────────────────────────────

        private void Show(BattleReward reward)
        {
            _overlay.style.display = DisplayStyle.Flex;

            if (reward.IsVictory)
            {
                _titleLabel.text = "ZAFERİ!";
                _titleLabel.RemoveFromClassList("title--defeat");
                _continueBtn.text = "Devam Et";
            }
            else
            {
                _titleLabel.text = "YENİLDİN";
                _titleLabel.AddToClassList("title--defeat");
                _continueBtn.text = "Tekrar Dene";
            }

            // First-clear satırı
            if (reward.IsFirstClear && reward.BonusGems > 0)
            {
                _firstClearRow.style.display = DisplayStyle.Flex;
                _gemsValue.text = $"+{reward.BonusGems} 💎";
            }
            else
            {
                _firstClearRow.style.display = DisplayStyle.None;
            }

            // Loot listesi
            _lootContainer.Clear();
            if (reward.Items != null && reward.Items.Length > 0)
            {
                foreach (var drop in reward.Items)
                    _lootContainer.Add(BuildLootRow(drop));
            }
            else
            {
                var empty = new Label("— loot yok —");
                empty.AddToClassList("loot-empty");
                _lootContainer.Add(empty);
            }

            // EXP animasyonu
            StopAllCoroutines();
            StartCoroutine(AnimateExp(reward.ExpGained));
        }

        private void Hide()
        {
            _overlay.style.display = DisplayStyle.None;
        }

        // ── EXP sayaç animasyonu ───────────────────────────────────────────────

        private IEnumerator AnimateExp(int target)
        {
            float elapsed = 0f;
            while (elapsed < ExpAnimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / ExpAnimDuration);
                // Ease-out: yavaşlayarak artar
                t = 1f - (1f - t) * (1f - t);
                int display = Mathf.RoundToInt(target * t);
                _expValue.text = $"+{display}";
                yield return null;
            }
            _expValue.text = $"+{target}";
        }

        // ── Loot satırı ────────────────────────────────────────────────────────

        private static VisualElement BuildLootRow(LootDrop drop)
        {
            var row = new VisualElement();
            row.AddToClassList("loot-row");

            var name = new Label(drop.ItemName);
            name.AddToClassList("loot-name");

            var qty = new Label($"x{drop.Quantity}");
            qty.AddToClassList("loot-qty");

            var badge = new Label(drop.Rarity.ToString());
            badge.AddToClassList("loot-rarity");
            badge.AddToClassList($"rarity-{drop.Rarity.ToString().ToLower()}");

            row.Add(name);
            row.Add(qty);
            row.Add(badge);
            return row;
        }
    }
}
