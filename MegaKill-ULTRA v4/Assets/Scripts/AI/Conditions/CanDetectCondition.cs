using System;
using AI;
using NaughtyAttributes;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "Can Detect",
    story: "[Agent] can detect [target] via [Sense]",
    category: "Conditions",
    id: "30f67f515a4530d60e4c9a373c58b22a"
)]
public partial class CanDetectCondition : Condition
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Agent;

    [SerializeReference]
    public BlackboardVariable<GameObject> Target;

    [SerializeReference]
    public BlackboardVariable<DetectionType> Sense;

    private PerceptionSystem perception;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
        if (Agent.Value == null)
        {
            Agent.Value = GameObject;
        }

        if (Agent.Value?.TryGetComponent(out perception) == false)
        {
            Debug.LogError("Agent has no perception system");
        }
    }

    public override void OnEnd() { }

    [BlackboardEnum]
    public enum DetectionType
    {
        Proximity,
        Vision,
        Hearing,
    }
}
