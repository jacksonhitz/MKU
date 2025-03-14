using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    SoundManager soundManager;
    Settings settings;

    [HideInInspector] public bool transition;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        soundManager = FindObjectOfType<SoundManager>();
        settings = FindObjectOfType<Settings>();
    }

    void Update()
    {
        Debug.Log("state" + StateManager.state);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        soundManager.Play();
        settings.menu.enabled = false;
    }
    

    public void Title()
    {
        SceneManager.LoadScene(0);
        StateManager.state = StateManager.GameState.Title;
        soundManager.ResetMusic();
        settings.menu.enabled = false;

    }

    public void Lvl()
    {
        SceneManager.LoadScene(1);
        soundManager.Play();
        settings.menu.enabled = false;
    }

    public void Score()
    {
        SceneManager.LoadScene(2);
        StateManager.state = StateManager.GameState.Score;
        soundManager.Play();
        settings.menu.enabled = false;
    }
}
