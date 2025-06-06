using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileUI : MonoBehaviour
{
    public static FileUI Instance { get; set; }

    [SerializeField] SpriteRenderer file;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
        StateManager.OnSilentChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
        StateManager.OnSilentChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        if (StateManager.State == StateManager.GameState.FILE || StateManager.previous == StateManager.GameState.FILE)
            file.enabled = true;
        else
            file.enabled = false;
    }
}
