using UnityEngine;
using System.Collections;

public class EnemyGun : MonoBehaviour
{
    public float reloadMag = 40f;
    public float reloadSpd = 2f;
    public float fireRate = 2f;
    public float bulletSpd = 50f;
    public float range = 1000f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public TrailRenderer tracerPrefab;

    private Vector3 rot = Vector3.zero;
    private SoundManager soundManager;
    private CamController cam;
    private Enemy enemy;
    private GameObject player;
    private bool isAttacking = false;
    private AudioSource sfx;
    public AudioClip gunshot;

    private Enemy enemyComponent;

    public Animator animator;
    public ParticleSystem muzzleFlash;

    [SerializeField] float targetAdjust;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        sfx = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        cam = FindObjectOfType<CamController>();
        soundManager = FindObjectOfType<SoundManager>();

        fireRate = Random.Range(1f, 3f);
        enemyComponent = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.los && InRange() && !isAttacking && CanShootNow())
        {
            StartCoroutine(CallAttack());
        }
    }

    IEnumerator CallAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(fireRate);
        
        if (CanShootNow())
        {
            Attack();
        }
        
        isAttacking = false;
    }

    public bool InRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Attack()
    {
        if (enemy.isDead || !CanShootNow() || !enemy.los)
        {
            return;
        }

        soundManager.EnemySFX(sfx, gunshot);

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
            muzzleFlash.Emit(10);
        }

        Vector3 targetPos = player.transform.position;
        targetPos.y = player.transform.position.y + targetAdjust;

        Vector3 targetDir = (targetPos - firePoint.position).normalized;

        GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

        rb.velocity = targetDir * bulletSpd;

        bullet.vel = bulletSpd;
        bullet.direction = targetDir;

        enemy.animator.SetTrigger("Shoot");

        TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
        StartCoroutine(HandleTracer(tracer, targetDir));
    }

    IEnumerator HandleTracer(TrailRenderer tracer, Vector3 direction)
    {
        float tracerLifetime = tracer.time;
        float elapsedTime = 0f;
        
        while (elapsedTime < tracerLifetime)
        {
            tracer.transform.position += direction * bulletSpd * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(tracer.gameObject);
    }

    bool CanShootNow()
    {
        if (enemyComponent != null && !enemyComponent.CanShoot)
        {
            return false;
        }
        return true;
    }
}