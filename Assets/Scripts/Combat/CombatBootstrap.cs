using UnityEngine;
using CanavarZindanlari.Data;
using CanavarZindanlari.UI;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Sınıf seçim ekranını bekler, seçim sonrası savaşı başlatır.
    /// </summary>
    public class CombatBootstrap : MonoBehaviour
    {
        [Header("Düşman (boşsa varsayılan test düşmanı)")]
        [SerializeField] private MonsterData _enemyData;

        private CombatManager _combat;

        private void Start()
        {
            _combat = FindObjectOfType<CombatManager>();
            if (_combat == null)
            {
                Debug.LogError("[Bootstrap] Sahnede CombatManager bulunamadı.");
                return;
            }

            var selector = FindObjectOfType<ClassSelectionScreen>();
            if (selector != null)
            {
                selector.OnClassSelected += OnClassChosen;
            }
            else
            {
                // Sahnede seçim ekranı yoksa fallback olarak Savaşçı ile başlat
                var fallback = Resources.Load<ClassData>("Classes/Savasco");
                BeginCombat(fallback);
            }
        }

        private void OnClassChosen(ClassData classData)
        {
            BeginCombat(classData);
        }

        private void BeginCombat(ClassData classData)
        {
            var player = BuildPlayer(classData);
            var enemy  = BuildEnemy();
            _combat.StartCombat(player, enemy, classData);
        }

        private CombatUnit BuildPlayer(ClassData classData)
        {
            if (classData != null)
            {
                return new CombatUnit
                {
                    DisplayName = classData.ClassName,
                    Archetype   = Archetype.Saldirgan,
                    MaxHP       = classData.StatAtLevel(classData.BaseHP,  1),
                    CurrentHP   = classData.StatAtLevel(classData.BaseHP,  1),
                    BaseATK     = classData.StatAtLevel(classData.BaseATK, 1),
                    BaseDEF     = classData.StatAtLevel(classData.BaseDEF, 1),
                    SPD         = classData.StatAtLevel(classData.BaseSPD, 1),
                };
            }

            return new CombatUnit
            {
                DisplayName = "Oyuncu",
                Archetype   = Archetype.Saldirgan,
                MaxHP       = 55, CurrentHP = 55,
                BaseATK     = 18, BaseDEF   = 40, SPD = 20,
            };
        }

        private CombatUnit BuildEnemy()
        {
            if (_enemyData != null)
            {
                const int lv = 5;
                int hp = _enemyData.StatAtLevel(_enemyData.BaseHP, lv);
                return new CombatUnit
                {
                    DisplayName = _enemyData.DisplayName,
                    Archetype   = _enemyData.Archetype,
                    MaxHP       = hp, CurrentHP = hp,
                    BaseATK     = _enemyData.StatAtLevel(_enemyData.BaseATK, lv),
                    BaseDEF     = _enemyData.StatAtLevel(_enemyData.BaseDEF, lv),
                    SPD         = _enemyData.StatAtLevel(_enemyData.BaseSPD, lv),
                };
            }

            return new CombatUnit
            {
                DisplayName = "Ateş Goblin",
                Archetype   = Archetype.Saldirgan,
                MaxHP       = 50, CurrentHP = 50,
                BaseATK     = 20, BaseDEF   = 18, SPD = 25,
            };
        }
    }
}
