using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public static class StateManager
{
    public enum GameState
    {
        TITLE,
        TUTORIAL,
        REHEARSAL,
        TANGO,

        // TANGO2,
        // TODO: make this scene code
        SABLE,
        SPEARHEAD,
    }

    public enum SceneState
    {
        TRANSITION,
        FILE,
        PLAYING,
        PAUSED,
        SCORE,
    }

    private static GameState state;
    public static GameState previous;

    public static event Action<GameState> OnStateChanged;
    public static event Action<GameState> OnSilentChanged;

    static readonly List<GameState> Order = new()
    {
        GameState.TITLE,
        GameState.TUTORIAL,
        GameState.REHEARSAL,
        GameState.TANGO,
        // GameState.TANGO2,
        GameState.SABLE,
        GameState.SPEARHEAD,
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

    // TODO: REMOVE COMMENTS
    static readonly HashSet<GameState> Active = new()
    {
        GameState.TUTORIAL,
        GameState.REHEARSAL,
        GameState.TANGO,
        // GameState.TANGO2,
        GameState.SABLE,
        GameState.SPEARHEAD,
    };

    static readonly HashSet<GameState> Passive = new()
    {
        GameState.TITLE,
        // GameState.FILE,
        // GameState.TRANSITION,
        // GameState.SCORE
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

    private static bool GroupCheck(HashSet<GameState> group) => group.Contains(state);

    public static bool IsActive() =>
        GroupCheck(Active) && SceneScript.Instance?.State is SceneState.PLAYING;

    public static bool IsPassive() => GroupCheck(Passive);

    public static bool IsScene() => GroupCheck(Scene);

    public static bool IsTransition() => SceneScript.Instance?.State is SceneState.TRANSITION;

    public static async UniTask LoadLevel(
        GameState newState,
        float delay,
        CancellationToken cancellationToken
    )
    {
        if (SceneScript.Instance?.State is not SceneState.TRANSITION)
        {
            SceneScript.Instance?.EndLevel();
            await UniTask.Delay(
                TimeSpan.FromSeconds(delay),
                DelayType.Realtime,
                cancellationToken: cancellationToken
            );
        }
        else
        {
            Debug.LogWarning("Trying to load state while already transitioning, ignoring request");
            return;
        }

        if (Scene.Contains(newState))
        {
            await SceneManager.LoadSceneAsync(newState.ToString());
            state = newState;
        }
    }

    public static void LoadNext()
    {
        int currentIndex = Order.IndexOf(state);
        int nextIndex = (currentIndex + 1) % Order.Count;
        _ = LoadLevel(Order[nextIndex], 2f, CancellationToken.None);
    }

    public static async UniTask RestartLevel(float delay, CancellationToken cancellationToken)
    {
        await LoadLevel(state, delay, cancellationToken);
    }
}
