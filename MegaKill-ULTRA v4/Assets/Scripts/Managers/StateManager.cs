using System;
using UnityEngine;

public static class StateManager 
{
    public enum GameState
    {
        Title,
        Intro,
        Tutorial,
        Lvl,
        Outro
    }

    private static GameState state = GameState.Title;

    public static event Action<GameState> OnStateChanged;

    public static GameState State
    {
        get => state;
        set
        {
            if (state != value) 
            {
                state = value;
                OnStateChanged?.Invoke(state); 
            }
        }
    }
}
