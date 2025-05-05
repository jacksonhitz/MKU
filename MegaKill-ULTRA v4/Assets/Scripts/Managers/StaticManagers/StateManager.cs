using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StateManager
{
    public enum GameState
    {
        TITLE,
        INTRO,

        TUTORIAL,
        TANGO,
        SABLE,
        SPEARHEAD,
        LAUNCH,

        FIGHT,
        TESTING,

        PAUSED,
        DEAD,
        OUTRO,
    }

    static GameState state;
    static GameState previous;

    public static event Action<GameState> OnStateChanged;
    public static GameState PREVIOUS => previous;

    static readonly HashSet<GameState> Scene = new()
    {
        GameState.TITLE,
        GameState.TUTORIAL,
        GameState.LAUNCH,
        GameState.TANGO,
        GameState.SABLE,
        GameState.SPEARHEAD,
    };

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
    }

    public static bool GroupCheck(HashSet<GameState> group) => group.Contains(state);

    public static bool IsActive() => GroupCheck(Active);
    public static bool IsPassive() => GroupCheck(Passive);
    public static bool IsScene() => GroupCheck(Scene);

    public static void LoadState(GameState newState)
    {
        if (State != newState && Scene.Contains(newState))
        {
            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                State = newState;
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(newState.ToString());

            Debug.Log("Loading " + newState);
        }
        else
        {
            State = newState;
        }
    }
    public static void NextState()
    {
        GameState[] values = (GameState[])Enum.GetValues(typeof(GameState));
        int currentIndex = Array.IndexOf(values, state);
        int nextIndex = (currentIndex + 1) % values.Length;
        LoadState(values[nextIndex]);
    }
}
