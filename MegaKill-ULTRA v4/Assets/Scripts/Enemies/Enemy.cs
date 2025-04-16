using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyState { Wander, Active, Brawl, Static }
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
    [Range(0f, 1f)] public float playerPriorityChance = 0.5f;

    // REFERENCES
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource sfx;

    // PUBLIC VAR
    protected float dmg;
    protected float attackRate;
    protected float attackRange;
    protected float detectionRange = 15f;
    protected bool isAttacking;
    protected bool detectedPlayer;

    // VAR
    float stunDuration = 2f;
    [HideInInspector] public bool los;
    [HideInInspector] public bool isDead;
    bool isStunned;
    bool hasSpawnedDeathEffect = false;

    Vector3 wanderDestination;
    float wanderTimer;

    GameObject currentBrawlTarget;
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
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            wanderDestination = GetRandomWanderPoint();
            agent.SetDestination(wanderDestination);
            wanderTimer = Random.Range(5f, 50f);
        }
    }

    Vector3 GetRandomWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 5f;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, 5f, NavMesh.AllAreas);
        return navHit.position;
    }

    void ActiveBehavior()
    {
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
        brawlTargetTimer -= Time.deltaTime;

        if (currentBrawlTarget == null ||
            (currentBrawlTarget.CompareTag("Enemy") && currentBrawlTarget.GetComponent<Enemy>().isDead) ||
            brawlTargetTimer <= 0f)
        {
            currentBrawlTarget = ChooseBrawlTarget();
            brawlTargetTimer = Random.Range(1f, 10f);
        }

        if (currentBrawlTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentBrawlTarget.transform.position);

            if (distance < attackRange)
            {
                agent.ResetPath();
                LookTowards(currentBrawlTarget.transform.position);
                StartCoroutine(AttackCheck(currentBrawlTarget));
            }
            else
            {
                agent.SetDestination(currentBrawlTarget.transform.position);
                LookTowards(currentBrawlTarget.transform.position);
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    GameObject ChooseBrawlTarget()
    {
        List<GameObject> possibleTargets = new List<GameObject>();

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        bool playerInRange = playerDistance <= detectionRange;

        if (playerInRange && Random.value < playerPriorityChance)
            return player.gameObject;

        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy == this || enemy.isDead) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= detectionRange)
                possibleTargets.Add(enemy.gameObject);
        }

        if (possibleTargets.Count == 0)
            return playerInRange ? player.gameObject : null;

        return possibleTargets[Random.Range(0, possibleTargets.Count)];
    }

    public IEnumerator AttackCheck(GameObject target)
    {
        if (!isAttacking)
        {
            animator.SetBool("isAttacking", true);
            isAttacking = true;
            CallAttack(target);
            yield return new WaitForSeconds(attackRate);
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
        else yield break;
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

    }

    public void HitCheck(bool lethal)
    {
        soundManager.EnemySFX(sfx, hitClip);
        Hit(lethal);
    }

    protected virtual void Hit(bool lethal)
    {
        if (lethal || isStunned)
        {
            Dead();
        }
        else
        {
            soundManager.EnemySFX(sfx, stunClip);
            StopAllCoroutines();
            StartCoroutine(Stun());
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
        gameManager.Kill(this);
        DropItem();

        if (!hasSpawnedDeathEffect && deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            hasSpawnedDeathEffect = true;
        }

        Destroy(gameObject);
    }

    protected abstract void CallAttack(GameObject target);
    protected abstract void Start();
    protected abstract void DropItem();
}
