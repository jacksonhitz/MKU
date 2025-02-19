using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject playerObj;
    PlayerController player;
    GameManager gameManager;
    SoundManager soundManager;


    float detectionRange = 30f;
    float pathfindingRange = 30f;
    float attackRange;
    

    bool detectedPlayer = false;
    public bool los = false;
    bool isFiring = false;

    public GameObject blood;
    public GameObject weaponPrefab; 
    public AudioClip rangedSquelch;
    public AudioClip meleeSquelch;

    AudioSource sfx;
    public Animator animator;
    public GameObject model;

    public bool ranged;

    public Item item;

    public bool isDead  = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = FindAnyObjectByType<PlayerController>();
        sfx = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        
        if (ranged)
        {
            attackRange = Random.Range(10f, 20f);
        }
        else
        {
            attackRange = 0f;
        }
    }

    void Update()
    {
        if (!isDead)
        {
            LOS();
            animator.SetFloat("spd", agent.velocity.magnitude);
            
            float distanceToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);

            if (distanceToPlayer < attackRange && los)
            {
                agent.ResetPath();
                Vector3 targetPosition = playerObj.transform.position;
                Vector3 directionToPlayer = targetPosition - transform.position;

                Vector3 lookDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
            else if (distanceToPlayer <= pathfindingRange && detectedPlayer)
            {
                Vector3 targetPosition = playerObj.transform.position;
                Vector3 directionToPlayer = targetPosition - transform.position;

                Vector3 lookDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }

                float clampedX = Mathf.Clamp(transform.position.x + directionToPlayer.x, targetPosition.x, targetPosition.x);
                float clampedZ = Mathf.Clamp(transform.position.z + directionToPlayer.z, targetPosition.z, targetPosition.z);

                Vector3 destination = new Vector3(clampedX, transform.position.y, clampedZ);
                agent.SetDestination(destination);
            }
            else
            {
                agent.ResetPath();
            }
        }
    }

    void LOS()
    {
        Vector3 targetPos = player.transform.position;
        Collider playerCollider = player.GetComponentInChildren<Collider>();

        targetPos.y = playerCollider.bounds.max.y - 0.1f;
        
        Vector3 dir = (targetPos - transform.position).normalized;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;

        LayerMask layerMask = LayerMask.GetMask("Ground", "Player");

        if (Physics.Raycast(ray, out hit, detectionRange, layerMask))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                los = false; 
                return;
            }

            if (hit.collider.CompareTag("Player"))
            {
                los = true;
                detectedPlayer = true;
            }
            else
            {
                los = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }
    }

    public void Hit()
    {
        Instantiate(blood, transform.position, Quaternion.identity);

        player.focus += 10;
        gameManager.Kill(this);

        if(ranged)
        {
            soundManager.EnemySFX(sfx, rangedSquelch);
        }
        else
        {
            soundManager.EnemySFX(sfx, meleeSquelch);
        }

        if(item != null)
        {
            item.Enable();
            EnemyGun enemyGun = GetComponent<EnemyGun>();
            enemyGun.StopAllCoroutines();
            enemyGun.enabled = false;
        }
        
        model.SetActive(false);
        

        gameManager.Score(100);
        isDead = true;

        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
