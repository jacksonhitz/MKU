using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpd;
    public float runDrag;
    public Transform orientation;
    float horzInput;
    float vertInput;
    Vector3 moveDir;
    Rigidbody rb;

    public bool isDriving;
    public Transform carPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        rb.drag = runDrag;
    }
    void Update()
    {
        if (!isDriving)
        {
            Move();
        }
        else
        {
            transform.position = carPos.position;
        }
    }

    void Move()
    {
        horzInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        if (vel.magnitude > moveSpd)
        {
            Vector3 clampedVel = vel.normalized * moveSpd;
            rb.velocity = new Vector3(clampedVel.x, rb.velocity.y, clampedVel.z);
        }

        moveDir = orientation.forward * vertInput + orientation.right * horzInput;
        rb.AddForce(moveDir.normalized * moveSpd * 10f, ForceMode.Force);

    }
}