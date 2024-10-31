using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class EnemyPatrol : MonoBehaviour
{
    GameObject player;
    Gun gun;
    NavMeshAgent agent;
   
    GameManager gameManager;
    
    [SerializeField] LayerMask groundLayer, playerLayer;

    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;
    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float chaseSpeed = 6.0f;
    [SerializeField] float fleeSpeed = 6.0f;
    //state change
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    bool playerInAttackRange;
    //[SerializeField] bool playerInSight;
    [SerializeField] bool isHostile;
    [SerializeField] Transform[] waypoints;
    int currentWaypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {  
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        gameManager = FindObjectOfType<GameManager>();
        gun = FindObjectOfType<Gun>();
        agent.speed = patrolSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (!isHostile) Patrol();
        if (isHostile && gameManager.score == 0) Patrol();
        if (isHostile && gameManager.score > 0) Chase();
        if (isHostile && playerInAttackRange && gameManager.score > 0) Attack();
    }
        

    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (!agent.hasPath || agent.remainingDistance < 2f)
        {
            SetNextWaypoint();
        }
    }

    void SetNextWaypoint()
    {
        if (waypoints.Length == 0) return;

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
     public void Flee()
    {
        agent.speed = fleeSpeed;

        // Flee in the opposite direction from the gunshot
        Vector3 fleeDirection = transform.position - player.transform.position;
        Vector3 fleeTarget = transform.position + fleeDirection.normalized * gun.scaredRad;

        agent.SetDestination(fleeTarget);
    }

}
