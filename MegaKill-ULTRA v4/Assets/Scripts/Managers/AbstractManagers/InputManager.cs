using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
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
            StateManager.NextState();
        }

        if (Input.GetKeyDown(KeyCode.B)) StateManager.LoadState(StateManager.GameState.TANGO2);
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            Debug.Log("called pause");
            if (StateManager.State != StateManager.GameState.PAUSED)
                StateManager.LoadState(StateManager.GameState.PAUSED);
            else
            {
                StateManager.LoadSilent(StateManager.PREVIOUS);
            }
        }
    }

    protected abstract void UpdateItems();
    protected abstract void UpdatePlayer();
}


