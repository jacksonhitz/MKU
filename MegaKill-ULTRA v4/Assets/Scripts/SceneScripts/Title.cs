using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : ScenesManager
{
    protected override void Awake()
    {
        StateManager.State = StateManager.GameState.TITLE;
        Instance = this;
        
    }
    void Start()
    {
        SoundManager.Instance.Play("Title");
    }
    protected override void Update() { }
}
