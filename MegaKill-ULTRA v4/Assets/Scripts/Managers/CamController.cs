using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CamController : MonoBehaviour
{
    public float sens;
    public Transform player;

    private float xRotation;
    private float yRotation;

    public Volume volume;
    public Volume vig;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorGrading;
    private LensDistortion lensDistortion;
    private ChannelMixer channelMixer;
    private Vignette vignette;

    public float chromSpd = 2f;
    public float fovSpd = 1.2f;
    public float satSpd = 1f;
    public float clrSpd = 0.5f;
    public float lensSpd = 0.25f;

    float redRandom;
    float greenRandom;
    float blueRandom;
    float redStart;
    float greenStart;
    float blueStart;

    Camera cam;
    float originalFOV;

    private Vector3 originalPosition;
    public float swayIntensity = 0.1f;
    GameManager gameManager;
    Settings settings;

    // Shader variables
    public Material camMat;
    float currentLerp = 0.15f;
    float currentFrequency = 0;
    float currentAmplitude = 0;

    public int phase;

    private float targetSpeedX;
    private float targetSpeedY;
    private float lerpSpeed = 1f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        settings = FindObjectOfType<Settings>();
    }

    void Start()
    {
        
        if (camMat != null)
        {
            camMat.SetFloat("_Lerp", currentLerp);
            camMat.SetFloat("_Frequency", currentFrequency);
            camMat.SetFloat("_Amplitude", currentAmplitude);

            RandomizeSpeed();
        }

        if (settings != null) sens = settings.sens;


        originalFOV = cam.fieldOfView;
        originalPosition = transform.localPosition;

        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out colorGrading);
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out channelMixer);

        vig.profile.TryGet(out vignette);

        SetClr();

        if (colorGrading != null)
        {
            colorGrading.postExposure.value = -10f;
        }

        StartCoroutine(FadeIn(2f));
    }

    void Update()
    {
        if (settings != null)
        {
            if (!settings.isPaused)
            {
                MoveCam();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else
        {
            MoveCam();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        UpdatePost();
        UpdateShader();
    }

    public void UpPhase()
    {
        phase++;
        RandomizeSpeed();
    }

    void FadeOut()
    {
        Debug.Log("fade call");

        currentLerp += 0.025f;
        currentFrequency += 0.25f;

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
    }

    void UpdateShader()
    {
        float cap = 3f * phase;
        Debug.Log(cap);

        if (currentAmplitude < cap)
        {
            currentAmplitude += 0.001f;
        }
        if (currentFrequency < cap)
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

    public void CallBlink()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        StartCoroutine(FadeOut(0.5f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn(0.5f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeOut(0.5f));
        yield return new WaitForSeconds(0.5f);
        gameManager.StartTutorial();
        StartCoroutine(FadeIn(0.5f));
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

        colorGrading.hueShift.value = redStart;

        channelMixer.redOutRedIn.value = redStart;
        channelMixer.greenOutGreenIn.value = greenStart;
        channelMixer.blueOutBlueIn.value = blueStart;
    }

    void MoveCam()
    {
        float mouseX = (Input.GetAxis("Mouse X") * Time.deltaTime * sens) / Time.timeScale;
        float mouseY = (Input.GetAxis("Mouse Y") * Time.deltaTime * sens) / Time.timeScale;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.9f, 89.9f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void UpdatePost()
    {
        // Chromatic Aberration
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Clamp(Mathf.Max(Mathf.PingPong(Time.time * chromSpd, 1f), 0.25f), 0, 5f);
        }

        // FOV change
        float fovChange = Mathf.Sin(Time.time * fovSpd) * 3f * volume.weight;
        cam.fieldOfView = originalFOV + fovChange;

        // Saturation
        if (colorGrading != null)
            colorGrading.saturation.value = Mathf.PingPong(Time.time * satSpd, 100f) - 50f;

        // Lens Distortion
        if (lensDistortion != null)
        {
            float distortionValue = Mathf.PingPong(Time.time * lensSpd, 1.5f) - 0.75f;
            lensDistortion.intensity.value = distortionValue;
        }

        // Camera Sway
        float swayAmountX = Mathf.Sin(Time.time * 2f) * swayIntensity * volume.weight;
        float swayAmountY = Mathf.Cos(Time.time * 2f) * swayIntensity * volume.weight;
        transform.localPosition = originalPosition + new Vector3(swayAmountX, swayAmountY, 0);

        // Camera Rot Sway
        float rotationSwayX = Mathf.Sin(Time.time * 1.5f) * swayIntensity * 0.5f * volume.weight;
        float rotationSwayY = Mathf.Cos(Time.time * 1.5f) * swayIntensity * 0.5f * volume.weight;
        transform.localRotation = Quaternion.Euler(rotationSwayX, rotationSwayY, 0) * Quaternion.Euler(xRotation, 0f, 0f);

        ClrAdjuster();
        ClrMixer();
    }

    public void Damaged(Vector3 damagePos)
    {
        vignette.center.value = new Vector2(0.5f, 0.5f);

        Vector3 damageDirection = -(damagePos - cam.transform.position).normalized;

        Vector3 cameraForward = cam.transform.forward;

        float angle = Vector3.SignedAngle(cameraForward, damageDirection, Vector3.up);

        float xOffset = Mathf.Clamp(Mathf.Sin(angle * Mathf.Deg2Rad), -1f, 1f);
        float yOffset = Mathf.Clamp(Mathf.Cos(angle * Mathf.Deg2Rad), -1f, 1f);

        float xCenter = Mathf.Clamp01(0.5f + xOffset * 0.5f);
        float yCenter = Mathf.Clamp01(0.5f + yOffset * 0.5f);

        vignette.center.value = new Vector2(xCenter, yCenter);
        vignette.intensity.value = .75f;

        StartCoroutine(FadeOutVig(2f));
    }

    private IEnumerator FadeOutVig(float duration)
    {
        float startIntensity = vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, 0f, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = 0f;
    }

    void ClrAdjuster()
    {
        float red = -200f + Mathf.PingPong(Time.time * clrSpd * redRandom, 200f);
        float green = -200f + Mathf.PingPong(Time.time * clrSpd * greenRandom, 200f);
        float blue = -200f + Mathf.PingPong(Time.time * clrSpd * blueRandom, 200f);

        if (colorGrading != null)
        {
            colorGrading.hueShift.value = Mathf.Lerp(red, green, blue);
        }
    }

    void ClrMixer()
    {
        float red = -200f + Mathf.PingPong(Time.time * clrSpd * redRandom, 50f);
        float green = -200f + Mathf.PingPong(Time.time * clrSpd * greenRandom, 50f);
        float blue = -200f + Mathf.PingPong(Time.time * clrSpd * blueRandom, 50f);

        channelMixer.redOutRedIn.value = red;
        channelMixer.greenOutGreenIn.value = green;
        channelMixer.blueOutBlueIn.value = blue;
    }
}
