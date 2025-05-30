using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] Material[] mats;
    [SerializeField] Material glow;

    Material mat;
    Renderer rend;
    
    public bool isHovering;
    public bool isInteractable;

    protected PlayerController player;
    protected SoundManager sound;

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
                Tango.Instance.Dosed(enemy);
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
    }
    
    void GetMat()
    {
        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<Renderer>();


        if (mats.Length > 0)
        {
            mat = mats[Random.Range(0, mats.Length)];
            rend.material = mat;
        }
        else mat = rend.material;

        Debug.Log("rend" + rend);
        Debug.Log("mat" + mat);
    }

    void OnMouseEnter()
    {
        if (isInteractable)
        {
            isHovering = true;
            GlowMat();
        }
    }

    void OnMouseExit()
    {
        isHovering = false;
        DefaultMat();
    }

    public void DefaultMat()
    {
        if (this != null) rend.material = mat;

    }

    public void GlowMat()
    {
        if (this != null) rend.material = glow;
    }
}
