using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Wander",
    story: "Agent walks toward random [point] within [distance] m",
    category: "Action",
    id: "cda3e61509d90c679513597e60dd3b4b"
)]
public partial class WanderAction : Action
{
    [SerializeReference]
    public BlackboardVariable<Transform> Point;

    [SerializeReference]
    public BlackboardVariable<float> Distance;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd() { }
}
