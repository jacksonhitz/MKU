using System;
using UnityEngine;

namespace AI.Sensors
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(PerceptionSystem))]
    public class Hearing : MonoBehaviour, ISensor
    {
        public bool SenseActive { get; set; } = true;

        public bool IsDetecting(DetectableTarget target)
        {
            return false;
        }
    }
}
