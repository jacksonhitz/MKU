using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    List<NPCAI> civs;

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

    public bool fadeOut;

    GameObject player;

    void Start()
    {
        civs = new List<NPCAI>(FindObjectsOfType<NPCAI>()); 
        soundManager = FindObjectOfType<SoundManager>(); 
        volume = FindObjectOfType<Volume>(); 
        ux = FindObjectOfType<UX>(); 
        player = GameObject.FindGameObjectWithTag("Player");

        UpPhase();

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);
        camMat.SetFloat("_Speed", currentSpeed);
    }

    public void ScareNPCs()
    {
        foreach(NPCAI civ in civs)
        {
            
            civ.CallFlee(player.transform.position);
        }
    }

    void Update()
    {
        if (!fadeOut)
        {
            float capLerp = 0.05f * phase; 
            float capFrequency = 5f * phase; 
            float capAmplitude = 0.1f * phase; 
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
            currentLerp += 0.0025f;
            currentFrequency += 0.025f;

            camMat.SetFloat("_Lerp", currentLerp); 
            camMat.SetFloat("_Frequency", currentFrequency); 
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
        }
    }

    IEnumerator Win()
    {
        fadeOut = true;

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(2);
    }

    public void Score(int newScore)
    {
        Debug.Log("score");

        score += newScore;
        
        ux.Score();
        ux.PopUp(newScore);
    }
}