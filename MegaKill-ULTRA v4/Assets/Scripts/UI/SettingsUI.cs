using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        sfxSlider.value = SettingsManager.Instance.sFXVolume;
        musicSlider.value = SettingsManager.Instance.musicVolume;
        sensSlider.value = SettingsManager.Instance.sensitivity;

        sfxInput.text = SettingsManager.Instance.sFXVolume.ToString("F0");
        musicInput.text = SettingsManager.Instance.musicVolume.ToString("F0");
        sensInput.text = SettingsManager.Instance.sensitivity.ToString("F0");

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
        StateManager.OnStateChanged += StateChange;
        StateManager.OnSilentChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
        StateManager.OnSilentChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        Debug.Log("state called");

        if (state == StateManager.GameState.PAUSED)
        {
            menu.enabled = true;
            Debug.Log("menu enabled");
        }
        else
        {
            menu.enabled = false;
            Debug.Log("menu disabled");
        }
    }

    public void Resume()
    {
        StateManager.LoadSilent(StateManager.PREVIOUS);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Exit()
    {
        StateManager.LoadState(StateManager.GameState.TITLE);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Restart()
    {
        StateManager.LoadState(StateManager.PREVIOUS);
        EventSystem.current.SetSelectedGameObject(null);
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
        sfxInput.text = SettingsManager.Instance.sFXVolume.ToString("F0");
    }

    void OnMusicInputChanged(string value)
    {
        if (float.TryParse(value, out float v))
        {
            SettingsManager.Instance.SetMusicVolume(v);
            musicSlider.value = v;
        }
        musicInput.text = SettingsManager.Instance.musicVolume.ToString("F0");
    }

    void OnSensitivityInputChanged(string value)
    {
        if (float.TryParse(value, out float v))
        {
            SettingsManager.Instance.SetSensitivity(v);
            sensSlider.value = v;
        }
        sensInput.text = SettingsManager.Instance.sensitivity.ToString("F0");
    }
}
