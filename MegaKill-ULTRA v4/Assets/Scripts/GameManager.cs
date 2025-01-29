using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int phase;

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

    public GameObject barrier;


    void Start()
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

        StartCoroutine(UpPhase());
    }

    public void ScareNPCs()
    {
        foreach (NPCAI civ in civs)
        {
            civ.CallFlee(player.transform.position);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count <= 3)
        {
            barrier.SetActive(false);
            Debug.Log("freedom!");
        }
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
