using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    SceneLoader sceneLoader;
    Settings settings;
    [HideInInspector] public GameManager gameManager;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sceneLoader = FindObjectOfType<SceneLoader>();
        settings = FindObjectOfType<Settings>();
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

                break;
            case StateManager.GameState.Intro:

                break;
            case StateManager.GameState.Tutorial:

                break;
            case StateManager.GameState.Lvl:

                break;
            case StateManager.GameState.Outro:
                
                break;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (StateManager.State == StateManager.GameState.Title)
            {
                sceneLoader.Subway();
            }
            if (StateManager.State == StateManager.GameState.Intro)
            {
                gameManager.Tutorial();
            }
            if (StateManager.State == StateManager.GameState.Tutorial)
            {
                gameManager.Lvl();
            }

        }
        
        
        
        if (StateManager.State == StateManager.GameState.Title)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                sceneLoader.Subway();
            }
        }
        
        if (StateManager.State == StateManager.GameState.Lvl)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                sceneLoader.Restart();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settings.isPaused)
            {
                settings.Pause();
            }
        }

        if (StateManager.State == StateManager.GameState.Intro || StateManager.State == StateManager.GameState.Tutorial)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //SkipIntro();
            }
        } 

        if (StateManager.State == StateManager.GameState.Lvl)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                //HighlightAll();
            }
            else
            {
                //HighlightItem();
            }
        }
    }
}
