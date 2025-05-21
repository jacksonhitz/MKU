using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public float musicVolume { get; private set; } = 50;
    public float sFXVolume { get; private set; } = 50;
    public float sensitivity { get; private set; } = 500;

    public SoundManager sound;

    

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        sound = SoundManager.Instance;
        SetMusicVolume(musicVolume);
        SetSFXVolume(sFXVolume);
        SetSensitivity(sensitivity);

    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp(value, 0, 100);

        sound.music.volume = musicVolume / 300f;
    }

    public void SetSFXVolume(float value)
    {
        sFXVolume = Mathf.Clamp(value, 0, 100);
        sound.sfx.volume = sFXVolume / 100f;
        sound.dialogue.volume = sFXVolume / 100f;
    }

    public void SetSensitivity(float value)
    {
        sensitivity = Mathf.Clamp(value, 0, 1000);
    }
}
