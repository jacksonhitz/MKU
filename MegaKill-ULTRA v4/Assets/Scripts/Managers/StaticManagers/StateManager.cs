using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
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

    private static GameState _level;

    public static event Action<GameState> LevelChanged;

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

    // TODO: REMOVE COMMENTS
    static readonly HashSet<GameState> Active = new()
    {
        GameState.TUTORIAL,
        GameState.REHEARSAL,
        GameState.TANGO,
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

    public static GameState Level
    {
        get => _level;
        private set
        {
            PreviousLevel = _level;
            _level = value;

            LevelChanged?.Invoke(_level);
            Debug.Log($"State changed to {_level}");
        }
    }

    public static GameState PreviousLevel { get; private set; } = GameState.TITLE;

    private static bool GroupCheck(HashSet<GameState> group) => group.Contains(_level);

    public static bool IsActive =>
        GroupCheck(Active) && SceneScript.Instance?.State is SceneState.PLAYING;

    public static bool IsPassive => GroupCheck(Passive);

    public static bool IsTransition => SceneScript.Instance?.State is SceneState.TRANSITION;

    public static bool IsFirstAttempt { get; private set; } = true;

    public static async UniTask LoadLevel(
        GameState newLevel,
        float delay,
        CancellationToken cancellationToken
    )
    {
        // TODO: Start loading scene during delay period and activate it when the delay is over
        if (SceneScript.Instance?.State is not SceneState.TRANSITION)
        {
            SceneScript.Instance?.ExitLevel();
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

        if (_level != newLevel)
        {
            PreviousLevel = newLevel;
            IsFirstAttempt = true;
        }
        else
        {
            IsFirstAttempt = false;
        }
        await SceneManager.LoadSceneAsync(newLevel.ToString());
        _level = newLevel;
        Debug.Log($"Loaded level {_level}");
    }

    [ConsoleMethod("GoToNextLevel", "Loads the next level in the order")]
    public static void LoadNext()
    {
        int currentIndex = Order.IndexOf(_level);
        int nextIndex = (currentIndex + 1) % Order.Count;
        _ = LoadLevel(Order[nextIndex], 2f, Application.exitCancellationToken);
    }

    public static async UniTask RestartLevel(float delay, CancellationToken cancellationToken)
    {
        Debug.Log("Restarting level");
        await LoadLevel(_level, delay, cancellationToken);
    }

    [ConsoleMethod("RestartLevel", "Restart the current level")]
    public static void RestartLevelCommand()
    {
        RestartLevel(2f, CancellationToken.None).Forget();
    }

    [ConsoleMethod("GoToLevel", "Loads the specified level")]
    public static void LoadLevelCommand(GameState newLevel)
    {
        _level = newLevel;
        LoadLevel(newLevel, 2f, Application.exitCancellationToken).Forget();
    }

    [ConsoleMethod("GoToLevel", "Loads the specified level")]
    public static void LoadLevelCommand(GameState newLevel, bool showFileScreen)
    {
        if (!showFileScreen)
            _level = newLevel;
        LoadLevel(newLevel, 2f, Application.exitCancellationToken).Forget();
    }
}
