using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour
{
    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    InteractionManager interacts;

    protected virtual void Awake()
    {
        interacts = FindObjectOfType<InteractionManager>();
    }
    protected virtual void Start()
    {
        interacts?.Collect();
    }

    void OnEnable()
    {
        if (lvlType == WinCon.Elim) EnemyManager.OnDeath += ElimCheck;

        StateManager.OnStateChanged += StateChange;
    }

    void OnDisable()
    {
        if (lvlType == WinCon.Elim) EnemyManager.OnDeath -= ElimCheck;
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        
    }
    void ElimCheck(Enemy enemy)
    {
        if (EnemyManager.Instance.enemies.Count == 0) Next();
    }

    public void Next()
    {
        StateManager.NextState(this);
    }
}
