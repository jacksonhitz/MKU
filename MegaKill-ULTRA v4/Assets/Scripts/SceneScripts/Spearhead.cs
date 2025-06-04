using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spearhead : ScenesManager
{
    void Awake()
    {
        Instance = this;
        if (SceneManager.GetActiveScene().name != "SPEARHEAD")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.SPEARHEAD, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.SPEARHEAD);
    }
    void Start()
    {
        SoundManager.Instance.Play("DJ");
    }
}
