using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [SerializeField] List<Item> items;
    [SerializeField] List<Interactable> interactables;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        items = new List<Item>();
        interactables = new List<Interactable>();

        Collect();
    }

    public void Collect()
    {
        items.Clear();
        items.AddRange(FindObjectsOfType<Item>());

        interactables.Clear();
        interactables.AddRange(FindObjectsOfType<Interactable>());
    }

    public void ExtractOn()
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.type == Interactable.Type.Extract) interactable.isInteractable = true;
            else if (interactable.type == Interactable.Type.Enemy) interactable.isInteractable = false;
        }
    }

    public void HighlightAll()
    {
        foreach (Interactable interactable in interactables)
        {
            interactable.GlowMat();
        }
    }
    public void HighlightOne()
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.isHovering && interactable.isInteractable)
            {
                interactable.GlowMat();
            }
            else
            {
                interactable.DefaultMat();
            }
        }
    }
}



