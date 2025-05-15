using System.Collections;
using UnityEngine;

public class Gun : Item
{
    public GunData data;
    public Transform firePoint;

    public float bullets;

    Vector3 rot = Vector3.zero;

    public override void Start()
    {
        base.Start();
        bullets = data.maxBullets;
        itemData = data;
    }

    void Update()
    {
        if (rb.isKinematic)
        {
            rot = Vector3.Lerp(rot, Vector3.zero, data.recoilSpd * Time.deltaTime);
            rot = data.rot;
        }
    }
    public void Recoil()
    {
        rot += new Vector3(-data.recoilMag, 0, 0f);
    }

    // === PLAYER ===
    public void FireRay(Vector3 dir)
    {
        Ray ray = new Ray(firePoint.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, data.range))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                enemy?.Hit(data.dmg);
            }

            StartCoroutine(HandleTracer(hit.point, true));
        }
    }

    // === ENEMY ===
    public void FireBullet(Vector3 dir)
    {
        GameObject bulletObj = Instantiate(data.bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.dir = dir;
        bullet.vel = data.vel;
        bullet.dmg = data.dmg;
        
        StartCoroutine(HandleTracer(dir, false));
    }

    

    IEnumerator HandleTracer(Vector3 dir, bool ray)
    {
        TrailRenderer tracer = Instantiate(data.tracerPrefab, firePoint.position, Quaternion.identity);

        float elapsedTime = 0f;
        if (ray)
        {
            while (elapsedTime < data.tracerDuration)
            {
                tracer.transform.position = Vector3.Lerp(firePoint.position, dir, elapsedTime / data.tracerDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (elapsedTime < tracer.time)
            {
                tracer.transform.position += dir.normalized * data.vel * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        Destroy(tracer.gameObject);
    }
}
