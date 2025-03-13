using UnityEngine;
using System.Collections;

public class EnemyMG : Enemy
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TrailRenderer tracerPrefab;
    public ParticleSystem muzzleFlash;

    Item item;

    float bulletSpd = 50f;
    float bullets = 12f;
    float targetAdjust = 0.25f;
    float spreadAngle = 10f; 
    float reloadRate = 3f;

    bool reloading;

    protected override void Start()
    {
        item = GetComponentInChildren<Item>();
        attackRange = 15f;
        attackRate = 0.15f;
        dmg = 3f; 
    }

    protected override void CallAttack()
    {
        if (bullets > 0)
        {
            bullets--;
            StartCoroutine(Attack());
        }
        else if (!reloading)
        {
            reloading = true;
            StartCoroutine(Reload());
        }
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        soundManager.EnemySFX(sfx, attackClip);

        muzzleFlash.Play();
        Vector3 targetPos = player.transform.position;
        targetPos.y += targetAdjust;

        Vector3 spread = new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
        Vector3 targetDir = (targetPos - firePoint.position).normalized + spread * 0.01f;

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

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadRate);
        Debug.Log("reloaded");
        bullets = 12f;
        reloading = false;
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