using UnityEngine;

public class PlayerInputs : InputManager
{
    InteractionManager interacts;
    PlayerController controller;

    void Awake()
    {
        interacts = FindObjectOfType<InteractionManager>();
        controller = FindObjectOfType<PlayerController>();
    }

    protected override void UpdateItems()
    {
        if (StateManager.IsActive())
        {
            if (Input.GetKey(KeyCode.Tab))
                interacts.isHighlightAll = true;
            else
                interacts.isHighlightAll = false;
        }
    }

    protected override void UpdatePlayer()
    {
        if (StateManager.IsActive())
        {
            Debug.Log("MOVING");

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


