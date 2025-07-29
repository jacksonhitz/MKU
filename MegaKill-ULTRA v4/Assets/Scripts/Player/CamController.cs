using System;
using System.Collections;
using KBCore.Refs;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class CamController : MonoBehaviour
{
    private const float DefaultFrequency = 10f;
    private const float DefaultAmplitude = 2f;
    private const float DefaultLerp = 0.1f;
    private const float PhaseFrequencyFactor = 5f;
    private static readonly int Frequency = Shader.PropertyToID("_Frequency");
    private static readonly int Amplitude = Shader.PropertyToID("_Amplitude");
    private static readonly int Lerp = Shader.PropertyToID("_Lerp");
    private static readonly int SpeedX = Shader.PropertyToID("_SpeedX");
    private static readonly int SpeedY = Shader.PropertyToID("_SpeedY");

    float xRotation;
    float yRotation;

    ChromaticAberration chromaticAberration;
    ColorAdjustments colorGrading;
    ChannelMixer channelMixer;

    float mixerSpd;
    float hueSpd;
    float fovSpd;

    float redRandom;
    float greenRandom;
    float blueRandom;
    float redStart;
    float greenStart;
    float blueStart;

    float originalFOV;

    Vector3 originalPosition;

    float currentLerp;
    float currentFrequency;
    float currentAmplitude;

    [Header("Shader")]
    [SerializeField]
    private int inactiveScenePhase = 1;

    [SerializeField]
    private int phase = 5;

    private int defaultPhase;

    float targetSpeedX;
    float targetSpeedY;

    [SerializeField, Range(0, 0.001f)]
    private float lerpSpeed = 0.0001f;

    [SerializeField, Range(0f, 0.1f)]
    private float swayIntensity = 0.01f;

    [SerializeField, Range(0f, 1f)]
    private float chromSpd = 0.25f;

    [Header("References")]
    [SerializeField, Parent]
    private PlayerController player;

    [SerializeField, Self]
    private Camera cam;

    [SerializeField, Self(Flag.Editable)]
    private Volume dynamicVolume;

    [SerializeField, Self(Flag.Editable)]
    private Volume staticVolume;

    [SerializeField, Required]
    private Material camMat;

    void Start()
    {
        defaultPhase = phase;
        Reset();
    }

    void Reset()
    {
        SetEffects();
        SetClr();
    }

    void SetEffects()
    {
        currentLerp = DefaultLerp;

        //PHASE 1 BY DEFAULT
        currentFrequency = DefaultFrequency;
        currentAmplitude = DefaultAmplitude;

        camMat.SetFloat(Lerp, currentLerp);
        camMat.SetFloat(Frequency, currentFrequency);
        camMat.SetFloat(Amplitude, currentAmplitude);

        RandomizeSpeed();

        originalFOV = cam.fieldOfView;
        originalPosition = transform.localPosition;

        staticVolume.profile.TryGet(out chromaticAberration);
        staticVolume.profile.TryGet(out colorGrading);
        dynamicVolume.profile.TryGet(out channelMixer);
    }

    void OnEnable()
    {
        SceneScript.StateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        SceneScript.StateChanged -= OnStateChanged;
    }

    void OnStateChanged(StateManager.SceneState state)
    {
        if (state is StateManager.SceneState.PLAYING)
        {
            StartCoroutine(Blink());
            cam.clearFlags = CameraClearFlags.Skybox;
            phase = defaultPhase;
            Reset();
        }

        if (state is StateManager.SceneState.FILE or StateManager.SceneState.SCORE)
        {
            phase = inactiveScenePhase;
            currentFrequency = phase * PhaseFrequencyFactor;
            currentAmplitude = phase;
            cam.backgroundColor = Color.black;
            cam.clearFlags = CameraClearFlags.Color;
        }
    }

    void Update()
    {
        if (StateManager.IsTransition)
            TransitionOn();
        else
        {
            UpdateShader();
            UpdatePost();
        }

        MoveCheck();
    }

    void MoveCheck()
    {
        if (StateManager.IsActive && !SettingsManager.IsPaused)
        {
            MoveCam();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator Blink()
    {
        yield return null;
        CallFadeOut();
        yield return new WaitForSeconds(0.2f);
        CallFadeIn();
        yield return new WaitForSeconds(0.2f);
        CallFadeOut();
        yield return new WaitForSeconds(0.2f);
        CallFadeIn();
    }

    public void UpPhase()
    {
        // phase++;
    }

    void TransitionOn()
    {
        currentAmplitude += .1f;
        currentFrequency += .1f;
        currentLerp += 0.001f;

        camMat.SetFloat(Frequency, currentFrequency);
        camMat.SetFloat(Amplitude, currentFrequency);
        camMat.SetFloat(Lerp, currentLerp);
    }

    void UpdateShader()
    {
        if (currentAmplitude < phase)
        {
            currentAmplitude += lerpSpeed;
        }
        else if (currentAmplitude > phase)
        {
            currentAmplitude -= lerpSpeed;
        }
        if (currentFrequency < phase * PhaseFrequencyFactor)
        {
            currentFrequency += lerpSpeed;
        }
        else if (currentFrequency > phase * PhaseFrequencyFactor)
        {
            currentFrequency -= lerpSpeed;
        }

        camMat.SetFloat(Lerp, currentLerp);
        camMat.SetFloat(Frequency, currentFrequency);
        camMat.SetFloat(Amplitude, currentAmplitude);

        // float speedX = Mathf.Lerp(camMat.GetFloat("_SpeedX"), targetSpeedX, lerpSpeed);
        // float speedY = Mathf.Lerp(camMat.GetFloat("_SpeedY"), targetSpeedY, lerpSpeed);

        // camMat.SetFloat("_SpeedX", speedX);
        // camMat.SetFloat("_SpeedY", speedY);
    }

    void RandomizeSpeed()
    {
        targetSpeedX = Random.Range(-0.01f, 0.01f);
        targetSpeedY = Random.Range(-0.01f, 0.01f);

        camMat.SetFloat(SpeedX, targetSpeedX);
        camMat.SetFloat(SpeedY, targetSpeedY);
    }

    public void CallFadeIn()
    {
        StartCoroutine(FadeIn(0.2f));
    }

    public void CallFadeOut()
    {
        StartCoroutine(FadeOut(0.2f));
    }

    IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        float initialExposure = -10f;
        float targetExposure = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            colorGrading.postExposure.value = Mathf.Lerp(
                initialExposure,
                targetExposure,
                elapsed / duration
            );
            yield return null;
        }

        colorGrading.postExposure.value = targetExposure;
    }

    IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;
        float initialExposure = 0f;
        float targetExposure = -10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            colorGrading.postExposure.value = Mathf.Lerp(
                initialExposure,
                targetExposure,
                elapsed / duration
            );
            yield return null;
        }

        colorGrading.postExposure.value = targetExposure;
    }

    void SetClr()
    {
        redRandom = Random.Range(25f, 50f);
        greenRandom = Random.Range(25f, 50f);
        blueRandom = Random.Range(25f, 50f);

        redStart = Random.Range(-200f, -100f);
        greenStart = Random.Range(-200f, -100f);
        blueStart = Random.Range(-200f, -100f);

        channelMixer.redOutRedIn.value = redStart;
        channelMixer.greenOutGreenIn.value = greenStart;
        channelMixer.blueOutBlueIn.value = blueStart;
    }

    void MoveCam()
    {
        float sens = SettingsManager.Instance != null ? SettingsManager.Instance.Sensitivity : 500f;

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * (sens / 2f) / Time.timeScale;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * (sens / 2f) / Time.timeScale;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.9f, 89.9f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void UpdatePost()
    {
        chromaticAberration.intensity.value = Mathf.PingPong(Time.time * chromSpd, 1f);

        float fovChange = Mathf.Sin(Time.time * fovSpd) * dynamicVolume.weight;
        cam.fieldOfView = originalFOV + fovChange;

        float swayAmountX = Mathf.Sin(Time.time * 2f) * swayIntensity * dynamicVolume.weight;
        float swayAmountY = Mathf.Cos(Time.time * 2f) * swayIntensity * dynamicVolume.weight;
        transform.localPosition = originalPosition + new Vector3(swayAmountX, swayAmountY, 0);

        float rotationSwayX =
            Mathf.Sin(Time.time * 1.5f) * swayIntensity * 0.5f * dynamicVolume.weight;
        float rotationSwayY =
            Mathf.Cos(Time.time * 1.5f) * swayIntensity * 0.5f * dynamicVolume.weight;
        transform.localRotation =
            Quaternion.Euler(rotationSwayX, rotationSwayY, 0) * Quaternion.Euler(xRotation, 0f, 0f);

        ClrHue();
        ClrMixer();

        if (dynamicVolume.weight < 5)
        {
            dynamicVolume.weight += 0.00001f;
        }

        fovSpd = 0.05f * dynamicVolume.weight;
        mixerSpd = 5f * dynamicVolume.weight;
        hueSpd = dynamicVolume.weight / 5;
    }

    void ClrHue()
    {
        float hue = Mathf.PingPong(
            Time.time * hueSpd * (redRandom + greenRandom + blueRandom) / 3f,
            360f
        );
        colorGrading.hueShift.value = Mathf.Lerp(-180f, 180f, hue / 360f);
    }

    void ClrMixer()
    {
        channelMixer.redOutRedIn.value = redStart + Mathf.PingPong(Time.time * mixerSpd, redRandom);
        channelMixer.greenOutGreenIn.value =
            greenStart + Mathf.PingPong(Time.time * mixerSpd, greenRandom);
        channelMixer.blueOutBlueIn.value =
            blueStart + Mathf.PingPong(Time.time * mixerSpd, blueRandom);
    }

    private void OnDestroy()
    {
        Reset();
    }
}
