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
        TRANSITION
    }

    static GameState state;
    static GameState previous;

    public static event Action<GameState> OnStateChanged;
    public static event Action<GameState> OnSilentChanged;

    public static GameState PREVIOUS => previous;
    public static GameState STATE => state;

    static readonly List<GameState> StateOrder = new()
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
        GameState.PAUSED,
        GameState.TRANSITION,
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
        int currentIndex = StateOrder.IndexOf(state);
        int nextIndex = (currentIndex + 1) % StateOrder.Count;
        caller.StartCoroutine(LoadState(StateOrder[nextIndex], 2f));
    }
}
