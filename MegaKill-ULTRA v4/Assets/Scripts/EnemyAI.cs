using UnityEngine;
using UnityEngine.AI;
using System.Collections; 

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject playerObj;
    PlayerController player;

    public float yLevelThreshold = 1f;   

    EnemyGun enemyGun;      
    float detectionRange = 20f; 
    float fireRate = 1f;        
    float fovAngle = 110f; 

    GameManager gameManager;


    //public AudioClip gunShot;
    //public AudioSource sfx;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = FindAnyObjectByType<PlayerController>();

        gameManager = FindObjectOfType<GameManager>();

        StartCoroutine(FireRaycasts());
    }

    void Update()
    {
        if (Mathf.Abs(transform.position.y - playerObj.transform.position.y) <= yLevelThreshold)
        {
            agent.SetDestination(playerObj.transform.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    IEnumerator FireRaycasts()
    {
        while (true)
        {
            if (IsPlayerInRange() && HasLineOfSight())
            {
                if (gameManager.score > 0)
                {
                    enemyGun.Shoot(player.transform);
                    //sfx.clip = gunShot;
                    //sfx.Play();

                    yield return new WaitForSeconds(fireRate);
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, playerObj.transform.position) <= detectionRange;
    }

    bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle < fovAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    
}
