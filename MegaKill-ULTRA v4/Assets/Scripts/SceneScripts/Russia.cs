using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Russia : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();
        StateManager.lvl = StateManager.GameState.SABLE;
        if (StateManager.State != StateManager.GameState.FILE)
            StateManager.StartLvl();
    }
}
