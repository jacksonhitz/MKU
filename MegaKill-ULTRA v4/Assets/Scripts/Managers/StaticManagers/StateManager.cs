using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StateManager
{
    public enum GameState
    {
        TESTING,
        TITLE,

        TUTORIAL,
        REHEARSAL,
        TANGO,
        TANGO2,
        SABLE,
        SPEARHEAD,

        PAUSED,
        TRANSITION,
        FILE,
        SCORE
    }

    static GameState state;
    static GameState previous;
    static GameState scene;

    public static GameState PREVIOUS => previous;
    public static GameState STATE => state;
    public static GameState SCENE => scene;

    public static event Action<GameState> OnStateChanged;
    public static event Action<GameState> OnSilentChanged;

    static readonly List<GameState> Order = new()
    {
        GameState.TITLE,
        GameState.TUTORIAL,
        GameState.REHEARSAL,
        GameState.TANGO,
        GameState.TANGO2,
        GameState.SABLE,
        GameState.SPEARHEAD
    };

    static readonly HashSet<GameState> Scene = new()
    {
        GameState.TITLE,
        GameState.TUTORIAL,
        GameState.REHEARSAL,
        GameState.TANGO,
        GameState.SABLE,
        GameState.SPEARHEAD,
    };

    static readonly HashSet<GameState> Active = new()
    {
        GameState.TUTORIAL,
        GameState.REHEARSAL,
        GameState.TANGO,
        GameState.TANGO2,
        GameState.SABLE,
        GameState.SPEARHEAD,
        GameState.TESTING
    };

    static readonly HashSet<GameState> Passive = new()
    {
        GameState.TITLE,
        GameState.FILE,
        GameState.PAUSED,
        GameState.TRANSITION,
        GameState.SCORE
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

    public static bool GroupCheck(HashSet<GameState> group) => group.Contains(state);

    public static bool IsActive() => GroupCheck(Active);
    public static bool IsPassive() => GroupCheck(Passive);
    public static bool IsScene() => GroupCheck(Scene);

    public static IEnumerator LoadState(GameState newState, float delay)
    {
        if (Scene.Contains(newState))
        {
            State = GameState.TRANSITION;
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(newState.ToString());
            Debug.Log("Loading Scene " + newState);
            State = newState;
        }
        else State = newState;
    }
    public static void LoadSilent(GameState newState)
    {
        previous = state;   
        state = newState;
        OnSilentChanged?.Invoke(state);
    }

    public static void NextState(MonoBehaviour caller)
    {
        int currentIndex = Order.IndexOf(state);
        int nextIndex = (currentIndex + 1) % Order.Count;
        caller.StartCoroutine(LoadState(Order[nextIndex], 2f));
    }
}
