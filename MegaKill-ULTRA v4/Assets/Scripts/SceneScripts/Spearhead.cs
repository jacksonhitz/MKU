using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spearhead : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        if (SceneManager.GetActiveScene().name != "SPEARHEAD")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.SPEARHEAD, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.SPEARHEAD);
    }
    protected override void Start()
    {
        base.Start();
        SoundManager.Instance.Play("DJ");
    }
}
