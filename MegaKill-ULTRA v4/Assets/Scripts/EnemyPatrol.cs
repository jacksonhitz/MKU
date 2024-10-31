using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
public class EnemyPatrol : MonoBehaviour
{
    GameObject player;

    NavMeshAgent agent;
   
    GameManager gameManager;
    
    [SerializeField] LayerMask groundLayer, playerLayer;

    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;
    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float chaseSpeed = 6.0f;
    //state change
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    bool playerInAttackRange;
    //[SerializeField] bool playerInSight;
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
        if (!walkpointSet) DestSearch();
        if (walkpointSet) agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }
    void DestSearch()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if(Physics.Raycast(destPoint, Vector3.down, groundLayer))
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
}
