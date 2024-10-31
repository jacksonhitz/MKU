using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    GameManager gameManager;

    [SerializeField] LayerMask groundLayer, playerLayer;

    // Patrol
    [SerializeField] Transform[] waypoints; // Array to store waypoints
    int currentWaypointIndex = 0;
    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float chaseSpeed = 6.0f;

    // State change
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    bool playerInAttackRange;
    [SerializeField] bool isHostile;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        gameManager = FindObjectOfType<GameManager>();
        agent.speed = patrolSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!isHostile) Patrol();
        if (isHostile && gameManager.score == 0) Patrol();
        if (isHostile && gameManager.score > 0) Chase();
        if (isHostile && playerInAttackRange && gameManager.score > 0) Attack();
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            SetNextWaypoint();
        }
    }

    void SetNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        // Select a random waypoint
        currentWaypointIndex = Random.Range(0, waypoints.Length);
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Attack()
    {
        agent.SetDestination(transform.position);
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.transform.position);
    }
}
