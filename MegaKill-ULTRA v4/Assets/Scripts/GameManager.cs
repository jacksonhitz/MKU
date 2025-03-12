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
    EnemyManager enemyManager;
    BulletTime bulletTime;
    Settings settings;

    List<Item> items;
    public List<GameObject> doors;

    public Pointer pointer;
    public CamController cam;
    public Material camMat;

    float currentLerp;
    float currentFrequency;
    float currentAmplitude;
    float currentSpeed;

    public GameObject enemyHolder;
    public GameObject eye;
    public GameObject cia;
    public GameObject black;

    public Dialogue intro;

    public bool fadeOut;
    public bool isIntro;
    public bool isPaused;

    void Awake()
    {
        Time.timeScale = 1f;
        
        settings = FindObjectOfType<Settings>();
        if (settings.isTutorial)
        {
            isIntro = true;
        }
        else
        {
            isIntro = false;
        }
        
        items = new List<Item>();  

        soundManager = FindObjectOfType<SoundManager>(); 
        enemyManager = FindObjectOfType<EnemyManager>();
        volume = FindObjectOfType<Volume>(); 
        ux = FindObjectOfType<UX>(); 
        bulletTime = FindObjectOfType<BulletTime>();
        player = GameObject.FindGameObjectWithTag("Player");
        cam = FindAnyObjectByType<CamController>();

        camMat.SetFloat("_Lerp", currentLerp);
        camMat.SetFloat("_Frequency", currentFrequency);
        camMat.SetFloat("_Amplitude", currentAmplitude);
        camMat.SetFloat("_Speed", currentSpeed);

        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }
    }
    void Start()
    {
        CollectItems();
        if (isIntro)
        {
            settings.isTutorial = false;
            StartIntro();
        }
        else
        {
            StartLvl();
        }
    }
    void Update()
    {
        UpdateShader();

        if (Input.GetKeyDown(KeyCode.Return) && isIntro)
        {
            StartLvl();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isIntro)
            {
                if (!isPaused)
                {
                    Pause();
                }
                else
                {
                    Unpause();
                    ux.UIOn();
                }
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            HighlightAll();
        }
        else
        {
            HighlightItem();
        }
    }
    public void StartIntro()
    {
        ux.IntroOn();
        ux.UIOff();
    }
    public void EndIntro()
    {
        PlayerActive();
        cam.CallBlink();
    }
    public void StartTutorial()
    {
        ux.TutorialOn();
        ux.UIOn();
    }
    public void StartLvl()
    {
        PlayerActive();

        ux.UIOn();
        enemyManager.Active();
        OpenDoors();
    }
    public void PlayerActive()
    {
        if (cia != null)
        {
            cia.SetActive(false);
        }
        CollectItems();
        isIntro = false;
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0.0000000001f;
        ux.Paused();
        //soundManager.Paused();
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        ux.UnPaused();
        //soundManager.UnPaused();
    }

    public void Exit()
    {
        StartCoroutine(ExitCoroutine());
    }
    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(false);
        }
    }

    public void CollectItems()
    {
        items.Clear(); 
        items.AddRange(FindObjectsOfType<Item>()); 
    }

    public void HighlightAll()
    {
        if (!isIntro)
        {
            foreach (Item item in items)
            {
                item.GlowMat();
            }
        }
    }
    public void HighlightItem()
    {
        foreach (Item item in items)
        {
            if (!item.isHovering || !item.available)
            {
                item.DefaultMat();
            }
            else
            {
                item.GlowMat();
            }
        }
    }
    

    public void Kill(Enemy enemy)
    {
        phase++;
    }

    public void CallDead()
    {
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        fadeOut = true;
        Time.timeScale = 1f;
        
        yield return new WaitForSeconds(3f);
        Restart();
    }

    IEnumerator ExitCoroutine()
    {
        fadeOut = true;
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
        Debug.Log("Called");
    }

    void UpdateShader()
    {
        if (fadeOut)
        {
            float accLerp = camMat.GetFloat("_Lerp");
            float accFrequency = camMat.GetFloat("_Frequency");

            accLerp += 0.025f;
            accFrequency += 0.25f;

            camMat.SetFloat("_Lerp", accLerp);
            camMat.SetFloat("_Frequency", accFrequency);
        }
        else
        {
            camMat.SetFloat("_Lerp", currentLerp);
            camMat.SetFloat("_Frequency", currentFrequency);
            camMat.SetFloat("_Amplitude", currentAmplitude);
            camMat.SetFloat("_Speed", currentSpeed);
        }
    }

    public void Score(int newScore)
    {
        string newString = newScore.ToString();
        ux.PopUp(newString);
    }
}
