using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CamController : MonoBehaviour
{
    public float sensX = 100f; 
    public float sensY = 100f; 
    public Transform player;

    private float xRotation; 
    private float yRotation;

    private PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    private ColorGrading colorGrading;
    private LensDistortion lensDistortion;

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

        postProcessVolume = GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);
        postProcessVolume.profile.TryGetSettings(out colorGrading);
        postProcessVolume.profile.TryGetSettings(out lensDistortion);

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

        colorGrading.mixerRedOutRedIn.value = redStart;
        colorGrading.mixerGreenOutGreenIn.value = greenStart;
        colorGrading.mixerBlueOutBlueIn.value = blueStart; 
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
        chromaticAberration.intensity.value = Mathf.Max(Mathf.PingPong(Time.time * chromSpd, 1f), 0.25f);
        cam.fieldOfView = originalFOV + Mathf.Sin(Time.time * fovSpd) * 10f;
        colorGrading.saturation.value = Mathf.PingPong(Time.time * satSpd, 100f) - 50f;

        float distortionValue = Mathf.PingPong(Time.time * lensSpd, lensMax - lensMin) + lensMin;
        lensDistortion.intensity.value = distortionValue;

        ClrMixer();
    }

    void ClrMixer()
    {
        float red = -200f + Mathf.PingPong(Time.time * clrSpd * redRandom, 200f);   
        float green = -200f + Mathf.PingPong(Time.time * clrSpd * greenRandom, 200f);
        float blue = -200 + Mathf.PingPong(Time.time * clrSpd * blueRandom, 200f);

        colorGrading.mixerRedOutRedIn.value = red;
        colorGrading.mixerGreenOutGreenIn.value = green;
        colorGrading.mixerBlueOutBlueIn.value = blue;   
    }
}
