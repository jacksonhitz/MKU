using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    float horzInput;
    float vertInput;

    bool isBraking;

    public float motorForce;
    public float brakeForce;
    public float maxSteer;
    public float carDrag;
    public float minTurnVel;

    PlayerController player;
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
    }
    void FixedUpdate()
    {
        if (player.isDriving)
        {
            Inputs();
        }

        Motor();
        Steer();
    }

    void Inputs()
    {
        horzInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
    }
    
    void Motor()
    {
        Vector3 forwardMovement = transform.forward * vertInput * motorForce;
        rb.AddForce(forwardMovement * Time.fixedDeltaTime, ForceMode.Force);
        
        if (vertInput == 0)
        {
            rb.velocity *= carDrag;
        }
    } 

    void Steer()
    {
        if (rb.velocity.magnitude > minTurnVel)
        {
            float steer = horzInput * maxSteer;
            transform.Rotate(0, steer * Time.fixedDeltaTime, 0);
        }
    }

    void Brake()
    {
        if (isBraking)
        {
            rb.velocity *= brakeForce;
        }
    }
}
