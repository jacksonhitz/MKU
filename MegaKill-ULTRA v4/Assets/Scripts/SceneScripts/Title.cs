using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : ScenesManager
{
    protected override void Awake()
    {
            StateManager.State = StateManager.GameState.TITLE;
    }
    protected override void Update() { }
}
