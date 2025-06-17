using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateBase();
        UpdateItems();
        UpdatePlayer();
    }

    void UpdateBase()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneScript.Instance?.State == StateManager.SceneState.FILE)
                SceneScript.Instance.StartLevel();
            if (SceneScript.Instance?.State == StateManager.SceneState.SCORE)
                StateManager.LoadNext();
        }

        if (Input.GetKeyDown(KeyCode.Z))
            SceneScript.Instance?.EndLevel();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Mathf.Approximately(Time.timeScale, 1))
                SettingsManager.Instance.Pause();
            else
                SettingsManager.Instance.Resume();
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}
