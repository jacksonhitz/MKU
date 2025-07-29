using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class DetectableTarget : MonoBehaviour
    {
        [ResetOnPlay]
        public static List<DetectableTarget> DetectableTargets { get; private set; }

        public void Awake()
        {
            DetectableTargets ??= new();
            DetectableTargets.Add(this);
        }

        public void OnDestroy()
        {
            DetectableTargets.Remove(this);
        }
    }
}
