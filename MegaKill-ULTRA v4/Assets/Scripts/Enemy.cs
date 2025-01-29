using UnityEngine;
using System.Collections; 

public class Enemy : MonoBehaviour
{
    public Transform player;     

    public EnemyGun enemyGun;      
    public float detectionRange = 20f; 
    public float fireRate = 1f;        
    public float fieldOfViewAngle = 110f; 

    GameManager gameManager;

    public Transform car;
    
    public AudioClip gunShot;
    public AudioSource sfx;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        gameManager = FindObjectOfType<GameManager>();

        StartCoroutine(FireRaycasts());
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
                    sfx.clip = gunShot;
                    sfx.Play();

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
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle < fieldOfViewAngle / 2)
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
