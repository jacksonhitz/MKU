using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CamController : MonoBehaviour
{
    Camera cam;

    float xRotation;
    float yRotation;

    [SerializeField] Volume dynamicVolume;
    [SerializeField] Volume staticVolume;

    ChromaticAberration chromaticAberration;
    ColorAdjustments colorGrading;
    ChannelMixer channelMixer;

    float chromSpd = 0.25f;
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
    float swayIntensity = 0.01f;

    public Material camMat;
    float currentLerp = 0;
    float currentFrequency = 0;
    float currentAmplitude = 0;

    public int phase;

    float targetSpeedX;
    float targetSpeedY;
    float lerpSpeed = 0.001f;

    PlayerController player;

    void Awake()
    {
        cam = GetComponent<Camera>();
        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        Reset();
        Blink();
    }

    void Reset()
    {
        phase = 5;
        SetEffects();
        SetClr();
    }

    void SetEffects()
    {
        currentLerp = 0.1f;

        //PHASE 1 BY DEFAULT
        currentFrequency = 10f;
        currentAmplitude = 2f;

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);

        RandomizeSpeed();

        originalFOV = cam.fieldOfView;
        originalPosition = transform.localPosition;

        staticVolume.profile.TryGet(out chromaticAberration);
        staticVolume.profile.TryGet(out colorGrading);
        dynamicVolume.profile.TryGet(out channelMixer);
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
        if (StateManager.IsActive() && StateManager.IsScene())
            StartCoroutine(Blink());

        switch (state)
        {
            case StateManager.GameState.SCORE: Reset(); break;
        }
    }

    void Update()
    {
        if (StateManager.State == StateManager.GameState.TRANSITION) TransitionOn();
        else
        {
            UpdateShader();
            UpdatePost();
        }

        MoveCheck();
    }

    void MoveCheck()
    {
        if (StateManager.IsActive() && Time.timeScale == 1)
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

        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentFrequency);
        camMat.SetFloat("_Lerp", currentLerp);
    }

    void UpdateShader()
    {
        if (currentAmplitude < phase)
        {
            currentAmplitude += 0.0001f;
        }
        if (currentFrequency < phase * 5)
        {
            currentFrequency += 0.0001f;
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
            colorGrading.postExposure.value = Mathf.Lerp(initialExposure, targetExposure, elapsed / duration);
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
            colorGrading.postExposure.value = Mathf.Lerp(initialExposure, targetExposure, elapsed / duration);
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

        float rotationSwayX = Mathf.Sin(Time.time * 1.5f) * swayIntensity * 0.5f * dynamicVolume.weight;
        float rotationSwayY = Mathf.Cos(Time.time * 1.5f) * swayIntensity * 0.5f * dynamicVolume.weight;
        transform.localRotation = Quaternion.Euler(rotationSwayX, rotationSwayY, 0) * Quaternion.Euler(xRotation, 0f, 0f);

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
        float hue = Mathf.PingPong(Time.time * hueSpd * (redRandom + greenRandom + blueRandom) / 3f, 360f);
        colorGrading.hueShift.value = Mathf.Lerp(-180f, 180f, hue / 360f);
    }

    void ClrMixer()
    {
        channelMixer.redOutRedIn.value = redStart + Mathf.PingPong(Time.time * mixerSpd, redRandom);
        channelMixer.greenOutGreenIn.value = greenStart + Mathf.PingPong(Time.time * mixerSpd, greenRandom);
        channelMixer.blueOutBlueIn.value = blueStart + Mathf.PingPong(Time.time * mixerSpd, blueRandom);
    }
}
