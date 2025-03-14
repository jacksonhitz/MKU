using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }


    SoundManager soundManager;
    SceneLoader sceneLoader;
    Canvas menu;
    

    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_InputField sfxInput;
    [SerializeField] TMP_InputField musicInput;
    [SerializeField] TMP_InputField sensInput;
    

    [HideInInspector] public float musicVolume = 50;
    [HideInInspector] public float sfxVolume = 50;
    [HideInInspector] public float sens = 500;


    public bool isPaused;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        soundManager = FindObjectOfType<SoundManager>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        menu = GetComponentInChildren<Canvas>();

    }

    void Start()
    {
        menu.enabled = false;
        
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;
        sensSlider.value = sens;
        sfxInput.text = sfxVolume.ToString("F0");
        musicInput.text = musicVolume.ToString("F0");
        sensInput.text = sens.ToString("F0");

        sfxSlider.onValueChanged.AddListener(SFXText);
        musicSlider.onValueChanged.AddListener(MusicText);
        sensSlider.onValueChanged.AddListener(SensText);

        sfxInput.onEndEdit.AddListener(SFXSlider);
        musicInput.onEndEdit.AddListener(MusicSlider);
        sensInput.onEndEdit.AddListener(SensSlider);

        sfxInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        musicInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        sensInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        soundManager.Pause();
        menu.enabled = true;
    }
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        soundManager.Play();
        menu.enabled = false;
    }

    public void Exit()
    {
        isPaused = false;
        Time.timeScale = 1f;
        soundManager.Play();
        menu.enabled = false;

        sceneLoader.Title();
    }

    public void Restart()
    {
        isPaused = false;
        Time.timeScale = 1f;
        soundManager.Play();
        menu.enabled = false;

        sceneLoader.Restart();
    }

    void SFXText(float newValue)
    {
        sfxVolume = newValue;
        sfxInput.text = sfxVolume.ToString("F0");
    }

    void MusicText(float newValue)
    {
        musicVolume = newValue;
        musicInput.text = musicVolume.ToString("F0");
    }

    void SensText(float newValue)
    {
        sens = newValue;
        sensInput.text = sens.ToString("F0");
    }

    void SFXSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            sfxVolume = Mathf.Clamp(parsedValue, 0, 100);
            sfxSlider.value = sfxVolume;
        }
        sfxInput.text = sfxVolume.ToString("F0");
    }

    void MusicSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            musicVolume = Mathf.Clamp(parsedValue, 0, 100);
            musicSlider.value = musicVolume;
        }
        musicInput.text = musicVolume.ToString("F0");
    }

    void SensSlider(string newValue)
    {
        if (float.TryParse(newValue, out float parsedValue))
        {
            sens = Mathf.Clamp(parsedValue, 0, 1000);
            sensSlider.value = sens;
        }
        sensInput.text = sens.ToString("F0");
    }
}
