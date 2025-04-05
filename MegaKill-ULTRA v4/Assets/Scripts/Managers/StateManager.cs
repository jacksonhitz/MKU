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
        Lvl,
        Paused,
        Outro,
        Testing
    }

    private static GameState state = GameState.Testing;
    private static GameState previous;


    public static event Action<GameState> OnStateChanged;
    public static GameState Previous => previous;

    private static readonly HashSet<GameState> Active = new()
    {
        GameState.Intro,
        GameState.Tutorial,
        GameState.Lvl,
        GameState.Testing
    };

    private static readonly HashSet<GameState> Passive = new()
    {
        GameState.Title,
        GameState.Outro,
        GameState.Paused
    };

    public static GameState State
    {
        get => state;
        set
        {
            if (state != value) 
            {
                previous = state; 
                state = value;
                OnStateChanged?.Invoke(state); 
            }
        }
    }


    public static bool GroupCheck(HashSet<GameState> group)
    {
        return group.Contains(state);
    }
    public static bool IsActive() => GroupCheck(Active);
    public static bool IsPassive() => GroupCheck(Passive);
}
