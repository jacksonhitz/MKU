using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public static class StateManager
{
    public enum GameState
    {
        TITLE,
        TUTORIAL,
        REHEARSAL,
        TANGO,
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
        GameState.SABLE,
        GameState.SPEARHEAD,
    };

    public static readonly Dictionary<Type, GameState> LevelMapping = new()
    {
        { typeof(Title), GameState.TITLE },
        { typeof(Tutorial), GameState.TUTORIAL },
        { typeof(Rehearsal), GameState.REHEARSAL },
        { typeof(Tango), GameState.TANGO },
        { typeof(Sable), GameState.SABLE },
        { typeof(Spearhead), GameState.SPEARHEAD },
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
        _level is not GameState.TITLE && SceneScript.Instance?.State is SceneState.PLAYING;

    public static bool IsPassive =>
        _level is GameState.TITLE || SceneScript.Instance?.State is not SceneState.PLAYING;

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
        Level = newLevel;
        Debug.Log($"Loaded level {_level}");
    }

    [Conditional("UNITY_EDITOR")]
    public static void DebugSetLevel(GameState level)
    {
        Debug.LogWarning("Level set manually in StateManager, make sure this is intentional.");
        Level = level;
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
