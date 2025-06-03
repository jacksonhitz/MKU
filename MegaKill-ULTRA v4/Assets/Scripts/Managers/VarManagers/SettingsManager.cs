using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    SettingsData settings;

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

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0, 100);
        settings.musicVolume = value;

        SoundManager.Instance.music.volume = value / 300f;
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0, 100);
        settings.sFXVolume = value;

        SoundManager.Instance.sfx.volume = value / 100f;
        SoundManager.Instance.dialogue.volume = value / 100f;
    }

    public void SetSensitivity(float value)
    {
        value = Mathf.Clamp(value, 0, 1000);
        settings.sensitivity = value;
    }
}
