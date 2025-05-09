using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Canvas menu;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_InputField sfxInput;
    [SerializeField] TMP_InputField musicInput;
    [SerializeField] TMP_InputField sensInput;

    void Start()
    {
        menu.enabled = false;

        // Initialize values from manager
        sfxSlider.value = SettingsManager.Instance.SFXVolume;
        musicSlider.value = SettingsManager.Instance.MusicVolume;
        sensSlider.value = SettingsManager.Instance.Sensitivity;

        sfxInput.text = SettingsManager.Instance.SFXVolume.ToString("F0");
        musicInput.text = SettingsManager.Instance.MusicVolume.ToString("F0");
        sensInput.text = SettingsManager.Instance.Sensitivity.ToString("F0");

        // Setup input validation
        sfxInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        musicInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        sensInput.characterValidation = TMP_InputField.CharacterValidation.Integer;

        // Listeners
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sensSlider.onValueChanged.AddListener(OnSensitivitySliderChanged);

        sfxInput.onEndEdit.AddListener(OnSFXInputChanged);
        musicInput.onEndEdit.AddListener(OnMusicInputChanged);
        sensInput.onEndEdit.AddListener(OnSensitivityInputChanged);
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= OnStateChanged;
    }

    void OnStateChanged(StateManager.GameState state)
    {
        if (state == StateManager.GameState.PAUSED)
        {
            menu.enabled = true;
        }
    }

    public void Resume()
    {
        menu.enabled = false;
        StateManager.SilentState(StateManager.PREVIOUS);
    }

    public void Exit()
    {
        StateManager.LoadState(StateManager.GameState.TITLE);
    }

    public void Restart()
    {
        menu.enabled = false;
        StateManager.LoadState(StateManager.State);
    }

    void OnSFXSliderChanged(float value)
    {
        SettingsManager.Instance.SetSFXVolume(value);
        sfxInput.text = value.ToString("F0");
    }

    void OnMusicSliderChanged(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
        musicInput.text = value.ToString("F0");
    }

    void OnSensitivitySliderChanged(float value)
    {
        SettingsManager.Instance.SetSensitivity(value);
        sensInput.text = value.ToString("F0");
    }

    void OnSFXInputChanged(string value)
    {
        if (float.TryParse(value, out float v))
        {
            SettingsManager.Instance.SetSFXVolume(v);
            sfxSlider.value = v;
        }
        sfxInput.text = SettingsManager.Instance.SFXVolume.ToString("F0");
    }

    void OnMusicInputChanged(string value)
    {
        if (float.TryParse(value, out float v))
        {
            SettingsManager.Instance.SetMusicVolume(v);
            musicSlider.value = v;
        }
        musicInput.text = SettingsManager.Instance.MusicVolume.ToString("F0");
    }

    void OnSensitivityInputChanged(string value)
    {
        if (float.TryParse(value, out float v))
        {
            SettingsManager.Instance.SetSensitivity(v);
            sensSlider.value = v;
        }
        sensInput.text = SettingsManager.Instance.Sensitivity.ToString("F0");
    }
}
