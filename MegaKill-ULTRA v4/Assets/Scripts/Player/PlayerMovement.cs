using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float runSpd;
    public float gravity;
    public float groundDistance;
    public float jumpHeight = 5f;
    public Transform groundCheck;
    public LayerMask groundMask;

    public bool isGrounded;
    public bool isRooted;

    float verticalVelocity;
    CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        InputManager.PlayerActionMap.Jump.performed += JumpOnPerformed;
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        Jump();
    }

    private void OnDestroy()
    {
        InputManager.PlayerActionMap.Jump.performed -= JumpOnPerformed;
    }

    private void Update()
    {
        if (StateManager.IsPassive)
            return;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Move(InputManager.PlayerActionMap.Move.ReadValue<Vector2>());
    }

    private void Jump()
    {
        if (isGrounded && !isRooted)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Move(Vector2 moveDir)
    {
        Vector3 movement = transform.right * moveDir.x + transform.forward * moveDir.y;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        if (isRooted)
            return;

        Vector3 finalMove = (movement * runSpd) + Vector3.up * verticalVelocity;
        characterController.Move(finalMove * Time.deltaTime);
    }
}
