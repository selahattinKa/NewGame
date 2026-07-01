using System;

namespace CanavarZindanlari.Core
{
    public enum GameScreen { Hub, ClassSelect, Dungeon, DungeonCollection, Arena, PetSelect, Shop, Equipment }

    /// <summary>
    /// Hangi ekranın aktif olduğunu yöneten merkezi durum makinesi.
    /// GoBack() ile bir önceki ekrana dönülür.
    /// </summary>
    public static class ScreenNavigator
    {
        public static GameScreen Current  { get; private set; } = GameScreen.Hub;
        public static GameScreen Previous { get; private set; } = GameScreen.Hub;

        public static event Action<GameScreen> OnScreenChanged;

        public static void GoTo(GameScreen screen)
        {
            Previous = Current;
            Current  = screen;
            OnScreenChanged?.Invoke(Current);
        }

        public static void GoBack()   => GoTo(Previous);
        public static void GoToHub()  => GoTo(GameScreen.Hub);
    }
}
