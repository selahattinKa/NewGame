using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Prototype: Play'e basınca savaşı otomatik başlatır.
    /// Gerçek oyunda sınıf seçim ekranı + zindan sistemi bu görevi üstlenir.
    /// </summary>
    public class CombatBootstrap : MonoBehaviour
    {
        [Header("Sınıf Seçimi (boşsa Savaşçı yüklenir)")]
        [SerializeField] private ClassData _playerClass;

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

            // ClassData yoksa Resources'tan yüklemeyi dene
            if (_playerClass == null)
                _playerClass = Resources.Load<ClassData>("Classes/Savasco");

            var player = BuildPlayer();
            var enemy  = BuildEnemy();
            _combat.StartCombat(player, enemy, _playerClass);
        }

        private CombatUnit BuildPlayer()
        {
            if (_playerClass != null)
            {
                return new CombatUnit
                {
                    DisplayName = _playerClass.ClassName,
                    Archetype   = Archetype.Saldirgan,
                    MaxHP       = _playerClass.StatAtLevel(_playerClass.BaseHP,  1),
                    CurrentHP   = _playerClass.StatAtLevel(_playerClass.BaseHP,  1),
                    BaseATK     = _playerClass.StatAtLevel(_playerClass.BaseATK, 1),
                    BaseDEF     = _playerClass.StatAtLevel(_playerClass.BaseDEF, 1),
                    SPD         = _playerClass.StatAtLevel(_playerClass.BaseSPD, 1),
                };
            }

            // Fallback — ClassData henüz atanmamış
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
