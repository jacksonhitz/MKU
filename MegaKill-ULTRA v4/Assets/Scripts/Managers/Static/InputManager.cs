using UnityEngine;

public static class InputManager
{
    static ItemManager itemManager;
    static Settings settings;

    public static void UpdateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (StateManager.State == StateManager.GameState.Title)
            {
                StateManager.Intro();
            }
            else if (StateManager.State == StateManager.GameState.Intro)
            {
                StateManager.Tutorial();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateManager.State != StateManager.GameState.Paused)
            {
                StateManager.Paused();
            }
            else
            {
                settings?.Unpaused();

                //broken
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
