using UnityEngine;

namespace CanavarZindanlari.Data
{
    [System.Serializable]
    public struct LootDrop
    {
        public string ItemName;
        public int Quantity;
        public Rarity Rarity;
        public Sprite Icon; // null = placeholder gösterilir
    }
}
