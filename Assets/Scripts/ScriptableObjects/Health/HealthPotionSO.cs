using UnityEngine;


namespace JuanIsometric2D.Objects
{
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "ScriptableObjects/Items/HealthPotion")]
    public class HealthPotionSO : ScriptableObject
    {
        [Header("Healing Settings")]
        [SerializeField] float healingAmount = 25f;
        [SerializeField] bool isPercentageBased = false;
        public float HealingAmount => healingAmount;
        public bool IsPercentageBased => isPercentageBased;


        [Header("Visual Settings")]
        [SerializeField] Sprite potionSprite;
        public Sprite PotionSprite => potionSprite;
    }
}