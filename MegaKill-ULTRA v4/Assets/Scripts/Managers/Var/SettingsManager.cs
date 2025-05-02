using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public float MusicVolume { get; private set; } = 50;
    public float SFXVolume { get; private set; } = 50;
    public float Sensitivity { get; private set; } = 500;

    SoundManager soundManager;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        soundManager = FindObjectOfType<SoundManager>();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp(value, 0, 100);
        if (soundManager != null)
            soundManager.music.volume = MusicVolume / 300f;
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp(value, 0, 100);
        if (soundManager != null)
        {
            soundManager.sfx.volume = SFXVolume / 100f;
            soundManager.dialogue.volume = SFXVolume / 100f;
        }
    }

    public void SetSensitivity(float value)
    {
        Sensitivity = Mathf.Clamp(value, 0, 1000);
    }
}
