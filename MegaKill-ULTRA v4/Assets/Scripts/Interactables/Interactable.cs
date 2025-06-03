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


    //THIS ENTIRE SYSTEM IS STUPID AND FUCKED BUT I DONT HAVE TIME TO FIX IT. MAKE INTERACT A GLOBAL INTERFACE W A LISTENER SYSTEM FUTURE ME
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
                ScenesManager.Instance.Interact(); //???????? FUCK

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
        if (isHovering || interacts.isHighlightAll)
            rend.material = glow;
        else
            rend.material = def;

    }


    //ATTENTION FUTURE ME: THIS WILL BREAK IF A MODEL IS USING MULTIPLE SEPERATE MATS FOR A SINGLE TEXTURE
    void GetMat()
    {
        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<SkinnedMeshRenderer>();
        if (rend == null) rend = GetComponentInChildren<MeshRenderer>();

        var mats = rend.materials;
        if (mats.Length > 0)
        {
            def = mats[Random.Range(0, mats.Length)];
            rend.material = def;
        }

        Debug.Log("rend: " + rend);
        Debug.Log("mat: " + def);
    }

}
