using System;
using UnityEngine;

namespace AI.Sensors
{
    [RequireComponent(typeof(PerceptionSystem))]
    public class Proximity : MonoBehaviour, ISensor
    {
        public bool SenseActive { get; set; } = true;

        [SerializeField]
        private float range;

        public bool IsDetecting(DetectableTarget target)
        {
            if (!SenseActive)
                return false;
            return Vector3.Distance(target.transform.position, transform.position) <= range;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
