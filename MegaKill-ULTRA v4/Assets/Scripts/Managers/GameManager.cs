using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public int score = 0;

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

        player = GameObject.FindGameObjectWithTag("Player");
        cam = FindObjectOfType<CamController>();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }

    void StateChange(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.Title:
                Title();
                break;
            case StateManager.GameState.Intro:
                Intro();
                break;
            case StateManager.GameState.Tutorial:
                break;
            case StateManager.GameState.Lvl:
                // Handle level logic
                break;
            case StateManager.GameState.Outro:
                // Handle score screen logic
                break;
        }
    }

    void Title()
    {
        
    }
    void Intro()
    {
        CollectItems();
    }
    public void Tutorial()
    {
        StateManager.State = StateManager.GameState.Tutorial;
        StartCoroutine(Blink());
    }

    public void Lvl()
    {
        StateManager.State = StateManager.GameState.Lvl;
        
        if (enemyManager != null)
        {
            enemyManager.Active();
        }
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
        cam.CallFadeIn();
    }

    public void Exit()
    {
        fadeOut = true;
        StartCoroutine(ExitCoroutine());
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
        //ui.PopUp(newString);
    }
}
