using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] Material glow;
    Material def;
    Renderer rend;

    public bool isHovering;
    public bool isInteractable;

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

    public virtual void Interact()
    {
        if (isInteractable)
        {
            isInteractable = false;
            if (type == Type.Door)
            {

            }
            else if (type == Type.Extract) StateManager.NextState(this);
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
        GetMat();
    }

    protected virtual void Start()
    {
        player = PlayerController.Instance;
        sound = SoundManager.Instance;
        enemies = EnemyManager.Instance;
        interacts = InteractionManager.Instance;
    }

    void Update()
    {
        if (rend == null) return;

        if (isHovering || interacts.isHighlightAll)
        {
            if (rend.material != glow)
                rend.material = glow;
        }
        else
        {
            if (rend.material != def)
                rend.material = def;
        }
    }

    void GetMat()
    {
        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<SkinnedMeshRenderer>();
        if (rend == null) rend = GetComponentInChildren<MeshRenderer>();

        def = rend.material;

        Debug.Log("rend: " + rend);
        Debug.Log("mat: " + def);
    }
}
