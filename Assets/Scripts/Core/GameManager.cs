using UnityEngine;
using CanavarZindanlari.Economy;

namespace CanavarZindanlari.Core
{
    /// <summary>
    /// Single entry point. Holds references to all manager singletons.
    /// Survives scene loads via DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        [SerializeField] private EconomyManager _economy;

        public EconomyManager Economy => _economy;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Application.targetFrameRate = 60;
        }
    }
}
