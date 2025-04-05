using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    void Awake()
    {
        if (Instance != null || this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "TITLE")
        {
            StateManager.State = StateManager.GameState.Title;
        }
    }

    void Update()
    {
        Debug.Log(StateManager.State);
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
    public void Title()
    {
        SceneManager.LoadScene(0);
        StateManager.State = StateManager.GameState.Title;
    }

    public void Subway()
    {
        SceneManager.LoadScene(1);
        StateManager.State = StateManager.GameState.Intro;
    }

    public void Outro()
    {
        SceneManager.LoadScene(2);
        StateManager.State = StateManager.GameState.Outro;
        // CHANGE LATER
    }

    public void Paused()
    {
        StateManager.State = StateManager.GameState.Paused;
    }

    public void Previous()
    {
        StateManager.State = StateManager.Previous;
    }
}
