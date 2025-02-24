using JuanIsometric2D.Combat;

using UnityEngine;


namespace JuanIsometric2D.Objects
{
    public class HealthPotion : MonoBehaviour
    {
        [Header("Potion Settings")]
        [SerializeField] HealthPotionSO potionData;

        void Awake()
        {
            InitializePotion();
        }

        void InitializePotion()
        {
            if (potionData == null)
            {
                Debug.LogError($"No HealthPotionSO assigned to {gameObject.name}!");
                return;
            }
        }

        public float GetHealAmount()
        {
            return potionData.HealingAmount;
        }

        public bool IsPercentageBased()
        {
            return potionData.IsPercentageBased;
        }
    }
}