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
    public bool isTutorialScene;

    void Awake()
    {
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

        enemyHolder.SetActive(false);

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
            StartIntro();
        }
        else
        {
            StartLvl();
        }
    }
    void Update()
    {
        //UpdateShader();

        if (Input.GetKeyDown(KeyCode.Return) && isIntro)
        {
            StartLvl();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            //Restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                //Pause();
            }
            else
            {
                //Unpause();
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

    public void PlayerActive()
    {
        if (cia != null)
        {
            cia.SetActive(false);
        }
        CollectItems();
        isIntro = false;
    }
    public void StartLvl()
    {
        PlayerActive();

        ux.UIOn();
        enemyManager.Active();
        OpenDoors();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0.0000000001f;
        ux.Paused();
        soundManager.Paused();
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        ux.UnPaused();
        soundManager.UnPaused();
        isPaused = false;
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(2);
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
        fadeOut = true;
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(3f);
        Restart();
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
    }

    public void Score(int newScore)
    {
        Debug.Log("score");

        score += newScore;

        ux.PopUp(newScore);
    }
}
