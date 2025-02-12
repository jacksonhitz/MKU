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
    Tutorial tutorial;

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
    public bool isIntro = false;

    public GameObject bat;
    public GameObject cia;
    public GameObject black;

    public GameObject healthBar;
    public GameObject focusBar;
    public GameObject ammo;

    public Dialogue intro;

    void Awake()
    {
        civs = new List<NPCAI>(FindObjectsOfType<NPCAI>()); 
        enemies = new List<Enemy>(); 
        CollectEnemies(); 

        soundManager = FindObjectOfType<SoundManager>(); 
        volume = FindObjectOfType<Volume>(); 
        ux = FindObjectOfType<UX>(); 
        tutorial = FindObjectOfType<Tutorial>(); 
        intro = FindObjectOfType<Dialogue>(); 
        player = GameObject.FindGameObjectWithTag("Player");
        cam = FindAnyObjectByType<CamController>();

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);
        camMat.SetFloat("_Speed", currentSpeed);
    }

    void Start()
    {
        intro.CallDialogue();
    }

    void CollectEnemies()
    {
        enemies.Clear(); 
        enemies.AddRange(FindObjectsOfType<Enemy>()); 
    }

    public void Tutorial()
    {
        cam.CallBlink();
    }

    public void Active()
    {
        healthBar.SetActive(true);
        focusBar.SetActive(true);
        cia.SetActive(false);
        bat.SetActive(true);

        isIntro = false;

        tutorial.CallDialogue();
    }

    public void Kill(Enemy enemy)
    {
        phase++;
        enemies.Remove(enemy);

        if (enemies.Count <= 2)
        {
            StartCoroutine(CleanUp());
        }
        else if (enemies.Count <= 1)
        {
            CallDead();
        }
    }

    IEnumerator CleanUp()
    {
        yield return new WaitForSeconds(10f);
        CallDead();
    }
    public void CallDead()
    {
        fadeOut = true;
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
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
            float currentLerp = camMat.GetFloat("_Lerp");
            float capLerp = 0.01f * phase;
            float currentFrequency = camMat.GetFloat("_Frequency");
            float capFrequency = 1f * phase;
            float currentAmplitude = camMat.GetFloat("_Amplitude");
            float capAmplitude = 0.02f * phase;
            float currentSpeed = camMat.GetFloat("_Speed");
            float capSpeed = 0.02f * phase;

            if (currentLerp < capLerp)
            {
                currentLerp += 0.00001f;
                camMat.SetFloat("_Lerp", currentLerp);
            }
            if (currentFrequency < capFrequency)
            {
                currentFrequency += 0.001f;
                camMat.SetFloat("_Frequency", currentFrequency);
            }
            if (currentAmplitude < capAmplitude)
            {
                currentAmplitude += 0.00002f;
                camMat.SetFloat("_Amplitude", currentAmplitude);
            }
            if (currentSpeed < capSpeed)
            {
                currentSpeed += 0.00002f;
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

    public void Score(int newScore)
    {
        Debug.Log("score");

        score += newScore;

        ux.Score();
        ux.PopUp(newScore);
    }
}
