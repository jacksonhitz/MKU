using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    GameObject[] civilians;

    int phase;

    public Volume volume;

    public int score = 0;

    public UX ux;

    public SoundManager soundManager;

    public Pointer pointer;
    public CamController cam;

    public Material camMat;
    float currentLerp;
    float currentFrequency;
    float currentAmplitude;
    float currentSpeed;

    float tempLerp;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        volume = FindAnyObjectByType<Volume>();
        ux = FindAnyObjectByType<UX>();

        UpPhase();

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);
        camMat.SetFloat("_Speed", currentSpeed);
    }

    void Update()
    {
        float currentLerp = camMat.GetFloat("_Lerp");
        float capLerp = 0.05f * phase; 
        float currentFrequency = camMat.GetFloat("_Frequency");
        float capFrequency = 5f * phase; 
        float currentAmplitude = camMat.GetFloat("_Amplitude");
        float capAmplitude = 0.1f * phase; 
        float currentSpeed = camMat.GetFloat("_Speed");
        float capSpeed = 0.1f * phase; 
        
        if (currentLerp < capLerp)
        {
            currentLerp += 0.0001f; 
            camMat.SetFloat("_Lerp", currentLerp); 
        }
        if (currentFrequency < capFrequency)
        {
            currentFrequency += 0.01f; 
            camMat.SetFloat("_Frequency", currentFrequency); 
        }
        if (currentAmplitude < capAmplitude)
        {
            currentAmplitude += 0.0002f; 
            camMat.SetFloat("_Amplitude", currentAmplitude); 
        }
        if (currentSpeed < capSpeed)
        {
            currentSpeed += 0.0002f; 
            camMat.SetFloat("_Speed", currentSpeed); 
        }
    }


    public void UpPhase()
    {
        phase++;
        ChooseTarget();
    }

    public void Score(int newScore)
    {
        Debug.Log("score");

        score += newScore;
        
        ux.Score();
        ux.PopUp(newScore);
    }

    public void ChooseTarget()
    {
        civilians = GameObject.FindGameObjectsWithTag("Civilian");
        Debug.Log("civs: " + civilians.Length);
        
        int randomIndex = Random.Range(0, civilians.Length);
        GameObject target = civilians[randomIndex];

        Civilian targetScript = target.GetComponent<Civilian>();
        
        targetScript.target = true;
        pointer.target = target.transform;
    }
    
}
