using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rehearsal : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();
        StateManager.lvl = StateManager.GameState.REHEARSAL;
        if (StateManager.State != StateManager.GameState.FILE)
            StateManager.StartLvl();
    }
}
