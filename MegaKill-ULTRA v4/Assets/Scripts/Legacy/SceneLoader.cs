using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    SoundManager soundManager;

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
    }

    void Update()
    {
        Debug.Log("state" + StateManager.state);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Title()
    {
        SceneManager.LoadScene(0);
        StateManager.state = StateManager.GameState.Title;
        soundManager.ResetMusic();
    }

    public void Lvl()
    {
        SceneManager.LoadScene(1);
    }

    public void Score()
    {
        SceneManager.LoadScene(2);
        StateManager.state = StateManager.GameState.Score;
    }
}
