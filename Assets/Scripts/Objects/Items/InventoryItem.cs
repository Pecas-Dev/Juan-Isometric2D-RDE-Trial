using System;
using UnityEngine;


namespace JuanIsometric2D.InventorySystem
{
    [Serializable]
    public class InventoryItem
    {
        public Item.ItemType ItemType { get; private set; }
        public int Amount { get; private set; }
        public Sprite ItemSprite { get; private set; }
        public bool IsStackable { get; private set; }
        public int MaxStackSize { get; private set; }


        public InventoryItem(Item sourceItem, int amount = 1)
        {
            ItemType = sourceItem.Type;
            ItemSprite = sourceItem.ItemSprite;
            IsStackable = sourceItem.IsStackable;
            MaxStackSize = sourceItem.MaxStackSize;
            Amount = amount;
        }

        public bool CanAddToStack(int amount)
        {
            return IsStackable && (Amount + amount <= MaxStackSize);
        }

        public void AddToStack(int amount)
        {
            if (IsStackable)
            {
                Amount = Mathf.Min(Amount + amount, MaxStackSize);
            }
        }

        public void RemoveFromStack(int amount)
        {
            if (IsStackable)
            {
                Amount = Mathf.Max(0, Amount - amount);
            }
        }
    }
}
