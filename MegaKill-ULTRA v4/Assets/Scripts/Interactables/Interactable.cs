using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Interactable : MonoBehaviour, IInteractable
{
    public Renderer rend;
    public Material def;
    [SerializeField] Material glow;

    PlayerController player;

    [HideInInspector] protected SoundManager sound;

    public bool isHovering;
    public bool isInteractable;

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
        if (type == Type.Door)
        {


        }
        else if (type == Type.Extract && StateManager.State == StateManager.GameState.TANGO2) StateManager.NextState(this);
    }

    public virtual void Awake()
    {
        rend = GetComponent<Renderer>();
        def = rend.materials[0];
    }

    void OnMouseEnter()
    {
        isHovering = true;
        GlowMat();
    }

    void OnMouseExit()
    {
        isHovering = false;
        DefaultMat();
    }

    public void DefaultMat()
    {
        rend.material = def;
    }

    public void GlowMat()
    {
        rend.material = glow;
    }
}
