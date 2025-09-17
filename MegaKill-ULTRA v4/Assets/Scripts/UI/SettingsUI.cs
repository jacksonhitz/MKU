using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [ResetOnPlay]
    public static SettingsUI Instance { get; private set; }

    [SerializeField]
    Slider sfxSlider,
        musicSlider,
        sensSlider;

    [SerializeField]
    TMP_InputField sfxInput,
        musicInput,
        sensInput;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Init(
            sfxSlider,
            sfxInput,
            SettingsManager.Instance.SFXVolume * 100f,
            x => SettingsManager.Instance.SetSFXVolume(x / 100f)
        );
        Init(
            musicSlider,
            musicInput,
            SettingsManager.Instance.MusicVolume * 100f,
            x => SettingsManager.Instance.SetMusicVolume(x / 100f)
        );
        Init(
            sensSlider,
            sensInput,
            SettingsManager.Instance.Sensitivity,
            SettingsManager.Instance.SetSensitivity
        );
    }

    public void Resume() => SettingsManager.Instance.Resume();

    public void Exit() => SettingsManager.Instance.Exit();

    public void Restart() => SettingsManager.Instance.Restart();

    void Init(Slider slider, TMP_InputField input, float initial, Action<float> apply)
    {
        slider.value = initial;
        input.text = Mathf.RoundToInt(slider.value).ToString();
        input.characterValidation = TMP_InputField.CharacterValidation.Integer;

        slider.onValueChanged.AddListener(v =>
        {
            apply(v);
            input.text = Mathf.RoundToInt(v).ToString();
        });

        input.onEndEdit.AddListener(text =>
        {
            if (float.TryParse(text, out float val))
                apply(val);
            slider.value = initial = slider.value;
            input.text = Mathf.RoundToInt(slider.value).ToString();
        });
    }
}
