using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : Interactable, IHitable
{
    private static readonly int GunKey = Animator.StringToHash("Gun");
    private static readonly int SpdKey = Animator.StringToHash("Spd");
    private static readonly int DanceKey = Animator.StringToHash("Dance");
    private static readonly int IsAttackingKey = Animator.StringToHash("isAttacking");
    private static readonly int StunKey = Animator.StringToHash("Stun");

    public enum EnemyState
    {
        Active,
        Wander,
        Brawl,
        Static,
        Pathing,
    }

    public EnemyState currentState;

    public GameObject deathEffect;
    public Item item;

    float playerPriorityChance = 0.5f;

    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public AudioSource sfx;

    public PathingPoint[] pathingPoints;

    [SerializeField]
    bool isStaticPathing;
    int currentPathIndex;

    [System.Serializable]
    public class PathingPoint
    {
        public Vector3 position;
    }

    protected float damage;

    [SerializeField]
    protected float attackRate;

    [SerializeField]
    protected float attackRange;
    protected float detectionRange = 100f;
    protected bool isAttacking;
    protected bool detectedPlayer;

    public bool dosed;
    public bool isStatic;
    public bool isDance;

    float stunDuration = 2f;

    [HideInInspector]
    public bool los;

    [HideInInspector]
    public bool isDead;
    bool isStunned;
    bool hasSpawnedDeathEffect;

    float maxHealth = 10f;
    float health;

    Vector3 wanderDestination;
    public GameObject target;
    float brawlTargetTimer;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        sfx = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        health = maxHealth;
        DefaultValues();
    }

    protected virtual void Update()
    {
        if (StateManager.IsPassive || isStunned)
            return;

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
            case EnemyState.Static:
                StaticBehaviour();
                break;
            case EnemyState.Pathing:
                PathingBehavior();
                break;
        }

        animator.SetBool(GunKey, item);

        if (agent != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat(SpdKey, speed);
        }
    }

    void StaticBehaviour()
    {
        if (isDance)
            animator.SetTrigger(DanceKey);
    }

    public void LOS()
    {
        if (player == null)
            return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Ground", "Player");
        if (Physics.Raycast(transform.position, direction, out hit, detectionRange, layerMask))
        {
            los = hit.collider.CompareTag("Player");
            detectedPlayer |= los;
        }
        else
            los = false;
    }

    public IEnumerator AttackCheck()
    {
        if (!isAttacking)
        {
            animator.SetBool(IsAttackingKey, true);
            isAttacking = true;
            ItemCheck();
            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy && !targetEnemy.isDead)
                targetEnemy.currentState = EnemyState.Brawl;
            yield return new WaitForSeconds(attackRate);
            isAttacking = false;
            animator.SetBool(IsAttackingKey, false);
        }
    }

    public virtual void SetItem(Item newItem)
    {
        item = newItem;
        attackRate = item.itemData.rate;
        if (item.itemData is GunData gunData)
            attackRange = gunData.range;
    }

    void ItemCheck()
    {
        if (item != null)
            item.UseCheck();
        else
            CallAttack();
    }

    public virtual void CallUse() { }

    protected virtual void CallAttack() { }

    protected virtual void DefaultValues() { }

    protected virtual void DropItem()
    {
        if (item != null)
            item.Dropped();
        DefaultValues();
    }

    void WanderBehavior()
    {
        agent.speed = 10f;
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 5f)
        {
            wanderDestination = GetWander();
            agent.SetDestination(wanderDestination);
        }
    }

    Vector3 GetWander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 25f + transform.position;
        NavMeshHit navHit;
        return NavMesh.SamplePosition(randomDirection, out navHit, 25f, NavMesh.AllAreas)
            ? navHit.position
            : transform.position;
    }

    void ActiveBehavior()
    {
        if (player == null)
            return;

        isInteractable = false;
        target = player.gameObject;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (los && distance <= attackRange)
        {
            if (agent != null)
                agent.ResetPath();

            LookTowards(player.transform.position);
            StartCoroutine(AttackCheck());
        }
        else if (detectedPlayer && distance <= detectionRange)
        {
            if (agent != null)
                agent.SetDestination(player.transform.position);

            LookTowards(player.transform.position);
        }
        else
        {
            if (agent != null)
                agent.ResetPath();
        }
    }

    void BrawlBehavior()
    {
        agent.speed = 30f;
        brawlTargetTimer -= Time.deltaTime;

        if (
            target == null
            || (target.CompareTag("Enemy") && target.GetComponent<Enemy>().isDead)
            || brawlTargetTimer <= 0f
        )
        {
            target = ChooseBrawlTarget();
            brawlTargetTimer = Random.Range(4f, 6f);
        }

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < attackRange)
            {
                agent.ResetPath();
                LookTowards(target.transform.position);
                StartCoroutine(AttackCheck());
            }
            else
            {
                Vector3 spreadPosition = GetSpread(target.transform.position);
                agent.SetDestination(spreadPosition);
                LookTowards(spreadPosition);
            }
        }
        else
            agent.ResetPath();
    }

    Vector3 GetSpread(Vector3 targetPosition)
    {
        Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
        Vector3 spreadDestination = targetPosition + new Vector3(randomOffset.x, 0, randomOffset.y);
        NavMeshHit navHit;
        return NavMesh.SamplePosition(spreadDestination, out navHit, 2f, NavMesh.AllAreas)
            ? navHit.position
            : targetPosition;
    }

    GameObject ChooseBrawlTarget()
    {
        List<GameObject> validTargets = new List<GameObject>();
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        bool playerInRange = playerDistance <= detectionRange;
        float dynamicChance =
            playerPriorityChance * (1f - Mathf.Clamp01(playerDistance / detectionRange)) * 1.1f;

        if (playerInRange && Random.value < dynamicChance)
            return player.gameObject;

        foreach (Enemy enemy in enemies.enemies)
        {
            if (enemy == this || enemy.isDead)
                continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > detectionRange)
                continue;

            int attackers = 0;
            foreach (Enemy other in enemies.enemies)
                if (other != this && !other.isDead && other.target == enemy.gameObject)
                    attackers++;

            if (attackers < 3)
                validTargets.Add(enemy.gameObject);
        }

        return validTargets.Count > 0
            ? validTargets[Random.Range(0, validTargets.Count)]
            : (playerInRange ? player.gameObject : null);
    }

    void PathingBehavior()
    {
        agent.speed = 10f;
        if (pathingPoints.Length == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            if (isStaticPathing && currentPathIndex == pathingPoints.Length - 1)
            {
                agent.SetDestination(pathingPoints[currentPathIndex].position);
                return;
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
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
    }

    public void Hit(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            Dead();
        else if (Mathf.Approximately(health, 5))
        {
            StopAllCoroutines();
            StartCoroutine(Stun());

            if (!isStatic)
                currentState = EnemyState.Active;
            sound.Play("EnemyStun", transform.position);
        }
        sound.Play("EnemyHit", transform.position);
    }

    IEnumerator Stun()
    {
        isStunned = true;
        animator.SetBool(StunKey, true);
        if (agent != null)
            agent.ResetPath();
        los = false;

        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        animator.SetBool(StunKey, false);

        if (agent != null)
            agent.isStopped = false;
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void Dead()
    {
        if (isDead)
            return;
        isDead = true;
        enemies.Kill(this);
        StopAllCoroutines();
        DropItem();
        if (!hasSpawnedDeathEffect && deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            hasSpawnedDeathEffect = true;
        }
    }
}
