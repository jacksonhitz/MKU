using System;
using System.Collections.Generic;
using UnityEngine;

public static class StateManager 
{
    public enum GameState
    {
        Title,
        Intro,
        Tutorial,
        Launch,
        Tango,
        Paused,
        Dead,
        Explore,
        Fight,
        Outro,
        Testing
    }

    private static GameState state;
    private static GameState previous;

    public static event Action<GameState> OnStateChanged;
    public static GameState Previous => previous;

    private static readonly HashSet<GameState> Lvls = new()
    {
        GameState.Launch,
        GameState.Tango,
    };
    private static readonly HashSet<GameState> Active = new()
    {
        GameState.Intro,
        GameState.Tutorial,
        GameState.Launch,
        GameState.Tango,
        GameState.Fight,
        GameState.Testing
    };
    private static readonly HashSet<GameState> Passive = new()
    {
        GameState.Title,
        GameState.Outro,
        GameState.Paused,
        GameState.Dead,
    };

    public static GameState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                if (state == GameState.Paused)
                {
                    SilentState(value);
                    return;
                }

                previous = state;
                state = value;
                OnStateChanged?.Invoke(state);
                Debug.Log("state Check");
            }
        }
    }

    public static void SilentState(GameState newState)
    {
        previous = state;
        state = newState;
    }

    public static bool GroupCheck(HashSet<GameState> group)
    {
        return group.Contains(state);
    }

    public static bool IsActive() => GroupCheck(Active);
    public static bool IsPassive() => GroupCheck(Passive);
}
