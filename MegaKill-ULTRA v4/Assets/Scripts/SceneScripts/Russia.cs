using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Russia : ScenesManager
{
    void Awake()
    {
        Instance = this;
        if (SceneManager.GetActiveScene().name != "SABLE")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.SABLE, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.SABLE);
    }
    void Start()
    {
        SoundManager.Instance.Play("4L");
    }
}
