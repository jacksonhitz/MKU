using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public void StartGame()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.TUTORIAL, 1f, destroyCancellationToken);
    }
}
