using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;

    UX ux;
    GameObject player;
    SoundManager soundManager;
    EnemyManager enemyManager;
    InputManager inputManager;
    SceneLoader sceneLoader;
    CamController cam;

    List<Item> items;
    List<GameObject> doors = new List<GameObject>(); 
    public GameObject cia;

    [HideInInspector] public bool fadeOut;

    void Awake()
    {
        Time.timeScale = 1f;
        
        items = new List<Item>();  

        soundManager = FindObjectOfType<SoundManager>(); 
        enemyManager = FindObjectOfType<EnemyManager>();
        inputManager = FindObjectOfType<InputManager>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        ux = FindObjectOfType<UX>(); 
        player = GameObject.FindGameObjectWithTag("Player");
        cam = FindObjectOfType<CamController>();

        doors.AddRange(GameObject.FindGameObjectsWithTag("Door"));

        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }
    }

    void Start()
    {
        CollectItems();
        if (StateManager.state == StateManager.GameState.Intro)
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
        if (StateManager.state == StateManager.GameState.Intro || StateManager.state == StateManager.GameState.Tutorial)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SkipIntro();
            }
        } 

        if (StateManager.state == StateManager.GameState.Lvl)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                HighlightAll();
            }
            else
            {
                HighlightItem();
            }
        }
    }

    void SkipIntro()
    {
        PrepLvl();
        StartLvl();
    }
    void PrepLvl()
    {
        soundManager.NewTrack();
        CollectItems();
        cia.SetActive(false);
    }

    public void StartIntro()
    {
        soundManager.music.Stop();
        ux.IntroOn();
    }
    public void EndIntro()
    {
        StartCoroutine(Blink());
    }
    public void StartTutorial()
    {
        StateManager.state = StateManager.GameState.Tutorial;
        
        PrepLvl();
        ux.TutorialOn();
    }
    public void EndTutorial()
    {
        StartLvl();
    }
    public void StartLvl()
    {
        StateManager.state = StateManager.GameState.Lvl;
        
        OpenDoors();

        ux.TutorialOff();
        ux.IntroOff();
        ux.UIOn();

        cia.SetActive(false);
        
        enemyManager.Active();

        Debug.Log("LVL STARTED");
    }
    IEnumerator Blink()
    {
        cam.CallFadeOut();
        yield return new WaitForSeconds(0.5f);
        cam.CallFadeIn();
        yield return new WaitForSeconds(0.5f);
        cam.CallFadeOut();
        cia.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StartTutorial();
        cam.CallFadeIn();
    }

    public void Exit()
    {
        fadeOut = true;
        StartCoroutine(ExitCoroutine());
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
        foreach (Item item in items)
        {
            item.GlowMat();
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
        sceneLoader.Restart();
    }

    IEnumerator ExitCoroutine()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1f);
        
        Debug.Log("Called");
    }

    void Score(int newScore)
    {
        string newString = newScore.ToString();
        ux.PopUp(newString);
    }
}
