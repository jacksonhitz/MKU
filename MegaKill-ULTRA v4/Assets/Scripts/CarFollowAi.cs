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
    public float turnSpeedReductionFactor = 0.5f; // Factor to reduce speed when turning
    public PlayerController playerController;

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
        carController.horzInput = Random.Range(-1f, 1f); // Randomly turn to avoid the wall

        if (reverseTimer >= reverseDuration)
        {
            isReversing = false;
            reverseTimer = 0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);
        if (distanceToPlayer > targetRadius)
        {
            isReversing = true;
        }
    }
}
