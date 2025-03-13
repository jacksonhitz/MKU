using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int score = 0;

    Volume volume;
    UX ux;
    GameObject player;
    SoundManager soundManager;
    EnemyManager enemyManager;
    BulletTime bulletTime;
    Settings settings;

    CamController cam;

    List<Item> items;
    public List<GameObject> doors;


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
        cam = FindObjectOfType<CamController>();

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
        fadeOut = true;
        StartCoroutine(ExitCoroutine());
    }
    public void Restart()
    {
        fadeOut = true;
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
        enemyManager.EnemyDead(enemy);
        cam.UpPhase();
        Score(100);
    }

    public void CallDead()
    {
        fadeOut = true;
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(3f);
        Restart();
    }

    IEnumerator ExitCoroutine()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
        Debug.Log("Called");
    }

    void Score(int newScore)
    {
        string newString = newScore.ToString();
        ux.PopUp(newString);
    }
}
