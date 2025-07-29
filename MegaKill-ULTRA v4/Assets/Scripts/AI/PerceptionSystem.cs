using AI.Sensors;
using KBCore.Refs;
using NaughtyAttributes;
using UnityEngine;

namespace AI
{
    public class PerceptionSystem : ValidatedMonoBehaviour
    {
        [Self(Flag.Optional)]
        [ShowIf(nameof(hearingSet))]
        [SerializeField]
        private Hearing hearing;

        [Self(Flag.Optional)]
        [ShowIf(nameof(proximitySet))]
        [SerializeField]
        private Proximity proximity;

        [Self(Flag.Optional)]
        [ShowIf(nameof(visionSet))]
        [SerializeField]
        private Vision vision;

        public bool HasHearing => hearingSet;
        public bool HasProximity => proximitySet;
        public bool HasVision => visionSet;

        public bool CanDetect(DetectableTarget target) =>
            HasVision && vision.IsDetecting(target)
            || HasHearing && hearing.IsDetecting(target)
            || HasProximity && proximity.IsDetecting(target);

        #region Validation
        private bool hearingSet => hearing != null;
        private bool proximitySet => proximity != null;
        private bool visionSet => vision != null;

        [HideIf(nameof(hearingSet))]
        [Button("Add Hearing Sense")]
        private void addHearing()
        {
            gameObject.AddComponent<Hearing>();
            this.ValidateRefs();
        }

        [HideIf(nameof(visionSet))]
        [Button("Add Vision Sense")]
        private void addVision()
        {
            gameObject.AddComponent<Vision>();
            this.ValidateRefs();
        }

        [HideIf(nameof(proximitySet))]
        [Button("Add Proximity Sense")]
        private void addProximity()
        {
            gameObject.AddComponent<Proximity>();
            this.ValidateRefs();
        }
        #endregion
    }
}
