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

    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;
    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float chaseSpeed = 6.0f;
    [SerializeField] float fleeSpeed = 6.0f;

    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    bool playerInAttackRange = false;
    [SerializeField] bool isHostile;
    
    public bool cop;
    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        gameManager = FindObjectOfType<GameManager>();
        gun = FindObjectOfType<Gun>();
        agent.speed = patrolSpeed;

        if (cop)
        {
            InvokeRepeating(nameof(HostilityCheck), 20f, 20f);
        }
    }

    void Update()
    {
        playerInAttackRange = Vector3.Distance(transform.position, player.transform.position) <= attackRange;

        if (!isHostile) Patrol();
        if (isHostile && gameManager.score == 0) Patrol();
        if (isHostile && gameManager.score > 0) Chase();
        if (isHostile && playerInAttackRange && gameManager.score > 0) Attack();

        RotateDir();

        if (!cop)
        {
            UpdateAnim();
        }
    }

    void HostilityCheck()
    {
        // 50% chance to toggle hostility
        isHostile = Random.Range(0, 4) == 0;
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (!walkpointSet) DestSearch();
        if (walkpointSet) agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }

    void DestSearch()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
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

        Vector3 fleeDirection = transform.position - player.transform.position;
        Vector3 fleeTarget = transform.position + fleeDirection.normalized * gun.scaredRad;

        agent.SetDestination(fleeTarget);
    }

    void RotateDir()
    {
        Vector3 velocity = agent.velocity;
        if (velocity.magnitude > 0.1f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void UpdateAnim()
    {
        bool isMoving = agent.velocity.magnitude > 0.1f;
        if (!isMoving)
        {
            animator.speed = 0;
        }
        else
        {
            animator.speed = 1;
        }
    }
}
