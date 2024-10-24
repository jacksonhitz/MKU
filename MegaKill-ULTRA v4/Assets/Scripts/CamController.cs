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

    private Volume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorGrading;
    private LensDistortion lensDistortion;
    private ChannelMixer channelMixer;

    public float chromSpd = 2f;
    public float fovSpd = 1.2f;
    public float satSpd = 1f;
    public float clrSpd = 0.5f;
    public float lensSpd = 0.8f; 
    public float lensMin = -50f;  
    public float lensMax = 25f;   

    float redRandom;
    float greenRandom;
    float blueRandom;
    float redStart;
    float greenStart;
    float blueStart;

    private Camera cam;
    private float originalFOV;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;

        // Get the Volume component for URP
        postProcessVolume = GetComponent<Volume>();

        // Try to get each post-processing effect from the Volume
        postProcessVolume.profile.TryGet(out chromaticAberration);
        postProcessVolume.profile.TryGet(out colorGrading);
        postProcessVolume.profile.TryGet(out lensDistortion);
        postProcessVolume.profile.TryGet(out channelMixer);

        SetClr();
    }

    void SetClr()
    {
        redRandom = Random.Range(.75f, 1.25f);
        greenRandom = Random.Range(.75f, 1.25f);
        blueRandom = Random.Range(.75f, 1.25f);

        redStart = Random.Range(-200f, 0f);
        greenStart = Random.Range(-200, 0f);
        blueStart = Random.Range(-200f, 0f);

        // Adjust the color grading using hue shift instead of mixer channels (URP doesn't have RGB mixing)
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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);

        Trip();
    }

    void Trip()
    {
        // Chromatic Aberration
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Max(Mathf.PingPong(Time.time * chromSpd, 1f), 0.25f);

        // FOV change
        float fovChange = Mathf.Sin(Time.time * fovSpd) * 10f * postProcessVolume.weight;
        cam.fieldOfView = originalFOV + fovChange;

        // Saturation
        if (colorGrading != null)
            colorGrading.saturation.value = Mathf.PingPong(Time.time * satSpd, 100f) - 50f;

        // Lens Distortion
        if (lensDistortion != null)
        {
            float distortionValue = Mathf.PingPong(Time.time * lensSpd, lensMax - lensMin) + lensMin;
            lensDistortion.intensity.value = distortionValue;
        }

        ClrAdjuster();
        ClrMixer();
    }

    void ClrAdjuster()
    {
        // Adjust hue dynamically for effect similar to color mixing
        float red = -200f + Mathf.PingPong(Time.time * clrSpd * redRandom, 200f);
        float green = -200f + Mathf.PingPong(Time.time * clrSpd * greenRandom, 200f);
        float blue = -200f + Mathf.PingPong(Time.time * clrSpd * blueRandom, 200f);

        if (colorGrading != null)
        {
            // Adjust hue shift or any color grading property to simulate RGB mixing
            colorGrading.hueShift.value = Mathf.Lerp(red, green, blue); // You can adjust this as per your visual needs
        }
    }
    void ClrMixer()
    {
        float red = -200f + Mathf.PingPong(Time.time * clrSpd * redRandom, 200f);
        float green = -200f + Mathf.PingPong(Time.time * clrSpd * greenRandom, 200f);
        float blue = -200f + Mathf.PingPong(Time.time * clrSpd * blueRandom, 200f);

        channelMixer.redOutRedIn.value = red;
        channelMixer.greenOutGreenIn.value = green;
        channelMixer.blueOutBlueIn.value = blue;
    }
}
