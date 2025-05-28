using UnityEngine;
using System;

[RequireComponent(typeof(Renderer))]
public class Interactable : MonoBehaviour, IInteractable
{
    public Renderer rend;
    public SkinnedMeshRenderer skinned;
    public Material def;
    [SerializeField] Material glow;

    public bool isHovering;
    public bool isInteractable;

    public event Action talk;

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
            else if (type == Type.Extract && StateManager.State == StateManager.GameState.TANGO2) StateManager.NextState(this);
            else if (type == Type.Enemy)
            {
                SoundManager.Instance.Talk();



            }
        }
        
    }

    public virtual void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) def = rend.materials[0];
        else
        {
            skinned = GetComponent<SkinnedMeshRenderer>();
            def = skinned.materials[0];
        }
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
