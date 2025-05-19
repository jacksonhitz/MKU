using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EditorTools
{
    public abstract class PlacementTool : EditorTool
    {
        public override bool IsAvailable()
        {
            return EditorWindow.HasOpenInstances<PlacementToolWindow>();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (Selection.activeGameObject == null)
                return;

            var go = Selection.activeGameObject;
            if (!go.TryGetComponent<Collider>(out var collider))
                return;

            Handles.color = Color.red;
            Handles.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }

        protected virtual RaycastHit? GetWorldRaycast()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Ground", "Default"));

            if (hit.collider == null)
            {
                return null;
            }

            return hit;
        }
    }

    public class PrefabPlacementTool : PlacementTool
    {
        public static Transform Parent { get; set; }
        public static PrefabPlacementData.PrefabEntry prefab;

        public override void OnToolGUI(EditorWindow window)
        {
            if (prefab == null || prefab.prefab == null)
            {
                return;
            }
            if (
                Event.current.type != EventType.MouseDown
                || Event.current.button != 0
                || Event.current.modifiers != EventModifiers.Alt
            )
            {
                return;
            }
            var isHit = GetWorldRaycast();

            if (!isHit.HasValue)
            {
                return;
            }

            var hit = isHit.Value;
            var pd = prefab.placementData;

            Quaternion rotation = Quaternion.Slerp(
                Quaternion.identity,
                Quaternion.LookRotation(hit.normal, Vector3.up),
                pd.normalAligned
            );
            rotation.eulerAngles = new(
                rotation.eulerAngles.x + pd.rotationOffset.x,
                rotation.eulerAngles.y + pd.rotationOffset.y,
                rotation.eulerAngles.z + pd.rotationOffset.z
            );
            rotation.eulerAngles = new(
                rotation.eulerAngles.x
                    + Random.Range(-pd.rotationRandomness.x, pd.rotationRandomness.x),
                rotation.eulerAngles.y
                    + Random.Range(-pd.rotationRandomness.y, pd.rotationRandomness.y),
                rotation.eulerAngles.z
                    + Random.Range(-pd.rotationRandomness.z, pd.rotationRandomness.z)
            );
            if (pd.randomFlipX && Random.value > 0.5f)
            {
                rotation.eulerAngles = new(
                    rotation.eulerAngles.x + 180,
                    rotation.eulerAngles.y,
                    rotation.eulerAngles.z
                );
            }

            if (pd.randomFlipY && Random.value > 0.5f)
            {
                rotation.eulerAngles = new(
                    rotation.eulerAngles.x,
                    rotation.eulerAngles.y + 180,
                    rotation.eulerAngles.z
                );
            }

            if (pd.randomFlipZ && Random.value > 0.5f)
            {
                rotation.eulerAngles = new(
                    rotation.eulerAngles.x,
                    rotation.eulerAngles.y,
                    rotation.eulerAngles.z + 180
                );
            }
            var go = Instantiate(prefab.prefab, hit.point, rotation);
            Undo.RegisterCreatedObjectUndo(go, "Placement Tool: Place Object");
            go.transform.localScale =
                Vector3.one * Random.Range(pd.scaleRandomnessMin, pd.scaleRandomnessMax);
            go.transform.parent = Parent;
            RaiseOutOfOverlap(go, LayerMask.GetMask("Ground", "Default"));
        }

        public static void RaiseOutOfOverlap(GameObject go, LayerMask mask, float step = 0.001f)
        {
            if (!go.TryGetComponent<Collider>(out var col) || !col.enabled)
                return;

            // 1) Compute “average” radius from the AABB extents:
            var b = col.bounds;
            float avgRadius = (b.extents.x + b.extents.y + b.extents.z);

            // 2) Grab all colliders we’re overlapping right now:
            Collider[] initialHits = Physics.OverlapSphere(
                b.center,
                avgRadius,
                mask,
                QueryTriggerInteraction.Ignore
            );
            if (initialHits.Length == 0)
            {
                Debug.Log("No colliders found");
                return; // nothing to depenetrate
            }

            // 3) We’ll allow moving up at most half the object’s height (bounds.extents.y):
            float maxMove = b.extents.y;
            Vector3 originalPos = go.transform.position;

            // 4) Step upward until none of those “initialHits” still penetrate:
            for (float offset = 0f; offset <= maxMove; offset += step)
            {
                bool stillPenetrating = false;

                foreach (var other in initialHits)
                {
                    if (other == col || !other.enabled)
                        continue;

                    // Test penetration at (originalPos + offsetup)
                    Ray ray = new Ray(b.center + Vector3.up * offset, Vector3.down);
                    if (other.Raycast(ray, out var hit, b.extents.y * 0.75f))
                    {
                        Debug.DrawLine(
                            ray.origin + offset * Vector3.forward,
                            hit.point,
                            Color.green,
                            5f
                        );
                        stillPenetrating = true;
                        break;
                    }
                }

                if (!stillPenetrating)
                {
                    // Found a clear position—apply it and return.
                    go.transform.position = originalPos + Vector3.up * offset;
                    return;
                }
            }

            // 5) If we reach here, we never found a “no‐overlap” spot before maxMove:
            go.transform.position = originalPos + Vector3.up * maxMove;
        }
    }

    public class EnemyPlacementTool : PlacementTool
    {
        public static Enemy EnemyType { get; set; }
    }

    public class NotePlacementTool : PlacementTool { }

    public class ContainerPlacementTool : PlacementTool { }

    public class ItemPlacementTool : PlacementTool { }
}
