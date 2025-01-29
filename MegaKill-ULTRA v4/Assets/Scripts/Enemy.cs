using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject playerObj;
    PlayerController player;

    EnemyGun enemyGun;
    float detectionRange = 50f;
    float pathfindingRange = 50f;
    float followDistanceX = 5f;
    float followDistanceZ = 5f;
    float fireRate; 

    GameManager gameManager;
    bool detectedPlayer = false;
    bool seesPlayer = false;
    bool isFiring = false;

    public GameObject blood;
    public AudioClip squelch;
    public AudioClip gunshot;
    AudioSource sfx;

    public bool melee;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = FindAnyObjectByType<PlayerController>();
        enemyGun = GetComponentInChildren<EnemyGun>();
        sfx = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();

        fireRate = Random.Range(1f, 3f); 
        StartCoroutine(FireRaycastRoutine());
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);

        if ((distanceToPlayer <= pathfindingRange || detectedPlayer) && seesPlayer)
        {
            detectedPlayer = true;

            Vector3 targetPosition = playerObj.transform.position;
            Vector3 directionToPlayer = targetPosition - transform.position;

            float clampedX = Mathf.Clamp(transform.position.x + directionToPlayer.x, targetPosition.x - followDistanceX, targetPosition.x + followDistanceX);
            float clampedZ = Mathf.Clamp(transform.position.z + directionToPlayer.z, targetPosition.z - followDistanceZ, targetPosition.z + followDistanceZ);

            Vector3 destination = new Vector3(clampedX, transform.position.y, clampedZ);
            agent.SetDestination(destination);
        }
        else
        {
            agent.ResetPath();
        }
    }

    IEnumerator FireRaycastRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            FireRaycast();
        }
    }

    void FireRaycast()
    {
        if (playerObj == null) return;

        Vector3 direction = (playerObj.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!melee)
                {
                    enemyGun.Shoot(playerObj.transform);
                    sfx.clip = gunshot;
                    sfx.Play();
                }
                seesPlayer = true;
            }
        }
    }
    public void Hit()
    {
        Instantiate(blood, transform.position, Quaternion.identity);
        sfx.clip = squelch;
        sfx.Play();

        gameManager.RemoveEnemy(this);

        this.gameObject.SetActive(false);
        gameManager.Score(100);
    }
}
