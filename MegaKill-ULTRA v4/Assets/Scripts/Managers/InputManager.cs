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
    void Update()
    {
        if (StateManager.state == StateManager.GameState.Title)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StateManager.state = StateManager.GameState.Intro;
                sceneLoader.Lvl();
            }
        }
        if (StateManager.state == StateManager.GameState.Lvl)
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
            else
            {
                settings.Resume();
            }
        }
    }
}
