using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : ScenesManager
{
    void Awake()
    {
        Instance = this;

            StateManager.LoadSilent(StateManager.GameState.TITLE);
    }

    void Start()
    {
        SoundManager.Instance.Play("Title");
    }
}
