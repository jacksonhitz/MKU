using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityUtils;

public class InteractionManager : Singleton<InteractionManager>
{
    [SerializeField, Scene(Flag.Optional | Flag.IncludeInactive)]
    private List<Item> items;

    [SerializeField, Scene(Flag.Optional | Flag.IncludeInactive)]
    private List<Interactable> interactables;

    public bool isHighlightAll;

    public void ExtractOn()
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.type == Interactable.Type.Extract)
                interactable.isInteractable = true;
            else if (interactable.type == Interactable.Type.Enemy)
                interactable.isInteractable = false;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.ValidateRefs();
    }
}
