using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public static event Action<(Type type, Interactable interactable)> InteractableUsed;

    [SerializeField]
    Material glow;

    [SerializeField]
    Material defaultMaterial;

    [SerializeField]
    Renderer rend;

    [SerializeField]
    Material[] randomMaterials;

    public bool isHovering;
    public bool isInteractable = true;
    public bool useRandomMaterial;

    public enum Type
    {
        Door,
        Item,
        Extract,
        Enemy,
    }

    public abstract Type InteractableType { get; }

    protected virtual void Awake()
    {
        // if rend is null, getrend
        rend ??= GetRenderer();

        if (useRandomMaterial && randomMaterials != null && randomMaterials.Length > 0)
            defaultMaterial = randomMaterials[Random.Range(0, randomMaterials.Length)];
        else if (defaultMaterial == null)
            defaultMaterial = rend.material;
    }

    protected virtual void LateUpdate()
    {
        if (!StateManager.IsActive || InteractionManager.Instance == null || rend == null)
            return;

        // if ishovering or isHighlightAll, glow, otherwise set to default material
        rend.material = (isHovering || InteractionManager.Instance.isHighlightAll)
            ? glow
            : defaultMaterial;
    }
    
    Renderer GetRenderer()
    {
        Renderer renderer = GetComponentInParent<Renderer>();
        if (renderer == null)
            renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();
        return renderer;
    }

    public void Interact()
    {
        if (!isInteractable)
            return;

        isInteractable = false;
        InteractableUsed?.Invoke((InteractableType, this));
        OnInteract();
    }

     protected abstract void OnInteract();
}
