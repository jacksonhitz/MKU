using System;
using System.Collections.Generic;
using UnityEngine;

public static class StateManager
{
    public enum GameState
    {
        Title,
        Intro,
        Paused,
        Dead,
        Outro,

        Tutorial,
        Launch,
        Tango,
        Sable,
        Spearhead,

        Fight,
        Testing
    }
    static readonly List<GameState> StateList = new List<GameState>
    {
        GameState.Title,
        GameState.Intro,
        GameState.Tutorial,
        GameState.Launch,
        GameState.Tango,
        GameState.Sable,
        GameState.Spearhead,
        GameState.Fight,
        GameState.Testing,
        GameState.Paused,
        GameState.Dead,
        GameState.Outro
    };

    private static GameState state;
    private static GameState previous;

    public static event Action<GameState> OnStateChanged;
    public static GameState Previous => previous;

    private static readonly HashSet<GameState> Lvls = new()
    {
        GameState.Tutorial, //Tutorial
        GameState.Launch, //Subway (Broken)
        GameState.Tango, //Festival
        GameState.Sable, //RUSSIA WIP
        GameState.Spearhead, //Vietnam (Not in yet)
    };
    private static readonly HashSet<GameState> Active = new()
    {
        GameState.Tutorial,
        GameState.Launch,
        GameState.Tango,
        GameState.Sable, 
        GameState.Spearhead,

        GameState.Fight,
        GameState.Testing
    };
    private static readonly HashSet<GameState> Passive = new()
    {
        GameState.Title,
        GameState.Intro,
        GameState.Outro,
        GameState.Paused,
        GameState.Dead,
    };

    public static GameState State
    {
        get => state;
        set
        {
            previous = state;
            state = value;

            OnStateChanged?.Invoke(state);
            Debug.Log($"State changed to {state}");
        }
    }

    public static void SilentState(GameState newState)
    {
        previous = state;
        state = newState;
    }

    public static bool GroupCheck(HashSet<GameState> group) => group.Contains(state);
    public static bool IsActive() => GroupCheck(Active);
    public static bool IsPassive() => GroupCheck(Passive);

    public static void Tutorial() => State = GameState.Tutorial;
    public static void Launch() => State = GameState.Launch;
    public static void Tango() => State = GameState.Tango;
    public static void Sable() => State = GameState.Tango;
    public static void Spearhead() => State = GameState.Spearhead;

    public static void Fight() => State = GameState.Fight;
    public static void Testing() => State = GameState.Testing;


    public static void Title() => State = GameState.Title;
    public static void Intro() => State = GameState.Intro;
    public static void Outro() => State = GameState.Outro;
    public static void Dead() => State = GameState.Dead;

    public static void Paused()
    {
        State = GameState.Paused;
        Time.timeScale = 0f;
    }

    public static void Unpaused()
    {
        State = Previous;
        Time.timeScale = 1f;
    }


    public static void NextState()
    {
        int stateIndex = StateList.IndexOf(state);
        if (stateIndex >= 0 && stateIndex < StateList.Count - 1)
        {
            State = StateList[stateIndex + 1]; 
        }
    }
}
