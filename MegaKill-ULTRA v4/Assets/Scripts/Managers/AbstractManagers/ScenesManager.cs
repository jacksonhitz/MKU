using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour
{
    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    protected virtual void Start()
    {
        InteractionManager.Instance?.Collect();
    }

    void OnEnable()
    {
        if (lvlType == WinCon.Elim) EnemyManager.OnEnemyDeath += ElimCheck;

        StateManager.OnStateChanged += StateChange;
    }

    void OnDisable()
    {
        if (lvlType == WinCon.Elim) EnemyManager.OnEnemyDeath -= ElimCheck;
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        
    }
    void ElimCheck(Enemy enemy)
    {
        if (EnemyManager.Instance.enemies.Count == 0) Next();
        Debug.Log("enemy killed");
    }

    public void Next()
    {
        StateManager.NextState(this);
    }
}
