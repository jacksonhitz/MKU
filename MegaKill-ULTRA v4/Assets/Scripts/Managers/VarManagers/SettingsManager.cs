using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public float MusicVolume { get; private set; } = 50;
    public float SFXVolume { get; private set; } = 50;
    public float Sensitivity { get; private set; } = 500;

    SoundManager sound;

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
        switch (state)
        {
          //  case StateManager.GameState.TITLE: Title(); break;

        }
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp(value, 0, 100);

        sound.music.volume = MusicVolume / 300f;
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp(value, 0, 100);
        sound.sfx.volume = SFXVolume / 100f;
        sound.dialogue.volume = SFXVolume / 100f;
    }

    public void SetSensitivity(float value)
    {
        Sensitivity = Mathf.Clamp(value, 0, 1000);
    }
}
