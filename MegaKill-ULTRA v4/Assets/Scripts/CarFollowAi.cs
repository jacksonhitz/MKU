using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollowAI : MonoBehaviour
{
    public Transform target;
    public CarController carController; 
    public float targetRadius = 2f; 
    public float maxDistance = 10f;
    public float brakeDistance = 3f;
    public PlayerController playerController;
    void FixedUpdate()
    {
        // Check if player is driving car or not
        if (playerController.currentState != PlayerController.State.driving)
        {
            FollowTarget();
        }
    }

    void FollowTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        directionToTarget.Normalize();

        // Adjust speed based on the distance to target
        float speedFactor = Mathf.Clamp(distanceToTarget / maxDistance, 0, 1);
        carController.vertInput = speedFactor;

        // Calculate the angle difference between the car's forward direction and the target direction
        float steerInput = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up) / 45f;
        carController.horzInput = Mathf.Clamp(steerInput, -1f, 1f);

        // Apply braking when the car gets close to the target
        if (distanceToTarget <= brakeDistance)
        {
            carController.isBraking = true;
        }
        else
        {
            carController.isBraking = false;
            // Adjust speed based on the distance to target
            speedFactor = Mathf.Clamp(distanceToTarget / maxDistance, 0, 1);
            carController.vertInput = speedFactor;

            // Calculate the angle difference between the car's forward direction and the target direction
            steerInput = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up) / 45f;
            carController.horzInput = Mathf.Clamp(steerInput, -1f, 1f);
        }
    }
}
