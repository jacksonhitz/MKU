using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] Material glow;
    [SerializeField] Material def;
    [SerializeField] Renderer rend;

    [SerializeField] Material[] mats;

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
        Enemy
    }
    public Type type;

    //CONVERT TO ABSTRACT AND REFACTOR

    public virtual void Interact()
    {
        if (isInteractable)
        {
            isInteractable = false;
            if (type == Type.Door)
            {

            }
            else if (type == Type.Extract) StateManager.LoadNext();
            else if (type == Type.Enemy)
            {
                EnemyPunch enemy = GetComponent<EnemyPunch>();
                enemy.dosed = true;
                ScenesManager.Instance.Interact();
                sound.Play("Interact");
            }
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

        if (!StateManager.IsActive() || interacts == null || rend == null)
            return;

        if (StateManager.IsActive())
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
        if (renderer == null) renderer = GetComponent<Renderer>();
        if (renderer == null) renderer = GetComponentInChildren<Renderer>();
        if (renderer == null) renderer = transform.root.GetComponentInChildren<Renderer>();
        return renderer;
    }
}
