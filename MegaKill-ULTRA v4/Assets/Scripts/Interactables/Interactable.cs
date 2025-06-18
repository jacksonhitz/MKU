using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public static event Action<(Type type, Interactable interactable)> InteractableUsed;

    [SerializeField]
    private Material glow;

    [SerializeField]
    private Material defaultMaterial;

    private Renderer rend;

    [SerializeField]
    private Material[] randomMaterials;

    public bool isHovering;
    public bool isInteractable = true;
    public bool useRandomMaterial;

    // managers
    protected PlayerController player;
    protected SoundManager sound;
    protected InteractionManager interacts;

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
    protected virtual void Start()
    {
        player = PlayerController.Instance;
        sound = SoundManager.Instance;
        interacts = InteractionManager.Instance;
    }

    protected virtual void LateUpdate()
    {
        if (!StateManager.IsActive || InteractionManager.Instance == null || rend == null)
            return;

        // Ff ishovering or isHighlightAll, glow, otherwise set to default material
        rend.material = ((isHovering && isInteractable) || InteractionManager.Instance.isHighlightAll)
            ? glow
            : defaultMaterial;
    }

    Renderer GetRenderer()
    {
        // Search all Renderer components in children
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
            throw new Exception($"No Renderer found in GameObject '{gameObject.name}' or its children.");

        return renderers[0]; // Use the first valid renderer found
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
