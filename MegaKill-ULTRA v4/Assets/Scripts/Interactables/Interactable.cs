using System;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Interactable : MonoBehaviour, IInteractable
{
    //CONVERT TO ABSTRACT AND REFACTOR
    [ResetOnPlay]
    public static event Action<(Type type, Interactable interactable)> InteractableUsed;

    [SerializeField]
    Material glow;

    [SerializeField]
    Material def;

    [SerializeField]
    Renderer rend;

    [SerializeField]
    Material[] mats;

    public bool isHovering;
    public bool isInteractable;
    public bool isRandomTex;

    protected PlayerController player;
    protected SoundManager sound;
    protected EnemyManager enemies;
    protected InteractionManager interacts;

    public enum Type
    {
        Door,
        Item,
        Extract,
        Enemy,
    }

    public Type type;

    public virtual void Interact()
    {
        if (!isInteractable)
            return;
        isInteractable = false;
        InteractableUsed?.Invoke((type, this));
        if (type == Type.Enemy)
        {
            EnemyPunch enemy = GetComponent<EnemyPunch>();
            enemy.dosed = true;
            sound.Play("Interact");
        }
    }

    protected virtual void Awake()
    {
        if (rend == null)
            rend = GetRenderer();

        player = PlayerController.Instance;
        sound = SoundManager.Instance;
        enemies = EnemyManager.Instance;
        interacts = InteractionManager.Instance;

        if (isRandomTex && mats != null && mats.Length > 0)
        {
            def = mats[Random.Range(0, mats.Length)];
        }
        else if (def == null)
        {
            def = rend.material;
        }
    }

    protected virtual void Start()
    {
        player = PlayerController.Instance;
        sound = SoundManager.Instance;
        enemies = EnemyManager.Instance;
        interacts = InteractionManager.Instance;
    }

    void LateUpdate()
    {
        if (!StateManager.IsActive || interacts == null || rend == null)
            return;

        if (StateManager.IsActive)
        {
            if (isHovering || interacts.isHighlightAll)
                rend.material = glow;
            else
                rend.material = def;
        }
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
}
