using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rehearsal : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();

        if (SceneManager.GetActiveScene().name != "REHEARSAL")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.REHEARSAL, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.REHEARSAL);
    }
}
