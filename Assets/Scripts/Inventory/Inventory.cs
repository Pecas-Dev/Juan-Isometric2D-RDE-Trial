using JuanIsometric2D.Combat;
using JuanIsometric2D.Objects;

using System;
using UnityEngine;
using System.Collections.Generic;



namespace JuanIsometric2D.InventorySystem
{
    public class Inventory
    {
        readonly List<InventoryItem> items = new();
        public IReadOnlyList<InventoryItem> Items => items;


        public event EventHandler OnInventoryChanged;


        public void AddItem(Item item, int amount = 1)
        {
            if (item == null)
            {
                return;
            }

            if (item.IsStackable)
            {
                var existingItem = items.Find(i => i.ItemType == item.Type);

                if (existingItem != null && existingItem.CanAddToStack(amount))
                {
                    existingItem.AddToStack(amount);

                    OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }

            items.Add(new InventoryItem(item, amount));
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool RemoveItem(Item.ItemType itemType, int amount = 1)
        {
            var item = items.Find(i => i.ItemType == itemType);

            if (item == null || item.Amount < amount)
            {
                return false;
            }

            item.RemoveFromStack(amount);

            if (item.Amount <= 0)
            {
                items.Remove(item);
            }

            OnInventoryChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public InventoryItem GetItem(Item.ItemType itemType)
        {
            return items.Find(i => i.ItemType == itemType);
        }

        public bool UseHealthPotion(HealthSystem playerHealth)
        {
            var potion = items.Find(i => i.ItemType == Item.ItemType.HealthPotion);

            if (potion == null) 
            {
                return false;
            }

            if (playerHealth.GetHealthPercentage() >= 1f)
            {
                Debug.Log("Health is already full!");
                return false;
            }

            var potionComponent = GameObject.FindFirstObjectByType<HealthPotion>();

            if (potionComponent == null) 
            {
                return false;
            }

            float healAmount = potionComponent.GetHealAmount();

            if (potionComponent.IsPercentageBased())
            {
                healAmount = playerHealth.GetMaxHealth() * (healAmount / 100f);
            }

            playerHealth.Heal(healAmount, true);
            RemoveItem(Item.ItemType.HealthPotion, 1);

            return true;
        }
    }
}
