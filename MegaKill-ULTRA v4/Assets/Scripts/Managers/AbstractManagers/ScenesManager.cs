using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour, IInteractable
{
    public static ScenesManager Instance { get; set; }

    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    protected GameObject lvl;

    public virtual void Interact() { }
    protected virtual void Awake()
    {
        Instance = this;
        if (lvl != null)
            lvl = transform.GetChild(0).gameObject;
    }
    protected virtual void Update()
    {
        if (StateManager.IsPassive())
        {
            lvl?.SetActive(false);
        }
        if (StateManager.IsActive())
        {
            lvl?.SetActive(true);
          //  InteractionManager.Instance.Collect(); // this is here bc the listerner is broken
        }
    }
}
