using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Prototype: savaşı otomatik başlatır. Play'e basınca savaş anında hazır.
    /// Gerçek oyunda bu, seçilen düşman + oyuncu canavara göre dışarıdan kurulacak.
    /// </summary>
    public class CombatBootstrap : MonoBehaviour
    {
        [Header("Manuel Override (boş bırakırsan test değerleri kullanılır)")]
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

            _combat.StartCombat(BuildPlayer(), BuildEnemy());
        }

        private CombatUnit BuildPlayer()
        {
            return new CombatUnit
            {
                DisplayName = "Oyuncu",
                Archetype   = Archetype.Saldirgan,
                MaxHP       = 80,
                CurrentHP   = 80,
                ATK         = 18,
                DEF         = 22,
                SPD         = 40,
            };
        }

        private CombatUnit BuildEnemy()
        {
            if (_enemyData != null)
            {
                // Test için Lv5 kullan — daha uzun savaş
                const int testLevel = 5;
                return new CombatUnit
                {
                    DisplayName = _enemyData.DisplayName,
                    Element     = _enemyData.Element,
                    Archetype   = _enemyData.Archetype,
                    MaxHP       = _enemyData.StatAtLevel(_enemyData.BaseHP,  testLevel),
                    CurrentHP   = _enemyData.StatAtLevel(_enemyData.BaseHP,  testLevel),
                    ATK         = _enemyData.StatAtLevel(_enemyData.BaseATK, testLevel),
                    DEF         = _enemyData.StatAtLevel(_enemyData.BaseDEF, testLevel),
                    SPD         = _enemyData.StatAtLevel(_enemyData.BaseSPD, testLevel),
                };
            }

            // Player hasar: 18 - 9 = ~9/tur → 50 HP ~6 tur
            // Enemy hasar: 20 - 11 = ~9/tur → 80 HP ~9 tur
            return new CombatUnit
            {
                DisplayName = "Ateş Goblin",
                Archetype   = Archetype.Saldirgan,
                MaxHP       = 50,
                CurrentHP   = 50,
                ATK         = 20,
                DEF         = 18,
                SPD         = 25,
            };
        }
    }
}
