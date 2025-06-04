using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spearhead : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();
        StateManager.lvl = StateManager.GameState.SPEARHEAD;
        if (StateManager.State != StateManager.GameState.FILE)
            StateManager.StartLvl();
    }
    void Start()
    {
        SoundManager.Instance.Play("DJ");
    }
}
