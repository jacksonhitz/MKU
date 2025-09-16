using System;
using AudioSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityUtils;
using Debug = UnityEngine.Debug;

public class SettingsManager : PersistentSingleton<SettingsManager>
{
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

    [Header("Mixers")]
    [SerializeField]
    private AudioMixerGroup masterMixer;

    [SerializeField]
    private AudioMixerGroup sfxMixer;

    [SerializeField]
    private AudioMixerGroup musicMixer;

    [SerializeField]
    private AudioMixerGroup dialogueMixer;

    public float MusicVolume => settings.musicVolume;
    public float SFXVolume => settings.sFXVolume;
    public float Sensitivity => settings.sensitivity;

    protected override void Awake()
    {
        base.Awake();
        if (gameObject == null)
            return;
        settings = Resources.Load<SettingsData>("Settings/Settings");
        Debug.Log("Settings Found: " + settings);
        if (settings == null)
            Debug.LogError("SettingsData asset not found at Resources/Settings/Settings");
        InputManager.PlayerActionMap.Pause.performed += OnPausePerformed;
    }

    private void OnDestroy()
    {
        if (!InputManager.Enabled)
            return;

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
        value = Mathf.Clamp01(value);
        settings.musicVolume = value;
        musicMixer.audioMixer.SetFloat("musicVolume", AudioExtensions.ToLogarithmicVolume(value));
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);
        settings.sFXVolume = value;
        sfxMixer.audioMixer.SetFloat("sfxVolume", AudioExtensions.ToLogarithmicVolume(value));
    }

    public void SetSensitivity(float value)
    {
        value = Mathf.Clamp(value, 0, 1000);
        settings.sensitivity = value;
    }
}
