using UnityEngine;

public class PlayerInputs : InputManager
{
    ItemManager itemManager;
    PlayerController controller;

    void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
        controller = FindObjectOfType<PlayerController>();


        if (itemManager == null)
            Debug.LogWarning("ItemManager not found");
        if (controller == null)
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

            controller.movement.Move(moveDir, jump);

            //INTERACT
            if (Input.GetKeyDown(KeyCode.F))
                controller.interact?.Interact();


            // USE/SHOOT
            if (Input.GetMouseButton(0))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    controller.items?.UseRight();
                else
                    controller.items?.UseLeft();
            }
            else if (Input.GetMouseButton(1))
            {
                controller.items?.UseRight();
            }

            // LEFT/RIGHT
            if (Input.GetKeyDown(KeyCode.Q))
                controller.items?.Left();

            if (Input.GetKeyDown(KeyCode.E))
                controller.items?.Right();
        }
    }
}


