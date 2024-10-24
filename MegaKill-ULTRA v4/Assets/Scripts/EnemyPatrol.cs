using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
public class EnemyPatrol : MonoBehaviour
{
    GameObject player;

    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;

    //state change
    [SerializeField] float sightRange;
    bool playerInSight;
    [SerializeField] bool isHostile;

    // Start is called before the first frame update
    void Start()
    {  
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        if (agent.isActiveAndEnabled) Patrol(); 
        if (playerInSight && isHostile) Chase();
    }
        

    void Patrol() // Enemy Patrols the map, navigating to random points on the map within a certain distance
    {
        if (!walkpointSet) DestSearch();
        if (walkpointSet) agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }
    void DestSearch() // Enemy searches for a certain destination witihin a specified range
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if(Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void Chase() // Enemy Chases the player around the map
    {
        agent.SetDestination(player.transform.position);
    }
}
