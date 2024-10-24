using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFollowAI : MonoBehaviour
{
    public Transform target;

    public CarController carController;
    public PlayerController playerController;

    public float targetRadius = 2f;
    public float maxDistance = 10f;
    public float brakeDistance = 3f;
    public float turnSpeedReductionFactor = 0.5f; // Factor to reduce speed when turning
    
    private bool isReversing = false;
    private float reverseDuration = 2f;
    private float reverseTimer = 0f;

    void FixedUpdate()
    {
        // Check if player is driving car or not
        if (playerController.currentState != PlayerController.State.driving || playerController.currentCar != transform)
        {
            FollowTarget();  // AI takes control if the player isn't driving this car
        }
    }

    void FollowTarget()
    {
        if (isReversing)
        {
            ReverseAndTurn();
            return;
        }

        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        directionToTarget.Normalize();

        // Calculate the angle difference between the car's forward direction and the target direction
        float steerInput = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up) / 45f;

        // Reduce speed if turning
        float turnFactor = 1f - Mathf.Abs(steerInput) * turnSpeedReductionFactor;
        turnFactor = Mathf.Clamp(turnFactor, 0.3f, 1f); // Set minimum speed to avoid complete stop

        // Adjust speed based on the distance to target and turn factor
        float speedFactor = Mathf.Clamp(distanceToTarget / maxDistance, 0, 1) * turnFactor;
        carController.vertInput = speedFactor;

        // Set the steering input
        carController.horzInput = Mathf.Clamp(steerInput, -1f, 1f);

        // Apply braking when the car gets close to the target
        if (distanceToTarget <= brakeDistance)
        {
            carController.isBraking = true;
        }
        else
        {
            carController.isBraking = false;
        }
    }

    void ReverseAndTurn()
    {
        reverseTimer += Time.deltaTime;
        carController.vertInput = -1f; // Reverse
        carController.horzInput = Random.Range(-1f, 1f); // Randomly turn to avoid obstacles

        if (reverseTimer >= reverseDuration)
        {
            isReversing = false;
            reverseTimer = 0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision happened at the front of the car
        Vector3 collisionDirection = collision.contacts[0].point - transform.position;
        float angle = Vector3.Angle(transform.forward, collisionDirection);

        // Avoid reversing if the collision was with a ramp, a civilian, player's car, or from ground
        if (collision.gameObject.CompareTag("Ramp") || collision.gameObject.CompareTag("Civilian") || collision.gameObject.CompareTag("NPC")|| Vector3.Dot(collision.contacts[0].normal, Vector3.up) > 0.5f)
        {
            return;
        }

        if (angle < 45f) // Collision is in front of the car
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);
            if (distanceToPlayer > targetRadius)
            {
                isReversing = true;
            }
        }
    }
}
