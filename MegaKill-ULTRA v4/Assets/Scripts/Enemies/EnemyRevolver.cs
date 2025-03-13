using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyRevolver : Enemy
{
    [Header("Refrences")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TrailRenderer tracerPrefab;
    public ParticleSystem muzzleFlash;

    Item item;

    Vector3 rot = Vector3.zero;

    float bulletSpd = 50f;
    float targetAdjust = 0.25f;

    protected override void Start()
    {
        item = GetComponentInChildren<Item>();
        attackRange = 15f;
        attackRate = 2f;
        dmg = 10f;
    }

    protected override void CallAttack()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        
        muzzleFlash.Play();
        muzzleFlash.Emit(10);

        Vector3 targetPos = player.transform.position;
        targetPos.y = player.transform.position.y + targetAdjust;

        Vector3 targetDir = (targetPos - firePoint.position).normalized;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

        rb.velocity = targetDir * bulletSpd;

        bullet.vel = bulletSpd;
        bullet.dir = targetDir;
        bullet.dmg = dmg;

        TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
        StartCoroutine(HandleTracer(tracer, targetDir));

        yield break;
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

    protected override void DropItem()
    {
        item.Dropped();
    }
}