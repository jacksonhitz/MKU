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
        StateManager.OnStateChanged += OnStateChanged;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= OnStateChanged;
    }
    void OnStateChanged(StateManager.GameState state)
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



