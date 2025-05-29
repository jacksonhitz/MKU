using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : ScenesManager
{
    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "TITLE")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.TITLE, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.TITLE);
    }
}
