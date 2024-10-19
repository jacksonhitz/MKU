using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    float horzInput;
    float vertInput;
    float currentBrakeForce;
    float steerAngle;
    bool isBraking;

    public WheelCollider fl;
    public WheelCollider fr;
    public WheelCollider bl;
    public WheelCollider br;
    
    public Transform flT;
    public Transform frT;
    public Transform blT;
    public Transform brT;

    public float motorForce;
    public float brakeForce;
    public float maxSteer;

    public PlayerController player;

    float MaxAngularVelocity = 1000000000F;



    void Start()
    {
        fl.attachedRigidbody.maxAngularVelocity = MaxAngularVelocity;
        fr.attachedRigidbody.maxAngularVelocity = MaxAngularVelocity;
        bl.attachedRigidbody.maxAngularVelocity = MaxAngularVelocity;
        br.attachedRigidbody.maxAngularVelocity = MaxAngularVelocity;
    }



    void FixedUpdate()
    {
        if (player.currentState == PlayerController.State.driving)
        {
            Inputs();
        }
        Motor();
        Steer();
        Wheels();

        Debug.Log(vertInput);

    }

    void Inputs()
    {
        horzInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
    }
    
    void Motor()    
    {
        fl.motorTorque = vertInput * motorForce;
        fr.motorTorque = vertInput * motorForce;
        bl.motorTorque = vertInput * motorForce;
        br.motorTorque = vertInput * motorForce;
        Debug.Log(fl.motorTorque);


        currentBrakeForce = isBraking ? brakeForce : 0f;
        if (isBraking)
        {
            Brake();
        }
    }
    void Brake()
    {
        fl.brakeTorque = currentBrakeForce;
        fr.brakeTorque = currentBrakeForce;
        bl.brakeTorque = currentBrakeForce;
        br.brakeTorque = currentBrakeForce;
    }

    void Steer()
    {
        steerAngle = maxSteer * horzInput;
        fl.steerAngle = steerAngle;
        fr.steerAngle = steerAngle;
    }
    
    void Wheels()
    {
        Single(fl,flT);
        Single(fr,frT);
        Single(bl,blT);
        Single(br,brT);
    }
    void Single(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
