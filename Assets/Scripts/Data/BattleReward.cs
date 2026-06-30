namespace CanavarZindanlari.Data
{
    [System.Serializable]
    public struct BattleReward
    {
        public bool IsVictory;
        public int ExpGained;
        public LootDrop[] Items;
        public bool IsFirstClear;
        public int BonusGems; // first-clear ikramiyesi
    }
}
