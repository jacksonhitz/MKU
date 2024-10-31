using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject[] civilians;

    int phase;

    public Volume volume;

    public int score = 0;

    public UX ux;

    public SoundManager soundManager;
    public PlayerController player;

    public Pointer pointer;
    public CamController cam;

    public Material camMat;
    float currentLerp;
    float currentFrequency;
    float currentAmplitude;
    float currentSpeed;

    float tempLerp;

    public bool fadeOut;

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
        if (!fadeOut)
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
        else
        {
            float accLerp = camMat.GetFloat("_Lerp");
            float accFrequency = camMat.GetFloat("_Frequency");

            accLerp += 0.0025f;
            accFrequency += 0.025f;

            camMat.SetFloat("_Lerp", accLerp); 
            camMat.SetFloat("_Frequency", accFrequency); 
        }
    }


    public void UpPhase()
    {
        if (phase == 5)
        {
            StartCoroutine(Win());
        }
        else
        {
            phase++;
            ChooseTarget();

        }
    }
    IEnumerator Win()
    {
        fadeOut = true;

        yield return new WaitForSeconds (5f);

        SceneManager.LoadScene(2);
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
        if (civilians == null || civilians.Length == 0)
        {
            Debug.LogWarning("No civilians found");
            return;
        }

        Debug.Log("civs: " + civilians.Length);
        
        int randomIndex = Random.Range(0, civilians.Length);
        GameObject target = civilians[randomIndex];

        Civilian targetScript = target.GetComponent<Civilian>();
        if (targetScript == null)
        {
            Debug.LogWarning("No Civilian component found on target");
            return;
        }
        
        targetScript.target = true;
        if (pointer != null)
        {
            pointer.target = target.transform;
        }
        else
        {
            Debug.LogWarning("Pointer not found");
        }
    }
    
}
