using UnityEngine;
using System;

public class Interactable : MonoBehaviour, IInteractable
{
    public Renderer rend;
    public Material def;
    [SerializeField] Material glow;

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
                Debug.Log("talked to" + enemy);
                Tango.Instance.Dosed(enemy);
                SoundManager.Instance.Talk();
            }
        }
        
    }

    public virtual void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<Renderer>();
        def = rend.materials[0];
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
        rend.material = def;
    }

    public void GlowMat()
    {
        rend.material = glow;
    }
}
