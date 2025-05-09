using UnityEngine;

public class PlayerInputs : InputManager
{
    ItemManager itemManager;
    PlayerController playerController;

    void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
        playerController = FindObjectOfType<PlayerController>();

        if (itemManager == null)
            Debug.LogWarning("ItemManager not found");
        if (playerController == null)
            Debug.LogWarning("PlayerController not found");
    }

    protected override void UpdateItems()
    {
        if (StateManager.IsActive())
        {
            if (Input.GetKey(KeyCode.Tab))
                itemManager?.HighlightAll();
            else
                itemManager?.HighlightItem();
        }
    }

    protected override void UpdatePlayer()
    {
        if (StateManager.IsActive())
        {
            // MOVE/JUMP
            Vector2 moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            bool jump = Input.GetKeyDown(KeyCode.Space);

            playerController?.Move(moveDir, jump);


            // USE/SHOOT
            if (Input.GetMouseButton(0))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    playerController?.UseRight();
                else
                    playerController?.UseLeft();
            }
            else if (Input.GetMouseButton(1))
            {
                playerController?.UseRight();
            }


            // LEFT/RIGHT
            if (Input.GetKeyDown(KeyCode.Q))
                playerController?.Left();

            if (Input.GetKeyDown(KeyCode.E))
                playerController?.Right();

        }
    }
}


