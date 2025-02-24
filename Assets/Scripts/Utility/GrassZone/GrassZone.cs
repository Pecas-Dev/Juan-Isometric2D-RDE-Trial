using UnityEngine;


namespace JuanIsometric2D
{
    public class GrassZone : MonoBehaviour
    {
        [Header("Grass Zone Scriptable Object")]
        [SerializeField] GrassZoneSO grassZoneSO;
        public GrassZoneSO GrassZoneSO => grassZoneSO;
    }
}
