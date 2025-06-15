using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : SceneScript
{
    protected override void Awake()
    {
        StateManager.State = StateManager.GameState.TITLE;
        Instance = this;
    }

    private new void Start()
    {
        base.Start();
        StartLevel();
    }

    public override void StartLevel()
    {
        SoundManager.Instance.Play("Title");
        State = StateManager.SceneState.PAUSED;
    }
}
