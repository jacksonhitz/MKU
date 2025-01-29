using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject playerObj;
    PlayerController player;
    float detectionRange = 50f;
    float pathfindingRange = 50f;
    float followDistanceX = 5f;
    float followDistanceZ = 5f;

    GameManager gameManager;
    bool detectedPlayer = false;
    public bool los = false;
    bool isFiring = false;

    public GameObject blood;
    public AudioClip squelch;
    AudioSource sfx;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = FindAnyObjectByType<PlayerController>();
        sfx = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);

        if ((distanceToPlayer <= pathfindingRange || detectedPlayer) && los)
        {
            detectedPlayer = true;

            Vector3 targetPosition = playerObj.transform.position;
            Vector3 directionToPlayer = targetPosition - transform.position;

            Vector3 lookDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            float clampedX = Mathf.Clamp(transform.position.x + directionToPlayer.x, targetPosition.x - followDistanceX, targetPosition.x + followDistanceX);
            float clampedZ = Mathf.Clamp(transform.position.z + directionToPlayer.z, targetPosition.z - followDistanceZ, targetPosition.z + followDistanceZ);

            Vector3 destination = new Vector3(clampedX, transform.position.y, clampedZ);
            agent.SetDestination(destination);
        }
        else
        {
            agent.ResetPath();
        }

        LOS();
    }

    void LOS()
    {
        if (playerObj == null) return;

        Vector3 direction = (playerObj.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                los = true;
                Debug.Log("los");
            }
            else
            {
                los = false;
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
