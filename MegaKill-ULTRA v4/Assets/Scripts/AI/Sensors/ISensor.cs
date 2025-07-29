using UnityEngine;

namespace AI.Sensors
{
    public interface ISensor
    {
        bool SenseActive { get; set; }
        public bool IsDetecting(DetectableTarget target);
    }
}
