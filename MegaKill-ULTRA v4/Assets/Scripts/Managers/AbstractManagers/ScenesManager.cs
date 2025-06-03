using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour, IInteractable
{
    public static ScenesManager Instance { get; set; }

    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    public bool isLvlOn;

    protected virtual void Awake()
    {
        isLvlOn = false;
    }
    protected virtual void Start()
    {

    }


    protected virtual void LvlOn()
    {
        if (!isLvlOn)
        {
            isLvlOn = true;
            InteractionManager.Instance?.Collect();
            Debug.Log("Scene: " + Instance);
        }
    }

    public void ElimCheck()
    {
        if (EnemyManager.Instance?.enemies.Count == 0) Next();
        Debug.Log("enemy killed");
    }

    public void Next()
    {
        StateManager.NextState(this);
    }

    public virtual void Interact() { }
}
