using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour, IHit
{
    public enum EnemyState { Wander, Active, Brawl, Static, Pathing }
    public EnemyState currentState;
    public bool friendly;

    [Header("SFX")]
    public AudioClip attackClip;
    public AudioClip deadClip;
    public AudioClip hitClip;
    public AudioClip stunClip;

    [Header("VFX")]
    public GameObject deathEffect;

    [Header("Brawl Settings")]
    float playerPriorityChance = 0.5f;

    // REFERENCES
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource sfx;

    [Header("Static Pathing Coordinates")]
    public PathingPoint[] pathingPoints;
    [SerializeField] bool isStaticPathing;
    int currentPathIndex = 0;

    [System.Serializable]
    public class PathingPoint
    {
        public Vector3 position;
    }

    // PUBLIC VAR
    protected float dmg;
    protected float attackRate;
    protected float attackRange;
    protected float detectionRange = 100f;
    protected bool isAttacking;
    protected bool detectedPlayer;

    public bool dosed;

    // VAR
    float stunDuration = 2f;
    [HideInInspector] public bool los;
    [HideInInspector] public bool isDead;
    bool isStunned;
    bool hasSpawnedDeathEffect = false;

    float maxHealth = 100f;
    float health;

    Vector3 wanderDestination;
    float wanderTimer;

    public GameObject currentBrawlTarget;
    float brawlTargetTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sfx = GetComponent<AudioSource>();

        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        enemyManager = FindObjectOfType<EnemyManager>();

        health = maxHealth;
    }

    protected virtual void Update()
    {
        if (isDead || isStunned) return;

        LOS();

        switch (currentState)
        {
            case EnemyState.Wander:
                WanderBehavior();
                break;
            case EnemyState.Active:
                ActiveBehavior();
                break;
            case EnemyState.Brawl:
                BrawlBehavior();
                break;
            case EnemyState.Pathing:
                PathingBehavior();
                break;
        }

        animator.SetBool("Run", agent.velocity.magnitude > 0.1f);
    }

    public void LOS()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Ground", "Player");

        if (Physics.Raycast(transform.position, direction, out hit, detectionRange, layerMask))
        {
            los = hit.collider.CompareTag("Player");
            detectedPlayer |= los;
        }
        else
        {
            los = false;
        }
    }

    void WanderBehavior()
    {
        agent.speed = 10f; // Slow stroll when wandering

        // Continuously choose new wander points
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 5f)
        {
            wanderDestination = GetRandomWanderPoint();
            agent.SetDestination(wanderDestination);  // Update destination immediately
        }
    }

    Vector3 GetRandomWanderPoint()
    {
        // Choose a random direction within a 50-unit radius
        Vector3 randomDirection = Random.insideUnitSphere * 25f;
        randomDirection += transform.position;
        NavMeshHit navHit;

        // Get a valid position on the NavMesh
        if (NavMesh.SamplePosition(randomDirection, out navHit, 25f, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        // Fallback: return the current position if no valid spot found
        return transform.position;
    }

    void ActiveBehavior()
    {
        if (friendly) return;

        if (los && Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            agent.ResetPath();
            LookTowards(player.transform.position);
            StartCoroutine(AttackCheck(player.gameObject));
        }
        else if (detectedPlayer && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            agent.SetDestination(player.transform.position);
            LookTowards(player.transform.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    void BrawlBehavior()
    {
        agent.speed = 30f; // Faster speed when brawling

        brawlTargetTimer -= Time.deltaTime;

        if (currentBrawlTarget == null ||
            (currentBrawlTarget.CompareTag("Enemy") && currentBrawlTarget.GetComponent<Enemy>().isDead) ||
            brawlTargetTimer <= 0f)
        {
            currentBrawlTarget = ChooseBrawlTarget();
            brawlTargetTimer = Random.Range(4f, 6f);
        }

        if (currentBrawlTarget != null)
        {
            Debug.DrawLine(transform.position, currentBrawlTarget.transform.position, Color.red);

            float distance = Vector3.Distance(transform.position, currentBrawlTarget.transform.position);

            if (distance < attackRange)
            {
                agent.ResetPath();
                LookTowards(currentBrawlTarget.transform.position);
                StartCoroutine(AttackCheck(currentBrawlTarget));
            }
            else
            {
                Vector3 spreadPosition = GetSpreadPosition(currentBrawlTarget.transform.position);
                agent.SetDestination(spreadPosition);
                LookTowards(spreadPosition);
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    Vector3 GetSpreadPosition(Vector3 targetPosition)
    {
        float spreadRadius = 1.5f;
        Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
        Vector3 offset3D = new Vector3(randomOffset.x, 0, randomOffset.y);
        Vector3 spreadDestination = targetPosition + offset3D;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(spreadDestination, out navHit, 2f, NavMesh.AllAreas))
            return navHit.position;

        return targetPosition;
    }

    GameObject ChooseBrawlTarget()
    {
        List<GameObject> validEnemyTargets = new List<GameObject>();
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        bool playerInRange = playerDistance <= detectionRange;

        // Dynamically scale chance based on proximity to player
        float proximity = 1f - Mathf.Clamp01(playerDistance / detectionRange);
        float dynamicChance = playerPriorityChance * proximity * 1.1f;

        // Random chance to target player if nearby
        if (playerInRange && Random.value < dynamicChance)
        {
            Debug.Log($"{gameObject.name} is targeting PLAYER (Chance: {dynamicChance:F2})");

            return player.gameObject;
        }

        // Look for other enemies to fight
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy == this || enemy.isDead) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > detectionRange) continue;

            // Limit dogpiling: max 3 attackers per target
            int currentAttackers = 0;
            foreach (Enemy other in enemyManager.enemies)
            {
                if (other == this || other.isDead) continue;
                if (other.currentBrawlTarget == enemy.gameObject)
                    currentAttackers++;
            }

            if (currentAttackers < 3)
            {
                validEnemyTargets.Add(enemy.gameObject);
            }
        }

        if (validEnemyTargets.Count > 0)
        {
            GameObject selected = validEnemyTargets[Random.Range(0, validEnemyTargets.Count)];
            Debug.Log($"{gameObject.name} is targeting ENEMY: {selected.name}");
            return selected;
        }

        // Fallback: target player if in range, otherwise null
        return playerInRange ? player.gameObject : null;
    }


    public IEnumerator AttackCheck(GameObject target)
    {
        if (!isAttacking)
        {
            Debug.Log($"{gameObject.name} is ATTACKING {target.name}");

            animator.SetBool("isAttacking", true);
            isAttacking = true;

            CallAttack(target);

            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy != null && !targetEnemy.isDead)
            {
                targetEnemy.currentState = EnemyState.Brawl;
            }

            yield return new WaitForSeconds(attackRate);
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
        else
        {
            Debug.Log($"{gameObject.name} tried to attack but was already attacking");

        }
        //yield break;
    }

    void PathingBehavior()
    {
        agent.speed = 10f;
        if (pathingPoints.Length == 0)
        {
            Debug.LogError("Pathing Behavior: No pathing points assigned.");
            return;
        }
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            if (isStaticPathing && currentPathIndex == pathingPoints.Length - 1)
            {
                agent.SetDestination(pathingPoints[currentPathIndex].position); // Destination is the last point
                return; // Stop at the last point if static pathing
            }
            agent.SetDestination(pathingPoints[currentPathIndex].position);

            currentPathIndex = (currentPathIndex + 1) % pathingPoints.Length;
        }
    }

    public void LookTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void Hit(float dmg)
    {
        soundManager.EnemySFX(sfx, hitClip);
        //soundManager.EnemyHit(transform.position);

        health -= dmg;
        Debug.Log("health: " + health);
        if (health <= 0)
        {
            Dead();
        }
        else
        {
            if (health == 50)
            {
                StopAllCoroutines();
                StartCoroutine(Stun());
                soundManager.EnemySFX(sfx, stunClip);
            }
        }
    }

    IEnumerator Stun()
    {
        isStunned = true;
        animator.SetBool("Stun", true);
        agent.ResetPath();
        los = false;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        animator.SetBool("Stun", false);
        agent.isStopped = false;

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void Dead()
    {
        if (isDead) return;

        isDead = true;
        StopAllCoroutines();
        enemyManager.Kill(this);
        DropItem();

        if (!hasSpawnedDeathEffect && deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            hasSpawnedDeathEffect = true;
        }

        Destroy(gameObject);
    }

    //public IEnumerator StartBrawlAggression()
    //{
    //    yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // dramatic delay
    //    currentBrawlTarget = ChooseBrawlTarget();
    //}

    protected abstract void CallAttack(GameObject target);
    protected abstract void Start();
    protected abstract void DropItem();
}
