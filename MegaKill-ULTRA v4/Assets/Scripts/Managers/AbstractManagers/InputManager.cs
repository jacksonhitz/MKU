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
            if (StateManager.State == StateManager.GameState.TITLE)
                StateManager.LoadState(StateManager.GameState.INTRO);
            else if (StateManager.State == StateManager.GameState.INTRO)
                StateManager.LoadState(StateManager.GameState.TUTORIAL);
            else if (StateManager.State == StateManager.GameState.TUTORIAL)
                StateManager.LoadState(StateManager.GameState.TANGO);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateManager.State != StateManager.GameState.PAUSED)
                StateManager.LoadState(StateManager.GameState.PAUSED);
            else
            {
                // Handle unpausing or returning to previous state
            }
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}


