using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance { get; set; }

    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    protected virtual void Start()
    {
        InteractionManager.Instance?.Collect();
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
}
