using System.Collections;
using UnityEngine;

public class EnemyShotgun : Enemy
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TrailRenderer tracerPrefab;
    public ParticleSystem muzzleFlash;

    Item item;
    float bulletSpd = 50f;
    int pellets = 12;
    float spreadAngle = 5f;
    float targetAdjust = 0.25f;

    protected override void Start()
    {
        item = GetComponentInChildren<Item>();
        attackRange = 20f;
        attackRate = 3f;
        dmg = 5f;
    }

    protected override void CallAttack(GameObject target)
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        soundManager.EnemySFX(sfx, attackClip);

        muzzleFlash.Play();
        muzzleFlash.Emit(10);

        Vector3 targetPos = player.transform.position;
        targetPos.y += targetAdjust;

        Vector3 targetDir = (targetPos - firePoint.position).normalized;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 spread = new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
            Quaternion spreadRotation = Quaternion.Euler(spread);
            Vector3 finalDir = spreadRotation * targetDir;

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(finalDir));
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

            rb.velocity = finalDir * bulletSpd;

            bullet.vel = bulletSpd;
            bullet.dir = finalDir;
            bullet.dmg = dmg;

            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, finalDir));
        }

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
