using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;
using SceneState = StateManager.SceneState;

public static class InputManager
{
    public static bool Enabled { get; private set; } = false;

    [ResetOnPlay]
    public static UIActions UIActionMap { get; private set; }

    [ResetOnPlay]
    public static PlayerActions PlayerActionMap { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Initialize()
    {
        var controls = new Controls();
        UIActionMap = controls.UI;
        UIActionMap.Enable();
        PlayerActionMap = controls.Player;
        PlayerActionMap.Enable();
        Enabled = true;
        SceneScript.StateChanged += SceneScriptOnStateChanged;
        SettingsManager.OnPauseChange += isPaused =>
        {
            if (isPaused)
            {
                SetActionMapState(PlayerActionMap, false);
                SetActionMapState(UIActionMap, true);
            }
            else if (StateManager.IsActive)
            {
                SetActionMapState(UIActionMap, false);
                SetActionMapState(PlayerActionMap, true);
            }
        };

        CancellationToken quitToken = Application.exitCancellationToken;
        quitToken.WaitUntilCanceled().GetAwaiter().OnCompleted(OnApplicationQuit);
    }

    private static void OnApplicationQuit()
    {
        Enabled = false;
    }

    private static void SceneScriptOnStateChanged(SceneState scene)
    {
        switch (scene)
        {
            case SceneState.TRANSITION:
                break;
            case SceneState.FILE:
                SetActionMapState(PlayerActionMap, false);
                SetActionMapState(UIActionMap, true);
                break;
            case SceneState.PLAYING:
                SetActionMapState(PlayerActionMap, true);
                SetActionMapState(UIActionMap, false);
                break;
            case SceneState.SCORE:
                SetActionMapState(PlayerActionMap, false);
                SetActionMapState(UIActionMap, true);
                break;
        }
    }

    private static void SetActionMapState(InputActionMap map, bool state)
    {
        foreach (InputAction inputAction in map)
        {
            if (state)
            {
                inputAction.Enable();
            }
            else
            {
                inputAction.Disable();
            }
        }
        PlayerActionMap.Pause.Enable();
    }
}
