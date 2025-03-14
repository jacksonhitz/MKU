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

    CamController cam;

    List<Item> items;
    public List<GameObject> doors;
    public GameObject cia;

    [HideInInspector] public bool fadeOut;

    void Awake()
    {
        Time.timeScale = 1f;
        
        items = new List<Item>();  

        soundManager = FindObjectOfType<SoundManager>(); 
        enemyManager = FindObjectOfType<EnemyManager>();
        inputManager = FindObjectOfType<InputManager>();

        ux = FindObjectOfType<UX>(); 
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
                StartLvl();
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
    public void StartIntro()
    {
        soundManager.music.Stop();
        ux.IntroOn();
    }
    public void EndIntro()
    {
        Active();
        cam.CallBlink();
    }
    public void StartTutorial()
    {
        StateManager.state = StateManager.GameState.Tutorial;

        soundManager.NewTrack();
        ux.TutorialOn();
    }
    public void EndTutorial()
    {
        StartLvl();
    }
    public void StartLvl()
    {
        enemyManager.Active();
        ux.UIOn();
        OpenDoors();

        StateManager.state = StateManager.GameState.Lvl;
    }
    public void Active()
    {
        soundManager.Play();
        cia.SetActive(false);
        CollectItems();
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
        Restart();
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
