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
    public AudioClip stunSound;

    AudioSource sfx;
    public Animator animator;
    public GameObject model;

    public bool ranged;

    public Item item;

    public bool isDead = false;
    
    public bool isStunned = false;
    public float stunDuration = 2f; 
    private float stunTimer = 0f;
    
    // Reference to EnemyGun component
    private EnemyGun enemyGun;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = FindAnyObjectByType<PlayerController>();
        sfx = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        enemyGun = GetComponent<EnemyGun>();
        
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
        // Handle stun timer
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                RecoverFromStun();
            }
            
            // When stunned, ensure shooting is completely disabled
            los = false;
            
            // Skip the rest of the update logic while stunned
            return;
        }

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

    public bool CanShoot
    {
        get { return !isStunned && !isDead && los; }
    }

    void LOS()
    {
        Vector3 targetPos = player.transform.position;
        Collider playerCollider = player.GetComponentInChildren<Collider>();

        targetPos.y = playerCollider.bounds.max.y - 0.1f;
        
        Vector3 dir = (targetPos - transform.position).normalized;
        Ray enemyRay = new Ray(transform.position, dir);
        RaycastHit hit;

        LayerMask layerMask = LayerMask.GetMask("Ground", "Player");

        if (Physics.Raycast(enemyRay, out hit, detectionRange, layerMask))
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
        if (isStunned)
        {
            KillEnemy();
        }
        else
        {
            StunEnemy();
        }
    }

    private void StunEnemy()
    {
        isStunned = true;
        stunTimer = stunDuration;
        
        if (stunSound != null)
        {
            soundManager.EnemySFX(sfx, stunSound);
        }

        if (animator != null)
        {
            animator.SetBool("Stunned", true);
        }
        
        // Stop the enemy from moving
        agent.ResetPath();
        agent.isStopped = true;
        
        // We'll just pause the shooting behavior by setting los to false
        // but keep the component enabled
        los = false;
        
        Debug.Log("Enemy stunned for " + stunDuration + " seconds");
    }
    
    private void RecoverFromStun()
    {
        isStunned = false;
        
        // Return to normal state
        if (animator != null)
        {
            animator.SetBool("Stunned", false);
        }
        
        // Allow movement again
        agent.isStopped = false;
        
        // LOS will be rechecked in the next Update() call
        // allowing the enemy to shoot again if they can see the player
        
        Debug.Log("Enemy recovered from stun");
    }
    
    private void KillEnemy()
    {
        Instantiate(blood, transform.position, Quaternion.identity);

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
            if (enemyGun != null)
            {
                enemyGun.StopAllCoroutines();
            }
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