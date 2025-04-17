using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    Settings settings;
    GameManager gameManager;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameManager = FindObjectOfType<GameManager>();
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
            case StateManager.GameState.Outro:
                break;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            gameManager.Intro();
            
            if (StateManager.State == StateManager.GameState.Intro)
            {
                gameManager.Tutorial();
            }
            if (StateManager.State == StateManager.GameState.Tutorial)
            {
                gameManager.Fight();
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("paused");
            if (StateManager.State != StateManager.GameState.Paused)
            {
                gameManager.Paused();
            }
            else
            {
                settings.Unpaused();
            }
        }

        Debug.Log("is active: " + StateManager.IsActive());

        if (StateManager.IsActive())
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
