using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StateManager
{
    public enum GameState
    {
        TESTING,
        TITLE,
        INTRO,

        TUTORIAL,
        TANGO,
        SABLE,
        SPEARHEAD,
        LAUNCH,

        FIGHT,

        PAUSED,
        DEAD,
        OUTRO,
    }

    static GameState state;
    static GameState previous;

    public static event Action<GameState> OnStateChanged;
    public static event Action<GameState> OnSilentChanged;
    public static GameState PREVIOUS => previous;

    static readonly List<GameState> SceneOrder = new()
    {
        GameState.TITLE,
        GameState.TUTORIAL,
        GameState.TANGO,
        GameState.SABLE,
        GameState.SPEARHEAD,
        //GameState.LAUNCH, 
    };
    static readonly HashSet<GameState> Scene = new(SceneOrder);

    static readonly HashSet<GameState> Active = new()
    {
        GameState.TUTORIAL,
        GameState.LAUNCH,
        GameState.TANGO,
        GameState.SABLE,
        GameState.SPEARHEAD,
        GameState.FIGHT,
        GameState.TESTING
    };

    static readonly HashSet<GameState> Passive = new()
    {
        GameState.TITLE,
        GameState.INTRO,
        GameState.OUTRO,
        GameState.PAUSED,
        GameState.DEAD,
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
        OnSilentChanged?.Invoke(state);
    }


    public static bool GroupCheck(HashSet<GameState> group) => group.Contains(state);

    public static bool IsActive() => GroupCheck(Active);
    public static bool IsPassive() => GroupCheck(Passive);
    public static bool IsScene() => GroupCheck(Scene);

    public static void LoadState(GameState newState)
    {
        if (State != newState || previous != GameState.PAUSED)
        {
            State = newState;
            if (Scene.Contains(newState) && SceneManager.GetActiveScene().name != newState.ToString())
            {
                SceneManager.LoadScene(newState.ToString());
                Debug.Log("Loading Scene " + newState);
            }
        }
        else
        {
            SilentState(newState);
        }
    }


    public static void NextState()
    {
        int currentIndex = SceneOrder.IndexOf(state);
        int nextIndex = (currentIndex + 1) % SceneOrder.Count;
        LoadState(SceneOrder[nextIndex]);
    }
}
