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
            {
                StateManager.State = StateManager.lvl;
            }
 
        if (Input.GetKeyDown(KeyCode.B))
            StateManager.State = StateManager.GameState.TANGO2;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("called pause");
            if (StateManager.State != StateManager.GameState.PAUSED)
                StateManager.State = StateManager.GameState.PAUSED;
            else
                StateManager.SilentState = StateManager.previous;
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}


