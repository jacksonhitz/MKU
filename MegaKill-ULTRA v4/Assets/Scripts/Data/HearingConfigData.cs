using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "HearingConfig", menuName = "AI/HearingConfig")]
    public class hearingConfigData : ScriptableObject
    {
        [field: SerializeField]
        public float HearingRange { get; private set; }
    }
}
