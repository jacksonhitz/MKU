using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    //CALL AWAKE/START SHIT EXTERNALLY SO THAT MANAGERS WITHOUT VAR CAN BE STATIC/ABSTRACT

    public static SettingsManager Instance { get; set; }

    SettingsData settings;

    [SerializeField] GameObject menu;

    public float MusicVolume => settings.musicVolume;
    public float SFXVolume => settings.sFXVolume;
    public float Sensitivity => settings.sensitivity;

    void Awake()
    {
        Instance = this;

        settings = Resources.Load<SettingsData>("Settings/Settings");
        Debug.Log("Settings Found: " + settings);
        if (settings == null)
            Debug.LogError("SettingsData asset not found at Resources/Settings/Settings");
    }

    void Start()
    {
        SetSettings();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
    }
    public void Resume()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }
    public void Exit()
    {
        Time.timeScale = 1;
        StartCoroutine(StateManager.LoadState(StateManager.GameState.TITLE, 2f));
    }
    public void Restart()
    {
        StartCoroutine(StateManager.LoadState(StateManager.STATE, 2f));
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

        if (SoundManager.Instance != null)
        {
            if (SoundManager.Instance.sfx != null)
                SoundManager.Instance.sfx.volume = value / 100f;

            if (SoundManager.Instance.dialogue != null)
                SoundManager.Instance.dialogue.volume = value / 100f;
        }
    }

    public void SetSensitivity(float value)
    {
        value = Mathf.Clamp(value, 0, 1000);
        settings.sensitivity = value;
    }
}
