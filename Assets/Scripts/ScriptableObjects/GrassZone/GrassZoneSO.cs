using UnityEngine;


namespace JuanIsometric2D
{
    [CreateAssetMenu(fileName = "GrassZoneValues", menuName = "ScriptableObjects/Utility/GrassZoneValues")]
    public class GrassZoneSO : ScriptableObject
    {
        [Header("Movement Settings")]
        [Range(0.1f, 1f)][SerializeField] float speedMultiplier = 0.5f;
        public float SpeedMultiplier => speedMultiplier;
    }
}

