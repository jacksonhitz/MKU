using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class SettingsManager : MonoBehaviour
{
    //CALL AWAKE/START SHIT EXTERNALLY SO THAT MANAGERS WITHOUT VAR CAN BE STATIC/ABSTRACT

    [ResetOnPlay]
    public static SettingsManager Instance { get; private set; }

    [ResetOnPlay]
    public static event Action<bool> OnPauseChange = delegate { };

    [SerializeField]
    private static bool _isPaused;

    public static bool IsPaused
    {
        get => _isPaused;
        private set
        {
            if (value != _isPaused)
                OnPauseChange?.Invoke(value);
            _isPaused = value;
        }
    }

    SettingsData settings;

    [SerializeField]
    GameObject menu;

    public float MusicVolume => settings.musicVolume;
    public float SFXVolume => settings.sFXVolume;
    public float Sensitivity => settings.sensitivity;

    private void Awake()
    {
        Instance = this;

        settings = Resources.Load<SettingsData>("Settings/Settings");
        Debug.Log("Settings Found: " + settings);
        if (settings == null)
            Debug.LogError("SettingsData asset not found at Resources/Settings/Settings");
        InputManager.PlayerActionMap.Pause.performed += OnPausePerformed;
    }

    private void OnDestroy()
    {
        InputManager.PlayerActionMap.Pause.performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext obj)
    {
        TogglePause();
    }

    void Start()
    {
        SetSettings();
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Exit()
    {
        Time.timeScale = 1;
        _ = StateManager.LoadLevel(StateManager.GameState.TITLE, 2f, destroyCancellationToken);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        StateManager.RestartLevel(2f, Application.exitCancellationToken).Forget();
    }

    void SetSettings()
    {
        SetMusicVolume(settings.musicVolume);
        SetSFXVolume(settings.sFXVolume);
        SetSensitivity(settings.sensitivity);

        Resume();
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0, 100);
        settings.musicVolume = value;

        if (SoundManager.Instance != null && SoundManager.Instance.music != null)
            SoundManager.Instance.music.volume = value / 300f;
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0, 100);
        settings.sFXVolume = value;

        if (SoundManager.Instance == null)
            return;
        if (SoundManager.Instance.sfx != null)
            SoundManager.Instance.sfx.volume = value / 100f;

        if (SoundManager.Instance.dialogue != null)
            SoundManager.Instance.dialogue.volume = value / 100f;
    }

    public void SetSensitivity(float value)
    {
        value = Mathf.Clamp(value, 0, 1000);
        settings.sensitivity = value;
    }
}
