using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Russia : ScenesManager
{
    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "SABLE")
        {
            SoundManager.Instance?.Play("4L");
            StartCoroutine(StateManager.LoadState(StateManager.GameState.SABLE, 0f));
        }
        else StateManager.LoadSilent(StateManager.GameState.SABLE);
    }
}
