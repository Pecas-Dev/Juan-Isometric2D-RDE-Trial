using UnityEngine;


namespace JuanIsometric2D.InventorySystem
{
    public class Item : MonoBehaviour
    {
        public enum ItemType
        {
            HealthPotion,
            Coins,
            // Weapon 
        }

        [Header("Item Settings")]
        [SerializeField] ItemType itemType;
        [SerializeField] Sprite itemSprite;

        [SerializeField] bool isStackable = true;

        [SerializeField] int maxStackSize = 99;

        public ItemType Type => itemType;
        public Sprite ItemSprite => itemSprite;
        public bool IsStackable => isStackable;
        public int MaxStackSize => maxStackSize;


        void OnValidate()
        {
            if (itemSprite == null)
            {
                var spriteRenderer = GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    itemSprite = spriteRenderer.sprite;
                }
            }
        }
    }
}