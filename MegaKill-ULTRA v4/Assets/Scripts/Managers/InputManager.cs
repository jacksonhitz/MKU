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
                gameManager.Intro();
            }
            if (StateManager.State == StateManager.GameState.Intro)
            {
                gameManager.Tutorial();
            }
            if (StateManager.State == StateManager.GameState.Tutorial)
            {
                gameManager.Lvl();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gameManager.Restart();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateManager.State == StateManager.GameState.Paused)
            {
                gameManager.Paused();
            }
            else
            {
                gameManager.Unpaused();
            }
        }


        if (StateManager.State == StateManager.GameState.Lvl)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                gameManager.HighlightAll();
            }
            else
            {
                gameManager.HighlightItem();
            }
        }
    }
}
