using System;

namespace CanavarZindanlari.Core
{
    public enum GameScreen { Hub, Dungeon, Arena, PetSelect }

    /// <summary>
    /// Hangi ekranın aktif olduğunu yöneten merkezi durum makinesi.
    /// Tüm IMGUI bileşenleri OnGUI başında Current'ı kontrol eder.
    /// </summary>
    public static class ScreenNavigator
    {
        public static GameScreen Current { get; private set; } = GameScreen.Hub;
        public static event Action<GameScreen> OnScreenChanged;

        public static void GoTo(GameScreen screen)
        {
            Current = screen;
            OnScreenChanged?.Invoke(screen);
        }

        public static void GoToHub() => GoTo(GameScreen.Hub);
    }
}
