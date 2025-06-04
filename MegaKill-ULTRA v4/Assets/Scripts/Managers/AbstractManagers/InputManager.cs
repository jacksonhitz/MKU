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

        Debug.Log(StateManager.State);
    }

    void UpdateBase()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StateManager.NextState(this);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            if (StateManager.State == StateManager.GameState.FILE)
                StartCoroutine(StateManager.LoadState(StateManager.lvl, 2f));


        if (Input.GetKeyDown(KeyCode.B))
            StateManager.State = StateManager.GameState.TANGO2;
        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(StateManager.LoadState(StateManager.GameState.SCORE, 2f));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
                SettingsManager.Instance.Pause();
            else
                SettingsManager.Instance.Resume();
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}


