using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int phase;

    public int score = 0;

    Volume volume;
    UX ux;
    GameObject player;
    SoundManager soundManager;

    List<NPCAI> civs;
    List<Enemy> enemies;

    public Pointer pointer;
    public CamController cam;
    public Material camMat;

    float currentLerp;
    float currentFrequency;
    float currentAmplitude;
    float currentSpeed;

    public bool fadeOut;



    void Awake()
    {
        civs = new List<NPCAI>(FindObjectsOfType<NPCAI>()); 
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>()); 
        soundManager = FindObjectOfType<SoundManager>(); 
        volume = FindObjectOfType<Volume>(); 
        ux = FindObjectOfType<UX>(); 
        player = GameObject.FindGameObjectWithTag("Player");

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);
        camMat.SetFloat("_Speed", currentSpeed);

        //StartCoroutine(UpPhase());

        ux.gameObject.SetActive(true);
    }

    public void ScareNPCs()
    {
        foreach (NPCAI civ in civs)
        {
            civ.CallFlee(player.transform.position);
        }
    }

    void Update()
    {
        if (!fadeOut)
        {
            float currentLerp = 0.0125f * phase; 
            camMat.SetFloat("_Lerp", currentLerp); 
            float currentFrequency = 1.25f * phase; 
            camMat.SetFloat("_Frequency", currentFrequency); 
            float currentAmplitude = 0.025f * phase; 
            camMat.SetFloat("_Amplitude", currentAmplitude); 
            float currentSpeed = 0.025f * phase; 
            camMat.SetFloat("_Speed", currentSpeed); 
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

    IEnumerator UpPhase()
    {
        while (true)
        {
            phase++;
            Debug.Log("Phase: " + phase);
            yield return new WaitForSeconds(30f);
        }
    }

    public void Score(int newScore)
    {
        Debug.Log("score");

        score += newScore;
        
        ux.Score();
        ux.PopUp(newScore);
    }
}
