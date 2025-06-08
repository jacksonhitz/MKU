using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [SerializeField] List<Item> items;
    public List<Interactable> interactables;

    public bool isHighlightAll;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        items = new List<Item>();
        interactables = new List<Interactable>();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;

        StateChange(StateManager.State);
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        Debug.Log("INTERACT CALLED");

        if (StateManager.IsActive())
        {
            Collect();
            Debug.Log("COLLECTING");
        } 
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
}



