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


    [SerializeField] float targetAdjust;

    protected override void Start()
    {
        item = GetComponentInChildren<Item>();
        attackRange = 15f;
    }

    protected override IEnumerator Attack()
    {
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
        bullet.direction = targetDir;

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