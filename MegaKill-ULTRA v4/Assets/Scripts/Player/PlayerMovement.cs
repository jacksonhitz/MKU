using UnityEngine;

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

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector2 moveDir, bool jump)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 movement = transform.right * moveDir.x + transform.forward * moveDir.y;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        if (jump && isGrounded && !isRooted)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (!isRooted)
        {
            Vector3 finalMove = (movement * runSpd) + Vector3.up * verticalVelocity;
            characterController.Move(finalMove * Time.deltaTime);
        }
    }
}
