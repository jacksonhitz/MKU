using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    CamController cam;
    SoundManager soundManager;

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_InputField volumeInput;
    [SerializeField] TMP_InputField sensInput;
    
    float volume = 50;
    float sens = 50;
    
    void Awake()
    {
        if (FindObjectsOfType<Settings>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        soundManager = FindAnyObjectByType<SoundManager>();
        cam = FindAnyObjectByType<CamController>();
    }

    void Start()
    {
        volumeSlider.value = volume;
        sensSlider.value = sens;
        volumeInput.text = volume.ToString("F0");
        sensInput.text = sens.ToString("F0");

        volumeSlider.onValueChanged.AddListener(UpdateVolumeText);
        sensSlider.onValueChanged.AddListener(UpdateSenText);
        volumeInput.onEndEdit.AddListener(UpdateVolumeSlider);
        sensInput.onEndEdit.AddListener(UpdateSensSlider);

        volumeInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        sensInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }

    void UpdateVolumeText(float newValue)
    {
        volume = newValue;
        volumeInput.text = volume.ToString("F0");
        //soundManager.SetVolume(volume / 100f);
    }

    void UpdateSenText(float newValue)
    {
        sens = newValue;
        sensInput.text = sens.ToString("F0");
        //cam.SetSensitivity(sens / 100f);
    }

    void UpdateVolumeSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            volume = Mathf.Clamp(parsedValue, 0, 100);
            volumeInput.text = volume.ToString("F0");
            volumeSlider.value = volume;
            //soundManager.SetVolume(volume / 100f);
        }
        else
        {
            volumeInput.text = volume.ToString("F0");
        }
    }

    void UpdateSensSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            sens = Mathf.Clamp(parsedValue, 0, 100);
            sensInput.text = sens.ToString("F0");
            sensSlider.value = sens;
            //cam.SetSensitivity(sens / 100f);
        }
        else
        {
            sensInput.text = sens.ToString("F0");
        }
    }
}
