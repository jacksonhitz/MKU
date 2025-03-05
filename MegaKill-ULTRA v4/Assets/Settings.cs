using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    CamController cam;
    SoundManager soundManager;
    GameManager gameManager;

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_InputField volumeInput;
    [SerializeField] TMP_InputField sensInput;
    
    public float volume = 50;
    public float sens = 500;

    public bool isTutorial;

    
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
            isTutorial = true;
        }
    }

    void Update()
    {

        if (cam == null)
        {
            cam = FindAnyObjectByType<CamController>();
        }
        if (soundManager == null)
        {
            soundManager = FindAnyObjectByType<SoundManager>();
        }
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }


    
    void Start()
    {
        volumeSlider.value = volume;
        sensSlider.value = sens;
        volumeInput.text = volume.ToString("F0");
        sensInput.text = sens.ToString("F0");

        volumeSlider.onValueChanged.AddListener(VolumeText);
        sensSlider.onValueChanged.AddListener(SensText);
        volumeInput.onEndEdit.AddListener(VolumeSlider);
        sensInput.onEndEdit.AddListener(SensSlider);

        volumeInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        sensInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }



    void VolumeText(float newValue)
    {
        volume = newValue;
        volumeInput.text = volume.ToString("F0");
        soundManager.volume = volume;
        soundManager.UpdateVolume();
    }

    void SensText(float newValue)
    {
        sens = newValue;
        sensInput.text = sens.ToString("F0");
        cam.sens = sens;
    }

    void VolumeSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            volume = Mathf.Clamp(parsedValue, 0, 100);
            volumeInput.text = volume.ToString("F0");
            volumeSlider.value = volume;
            soundManager.volume = volume;
            soundManager.UpdateVolume();
        }
        else
        {
            volumeInput.text = volume.ToString("F0");
        }
    }

    void SensSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            sens = Mathf.Clamp(parsedValue, 0, 1000);
            sensInput.text = sens.ToString("F0");
            sensSlider.value = sens;
            cam.sens = sens;
        }
        else
        {
            sensInput.text = sens.ToString("F0");
        }
    }
}
