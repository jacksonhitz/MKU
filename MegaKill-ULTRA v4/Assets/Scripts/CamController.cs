using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CamController : MonoBehaviour
{
    public float sensX = 100f;
    public float sensY = 100f;
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

    public Material mat;
    private Vector3 originalPosition; 
    public float swayIntensity = 0.1f;


    public void UpPhase(int phase)
    {
        swayIntensity += 0.1f;

    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;
        originalPosition = transform.localPosition; 

        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out colorGrading);
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out channelMixer);

        vig.profile.TryGet(out vignette);

        SetClr();
    }

    void SetClr()
    {
        redRandom = Random.Range(.75f, 1.25f);
        greenRandom = Random.Range(.75f, 1.25f);
        blueRandom = Random.Range(.75f, 1.25f);

        redStart = Random.Range(-200f, -150f);
        greenStart = Random.Range(-200f, -150f);
        blueStart = Random.Range(-200f, -150f);

        colorGrading.hueShift.value = redStart;

        channelMixer.redOutRedIn.value = redStart;
        channelMixer.greenOutGreenIn.value = greenStart;
        channelMixer.blueOutBlueIn.value = blueStart;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.9f, 89.9f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);

        Trip();
    }

    void Trip()
    {
        // Chromatic Aberration
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Clamp(Mathf.Max(Mathf.PingPong(Time.time * chromSpd, 1f), 0.25f), 0, 5f);
        }

        // FOV change
        float fovChange = Mathf.Sin(Time.time * fovSpd) * 10f * volume.weight;
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

        // Camera Sway based on volume weight
        float swayAmountX = Mathf.Sin(Time.time * 2f) * swayIntensity * volume.weight;
        float swayAmountY = Mathf.Cos(Time.time * 2f) * swayIntensity * volume.weight;
        transform.localPosition = originalPosition + new Vector3(swayAmountX, swayAmountY, 0);

        // Camera Rotation Sway
        float rotationSwayX = Mathf.Sin(Time.time * 1.5f) * swayIntensity * 0.5f * volume.weight; // Slight pitch tilt
        float rotationSwayY = Mathf.Cos(Time.time * 1.5f) * swayIntensity * 0.5f * volume.weight; // Slight yaw tilt
        transform.localRotation = Quaternion.Euler(rotationSwayX, rotationSwayY, 0) * Quaternion.Euler(xRotation, 0f, 0f);

        ClrAdjuster();
        ClrMixer();
    }


    public void Damaged(Vector3 damagePos)
    {
        // Reset the vignette center
        vignette.center.value = new Vector2(0.5f, 0.5f); // Centered initially

        // Calculate the direction based on the camera's forward direction
        Vector3 damageDirection = -(damagePos - cam.transform.position).normalized;

        // Get the camera's forward direction
        Vector3 cameraForward = cam.transform.forward;
        
        // Calculate the signed angle between the camera's forward direction and the damage direction
        float angle = Vector3.SignedAngle(cameraForward, damageDirection, Vector3.up);
        
        // Calculate the offsets based on the angle
        float xOffset = Mathf.Clamp(Mathf.Sin(angle * Mathf.Deg2Rad), -1f, 1f);  // X-axis movement based on damage direction
        float yOffset = Mathf.Clamp(Mathf.Cos(angle * Mathf.Deg2Rad), -1f, 1f);  // Y-axis movement based on damage direction

        // Normalize the offsets to a 0-1 range for vignette center
        float xCenter = Mathf.Clamp01(0.5f + xOffset * 0.5f);  // Reduced influence for x-axis
        float yCenter = Mathf.Clamp01(0.5f + yOffset * 0.5f);  // Increased influence for y-axis

        // Set the vignette center based on calculated offsets
        vignette.center.value = new Vector2(xCenter, yCenter);
        vignette.intensity.value = .6f;  

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

        vignette.intensity.value = 0f; // Ensure it ends exactly at 0
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
