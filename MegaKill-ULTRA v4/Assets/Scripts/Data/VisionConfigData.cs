using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "VisionConfig", menuName = "AI/VisionConfig")]
    public class VisionConfigData : ScriptableObject
    {
        [field: SerializeField]
        public float Range { get; private set; }

        [field: SerializeField]
        public float FOVAngle { get; private set; }

        [field: SerializeField]
        public Vector3 AngleOffset { get; private set; }
    }
}
