using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    void Update()
    {
        UpdateBase();
        UpdateItems();
        UpdatePlayer();
    }

    void UpdateBase()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StateManager.NextState();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateManager.State != StateManager.GameState.PAUSED)
                StateManager.LoadState(StateManager.GameState.PAUSED);
            else
            {
                StateManager.SilentState(StateManager.PREVIOUS);
            }
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}


