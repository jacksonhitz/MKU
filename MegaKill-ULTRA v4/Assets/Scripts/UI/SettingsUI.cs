using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Canvas menu;
    [SerializeField] Slider sfxSlider, musicSlider, sensSlider;
    [SerializeField] TMP_InputField sfxInput, musicInput, sensInput;

    void Start()
    {
        menu.enabled = false;
        Init(sfxSlider, sfxInput, SettingsManager.Instance.SFXVolume, SettingsManager.Instance.SetSFXVolume);
        Init(musicSlider, musicInput, SettingsManager.Instance.MusicVolume, SettingsManager.Instance.SetMusicVolume);
        Init(sensSlider, sensInput, SettingsManager.Instance.Sensitivity, SettingsManager.Instance.SetSensitivity);
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

    void StateChange(StateManager.GameState state) => menu.enabled = (state == StateManager.GameState.PAUSED);

    public void Resume() => Close(StateManager.previous);
    public void Exit() => StartCoroutine(StateManager.LoadState(StateManager.GameState.TITLE, 2f));
    public void Restart() => StartCoroutine(StateManager.LoadState(StateManager.previous, 2f));

    void Close(StateManager.GameState target)
    {
        StateManager.SilentState = target;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Init(Slider slider, TMP_InputField input, float initial, System.Action<float> apply)
    {
        slider.value = initial;
        input.text = Mathf.RoundToInt(initial).ToString();
        input.characterValidation = TMP_InputField.CharacterValidation.Integer;

        slider.onValueChanged.AddListener(v => {
            apply(v);
            input.text = Mathf.RoundToInt(v).ToString();
        });

        input.onEndEdit.AddListener(text => {
            if (float.TryParse(text, out float val)) apply(val);
            slider.value = initial = slider.value; 
            input.text = Mathf.RoundToInt(slider.value).ToString();
        });
    }
}
