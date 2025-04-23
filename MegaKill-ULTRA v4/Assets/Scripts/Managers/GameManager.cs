using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int score = 0;

    SoundManager soundManager;
    EnemyManager enemyManager;
    InputManager inputManager;
    CamController cam;
    Settings settings;
    UEye uEye;
    Crosshair crosshair;

    List<Item> items;
    List<GameObject> doors = new List<GameObject>(); 
    public GameObject cia;

    [HideInInspector] public bool fadeOut;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Time.timeScale = 1f;
        
        items = new List<Item>();  

        soundManager = FindObjectOfType<SoundManager>(); 
        enemyManager = FindObjectOfType<EnemyManager>();
        inputManager = FindObjectOfType<InputManager>();
        settings = FindObjectOfType<Settings>();

        uEye = FindObjectOfType<UEye>();
        crosshair = FindObjectOfType<Crosshair>();
    }
    void Start()
    {
        StateManager.State = StateManager.GameState.Title;
    }
    void Update()
    {
        Debug.Log(StateManager.State);
    }

    public void Title()
    {
       // SceneManager.LoadScene(0);
       // StateManager.State = StateManager.GameState.Title;
    }
    public void Intro()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex + 1);
       // StateManager.State = StateManager.GameState.Intro;
    }
    public void Testing()
    {
        StateManager.State = StateManager.GameState.Testing;
    }
    public void Tutorial()
    {
        StateManager.State = StateManager.GameState.Tutorial;
    }
    public void Launch()
    {
        StateManager.State = StateManager.GameState.Launch;
    }
    public void Tango()
    {
        StateManager.State = StateManager.GameState.Tango;
    }
    public void Fight()
    {
        StateManager.State = StateManager.GameState.Fight;
    }
    public void Outro()
    {
        SceneManager.LoadScene(2);
        StateManager.State = StateManager.GameState.Outro;
    }
    public void Paused()
    {
        StateManager.State = StateManager.GameState.Paused;
        Time.timeScale = 0f;
    }
    public void Unpaused()
    {
        StateManager.State = StateManager.Previous;
        Time.timeScale = 1f;

        if (soundManager != null)
        {
            soundManager.Unpaused();
        }
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Win()
    {
        Title();
        // CHANGE LATER
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
            if (item.isHovering && item.currentState == Item.ItemState.Available)
            {
                item.GlowMat();
            }
            else
            {
                item.DefaultMat();
            }
        }
    }

    public void CallDead()
    {
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(3f);
        Restart();
    }

    

    void Score(int newScore)
    {
        string newString = newScore.ToString();
        //ui.PopUp(newString);
    }
}
