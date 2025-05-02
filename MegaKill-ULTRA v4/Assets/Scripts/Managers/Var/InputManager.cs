using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    ItemManager itemManager;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        itemManager = FindObjectOfType<ItemManager>();
    }

    void Update()
    {
        UpdateInputs();
    }

    void UpdateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (StateManager.State == StateManager.GameState.TITLE)
            {
                StateManager.LoadState(StateManager.GameState.INTRO);
            }
            else if (StateManager.State == StateManager.GameState.INTRO)
            {
                StateManager.LoadState(StateManager.GameState.TUTORIAL);
            }
            else if (StateManager.State == StateManager.GameState.TUTORIAL)
            {
                StateManager.LoadState(StateManager.GameState.TANGO);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateManager.State != StateManager.GameState.PAUSED)
            {
                StateManager.LoadState(StateManager.GameState.PAUSED);
            }
            else
            {
                // FIX LATER
                
            }
        }

        if (StateManager.IsActive())
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                itemManager?.HighlightAll();
            }
            else
            {
                itemManager?.HighlightItem();
            }
        }
    }
}
