using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        if (SceneManager.GetActiveScene().name != "TITLE")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.TITLE, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.TITLE);
    }

    protected override void Start()
    {
        base.Start();
        SoundManager.Instance.Play("Title");
    }
}
