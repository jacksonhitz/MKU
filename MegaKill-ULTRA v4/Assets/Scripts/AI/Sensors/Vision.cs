using System;
using System.Collections.Generic;
using Data;
using NaughtyAttributes;
using UnityEngine;

namespace AI.Sensors
{
    [RequireComponent(typeof(PerceptionSystem))]
    public class Vision : MonoBehaviour, ISensor
    {
        private const int MaxVisibleEntities = 50;

        [SerializeField]
        private LayerMask detectionMask = 0;

        [Required]
        [Expandable]
        [SerializeField]
        private VisionConfigData visionConfig;

        [Required]
        [SerializeField]
        private Transform eyes;

        private readonly List<DetectableTarget> detectedTargets = new();
        private bool hasCachedResults = false;

        public bool SenseActive { get; set; } = true;

        public void FixedUpdate()
        {
            if (hasCachedResults)
                hasCachedResults = false;
        }

        public bool IsDetecting(DetectableTarget target)
        {
            if (!SenseActive)
                return false;
            Sense();
            return detectedTargets.Contains(target);
        }

        private void Sense()
        {
            if (hasCachedResults)
                return;
            detectedTargets.Clear();
            hasCachedResults = true;
            var results = new Collider[MaxVisibleEntities];
            Physics.OverlapSphereNonAlloc(
                transform.position,
                visionConfig.Range,
                results,
                detectionMask
            );
            foreach (var collider in results)
            {
                var dirToTarget = collider.transform.position - eyes.position;
                if (Vector3.Dot(dirToTarget, eyes.forward) < visionConfig.FOVAngle / 180f)
                    continue;
                var pos = collider.transform.position;
                if (Vector3.Distance(pos, eyes.position) > visionConfig.Range)
                    continue;

                Ray ray = new Ray(eyes.position, dirToTarget);
                if (
                    !Physics.Raycast(
                        ray,
                        visionConfig.Range,
                        detectionMask,
                        QueryTriggerInteraction.Collide
                    )
                )
                {
                    continue;
                }

                if (collider.TryGetComponent<DetectableTarget>(out var target))
                {
                    detectedTargets.Add(target);
                }
            }
        }

        private void OnDisable()
        {
            detectedTargets.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            if (!eyes)
                return;

            DrawFOVArc();
            Gizmos.color = Color.white;
            foreach (var target in detectedTargets)
            {
                Gizmos.DrawWireSphere(target.transform.position, 0.2f);
            }
        }

        // Claude Sonnet 3.7
        private void DrawFOVArc()
        {
            // Draw the FOV arc
            Vector3 position = eyes.position;
            Vector3 direction = eyes.forward;

            // Calculate the FOV angles
            float halfFOV = visionConfig.FOVAngle / 2f;
            Vector3 leftRayDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * direction;
            Vector3 rightRayDirection = Quaternion.AngleAxis(halfFOV, Vector3.up) * direction;

            // Draw the FOV boundary rays
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position, leftRayDirection * visionConfig.Range);
            Gizmos.DrawRay(position, rightRayDirection * visionConfig.Range);

            // Draw the FOV arc with line segments
            int segments = 20;
            Vector3 prevPos = position + (leftRayDirection * visionConfig.Range);

            // Semi-transparent color for the FOV area
            Gizmos.color = new Color(1f, 1f, 0f, 0.1f);

            // Draw the arc segments
            for (int i = 0; i <= segments; i++)
            {
                float angle = -halfFOV + (visionConfig.FOVAngle * i / segments);
                Vector3 arcSegmentDirection = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                Vector3 newPos = position + (arcSegmentDirection * visionConfig.Range);

                // Draw segment line
                Gizmos.DrawLine(prevPos, newPos);

                // Draw line back to origin to create a "pie slice" effect
                Gizmos.DrawLine(position, newPos);

                prevPos = newPos;
            }
        }
    }
}
