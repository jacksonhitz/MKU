using System.Collections;
using UnityEngine;

public class Gun : Item
{
    public GunData data;
    public Transform firePoint;
    

    public float bullets;

    public ParticleSystem muzzleFlash;

    Vector3 recoilRot;
    Quaternion ogRot;

    protected override void Start()
    {
        base.Start();
        bullets = data.maxBullets;
        itemData = data;

        ogRot = Quaternion.Euler(data.rot);
    }

    void Update()
    {
        if (currentState == ItemState.Player)
        {
            recoilRot = Vector3.Lerp(recoilRot, Vector3.zero, data.recoilSpd * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(recoilRot) * ogRot;
        }
    }

    IEnumerator Recoil()
    {
        firePoint.parent = hand;
        recoilRot += new Vector3(-data.recoilMag, Random.Range(-data.recoilMag * 0.5f, data.recoilMag * 0.5f), 0f);
        while (recoilRot.magnitude > 0.01f)
            yield return null;
        firePoint.parent = transform;
    }

    public void FireVFX()
    {
        StartCoroutine(Recoil());
        muzzleFlash.Play();
    }

    public void FireRay(Vector3 dir)
    {
        Ray ray = new Ray(firePoint.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                enemy?.Hit(10f);
                ScoreManager.Instance?.AddGunScore();
            }
            StartCoroutine(HandleTracer(hit.point, true));
        }
    }

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
