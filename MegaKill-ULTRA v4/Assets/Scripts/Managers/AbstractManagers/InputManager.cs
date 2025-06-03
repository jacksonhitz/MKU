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

        if (Input.GetKeyDown(KeyCode.B))
            StartCoroutine(StateManager.LoadState(StateManager.GameState.TANGO2, 0f));

        if (Input.GetKeyDown(KeyCode.Space))
            ScenesManager.Instance.isLvlOn = true;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("called pause");
            if (StateManager.State != StateManager.GameState.PAUSED)
                StartCoroutine(StateManager.LoadState(StateManager.GameState.PAUSED, 0f));
            else
            {
                StateManager.LoadSilent(StateManager.PREVIOUS);
            }
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}


