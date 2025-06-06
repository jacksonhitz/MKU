using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rehearsal : ScenesManager
{
    protected override void Awake()
    {
        base.Awake();
        StateManager.lvl = StateManager.GameState.REHEARSAL;
        if (StateManager.State != StateManager.GameState.FILE)
            StateManager.StartLvl();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.SCORE: NewsDialogue(); break;
        }
    }

    void NewsDialogue()
    {
        Info.Instance.TypeText("We are just now receiving reports from the authorities that an underground USSR base has been discovered operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...  ", 0f);
        Dialogue.Instance.TypeText("PRESS SPACE TO CONTINUE", 0f);
    }
}
