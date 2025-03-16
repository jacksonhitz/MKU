using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CamController : MonoBehaviour
{
    [HideInInspector] public float sens = 500f;
    public Transform player;

    private float xRotation;
    private float yRotation;

    public Volume dynamicVolume;
    public Volume staticVolume;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorGrading;
    private ChannelMixer channelMixer;

    float chromSpd = 0.5f;
    float satSpd = 5f;
    float clrSpd;
    float fovSpd;

    float redRandom;
    float greenRandom;
    float blueRandom;
    float redStart;
    float greenStart;
    float blueStart;

    Camera cam;
    float originalFOV;

    private Vector3 originalPosition;
    float swayIntensity = 0.01f;
    Settings settings;
    SceneLoader sceneLoader;

    // Shader variables
    public Material camMat;
    float currentLerp = 0.1f;
    float currentFrequency = 0;
    float currentAmplitude = 0;

    public int phase;

    private float targetSpeedX;
    private float targetSpeedY;
    private float lerpSpeed = 0.001f;

    PlayerController playerController;

    void Awake()
    {
        cam = GetComponent<Camera>();
        settings = FindObjectOfType<Settings>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        playerController = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        currentLerp = 0f;
        currentFrequency = 0;
        currentAmplitude = 0;
        
        
        if (camMat != null)
        {
            camMat.SetFloat("_Lerp", currentLerp);
            camMat.SetFloat("_Frequency", currentFrequency);
            camMat.SetFloat("_Amplitude", currentAmplitude);

            RandomizeSpeed();
        }

        originalFOV = cam.fieldOfView;
        originalPosition = transform.localPosition;

        staticVolume.profile.TryGet(out chromaticAberration);
        staticVolume.profile.TryGet(out colorGrading);
        dynamicVolume.profile.TryGet(out channelMixer);

        SetClr();

        if (colorGrading != null)
        {
            colorGrading.postExposure.value = -10f;
        }

        StartCoroutine(FadeIn(2f));
    }

    void Update()
    {
        sens = settings.sens;
        
        if (!settings.isPaused && StateManager.state != StateManager.GameState.Title)
        {
            MoveCam();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (playerController.isDead)
        {
            TransitionOn();
        }

        

        UpdatePost();

        if (StateManager.state != StateManager.GameState.Intro)
        {
            if(!playerController.isDead)
            {
                currentLerp = 0.15f;
            }
            
            UpdateShader();
            
            Debug.Log("shaders called");

            if (dynamicVolume.weight < 5)
            {
                dynamicVolume.weight += 0.0001f;
            }
            fovSpd = 0.1f * dynamicVolume.weight;
            clrSpd = 10f * dynamicVolume.weight;

            colorGrading.saturation.value += Time.deltaTime * satSpd;
        }
        else
        {
            colorGrading.saturation.value = -180f;
        }
    }

    public void UpPhase()
    {
        phase++;
    }

    void TransitionOn()
    {
        currentAmplitude += 0.1f;
        currentFrequency += 0.1f;
        currentLerp += 0.01f;

        camMat.SetFloat("_Lerp", currentAmplitude);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentFrequency);
    }

    void TransitionOff()
    {
        sceneLoader.transition = false;
        ResetShader();
    }

    void ResetShader()
    {
        currentLerp = 0f;
        currentFrequency = 0.15f;
        currentAmplitude = 0f;

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);
    }

    public void Focus()
    {
        currentAmplitude -= 4f;
        currentFrequency -= 4f;
        phase--;
    }

    void UpdateShader()
    {
        float cap = 4f * phase;
        Debug.Log(cap);

        if (currentAmplitude < cap)
        {
            currentAmplitude += 0.001f;
        }
        if (currentFrequency < cap * 2)
        {
            currentFrequency += 0.001f;
        }

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);

        float speedX = Mathf.Lerp(camMat.GetFloat("_SpeedX"), targetSpeedX, lerpSpeed);
        float speedY = Mathf.Lerp(camMat.GetFloat("_SpeedY"), targetSpeedY, lerpSpeed);

        camMat.SetFloat("_SpeedX", speedX);
        camMat.SetFloat("_SpeedY", speedY);
    }

    void RandomizeSpeed()
    {
        targetSpeedX = Random.Range(-0.01f, 0.01f);
        targetSpeedY = Random.Range(-0.01f, 0.01f);
    }

    public void CallFadeIn()
    {
        StartCoroutine(FadeIn(0.5f));
    }

    public void CallFadeOut()
    {
        StartCoroutine(FadeOut(0.5f));
    }

    IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        float initialExposure = -10f;
        float targetExposure = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (colorGrading != null)
            {
                colorGrading.postExposure.value = Mathf.Lerp(initialExposure, targetExposure, elapsed / duration);
            }
            yield return null;
        }

        if (colorGrading != null)
        {
            colorGrading.postExposure.value = targetExposure;
        }
    }

    IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;
        float initialExposure = 0f;
        float targetExposure = -10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (colorGrading != null)
            {
                colorGrading.postExposure.value = Mathf.Lerp(initialExposure, targetExposure, elapsed / duration);
            }
            yield return null;
        }

        if (colorGrading != null)
        {
            colorGrading.postExposure.value = targetExposure;
        }
    }

    void SetClr()
    {
        redRandom = Random.Range(0.75f, 1.25f);
        greenRandom = Random.Range(0.75f, 1.25f);
        blueRandom = Random.Range(0.75f, 1.25f);

        redStart = Random.Range(-200f, -150f);
        greenStart = Random.Range(-200f, -150f);
        blueStart = Random.Range(-200f, -150f);

        channelMixer.redOutRedIn.value = redStart;
        channelMixer.greenOutGreenIn.value = greenStart;
        channelMixer.blueOutBlueIn.value = blueStart;
    }

    void MoveCam()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sens / Time.timeScale;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sens / Time.timeScale;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.9f, 89.9f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);

        Debug.Log("sens: " + sens);
    }

    void UpdatePost()
    {
        chromaticAberration.intensity.value = Mathf.PingPong(Time.time * chromSpd, 1f);

        float fovChange = Mathf.Sin(Time.time * fovSpd) * dynamicVolume.weight;
        cam.fieldOfView = originalFOV + fovChange;

        float swayAmountX = Mathf.Sin(Time.time * 2f) * swayIntensity * dynamicVolume.weight;
        float swayAmountY = Mathf.Cos(Time.time * 2f) * swayIntensity * dynamicVolume.weight;
        transform.localPosition = originalPosition + new Vector3(swayAmountX, swayAmountY, 0);

        float rotationSwayX = Mathf.Sin(Time.time * 1.5f) * swayIntensity * 0.5f * dynamicVolume.weight;
        float rotationSwayY = Mathf.Cos(Time.time * 1.5f) * swayIntensity * 0.5f * dynamicVolume.weight;
        transform.localRotation = Quaternion.Euler(rotationSwayX, rotationSwayY, 0) * Quaternion.Euler(xRotation, 0f, 0f);

        ClrAdjuster();
        ClrMixer();
    }

    void ClrAdjuster()
    {
        float hue = Mathf.PingPong(Time.time * clrSpd * (redRandom + greenRandom + blueRandom) / 3f, 360f);
        colorGrading.hueShift.value = Mathf.Lerp(-180f, 180f, hue / 360f);
    }


    void ClrMixer()
    {
        channelMixer.redOutRedIn.value = -200f + Mathf.PingPong(Time.time * clrSpd * redRandom, 50f);
        channelMixer.greenOutGreenIn.value = -200f + Mathf.PingPong(Time.time * clrSpd * greenRandom, 50f);
        channelMixer.blueOutBlueIn.value = -200f + Mathf.PingPong(Time.time * clrSpd * blueRandom, 50f);
    }
}
