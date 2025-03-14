using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip attackClip;
    public AudioClip deadClip;
    public AudioClip hitClip;
    public AudioClip stunClip;


    [Header("VFX")]
    public GameObject deathEffect;


    //REFRENCES
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public SoundManager soundManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource sfx;


    //PUBLIC VAR
    protected float dmg;
    protected float attackRate;
    protected float attackRange;
    protected float detectionRange = 15f;
    protected bool isAttacking;

    protected bool detectedPlayer;

    
    //VAR
    float stunDuration = 2f;

    [HideInInspector] public bool los;
    [HideInInspector] public bool isDead;
    bool isStunned;


    
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
        LOS();
        Pathfind();
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

    protected virtual void Pathfind()
    {
        if (los && Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            agent.ResetPath();
            LookTowards(player.transform.position);
            StartCoroutine(AttackCheck());
        }
        else if (detectedPlayer && !isStunned && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            agent.SetDestination(player.transform.position);
            LookTowards(player.transform.position);
        }
        else
        {
            agent.ResetPath();
        }

        if (agent.velocity.magnitude > 0.1)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }
    public IEnumerator AttackCheck()
    {
        if (!isAttacking)
        {
            animator.SetBool("isAttacking", true);
            isAttacking = true;
            CallAttack();
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
        StopAllCoroutines();
        gameManager.Kill(this);
        DropItem();
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
    protected abstract void CallAttack();
    protected abstract void Start();
    protected abstract void DropItem();
}
