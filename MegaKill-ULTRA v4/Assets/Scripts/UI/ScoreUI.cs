using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; set; }

    [SerializeField] SpriteRenderer image;

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
        if (StateManager.State == StateManager.GameState.SCORE || StateManager.previous == StateManager.GameState.SCORE)
            image.enabled = true;
        else
            image.enabled = false;
    }
}
