using UnityEngine;
using System.Collections;

public class EnemyGun : MonoBehaviour
{
    float reloadMag = 40f;
    float reloadSpd = 2f;
    float fireRate = 2f;
    float bulletSpd = 50f;
    float range = 40f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public TrailRenderer tracerPrefab;

    private Vector3 rot = Vector3.zero;
    private Vector3 originalRot;
    private Vector3 originalPos;

    public SoundManager soundManager;
    private CamController cam;
    private Enemy enemy;
    private GameObject player;
    private bool isAttacking = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player");
        cam = FindObjectOfType<CamController>();

        fireRate = Random.Range(1f, 3f);
    }

    void Update()
    {
        if (enemy.los && InRange() && !isAttacking)
        {
            StartCoroutine(CallAttack());
        }
    }

    IEnumerator CallAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(fireRate);
        Attack();
        isAttacking = false;
    }

    bool InRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Recoil()
    {
        rot += new Vector3(-reloadMag, 0, 0f);
    }

    void Attack()
    {
        //Recoil();

        Vector3 targetDir = (player.transform.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = targetDir * bulletSpd;

        TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
        StartCoroutine(HandleTracer(tracer, bullet));
    }

    IEnumerator HandleTracer(TrailRenderer tracer, GameObject bullet)
    {
        while (bullet != null)
        {
            tracer.transform.position = bullet.transform.position;
            yield return null;
        }

        yield return new WaitForSeconds(tracer.time);

        Destroy(tracer.gameObject);
    }
}