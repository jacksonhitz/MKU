using System.Collections;
using UnityEngine;

public class Gun : Item
{
    public GunData data;

    public GameObject muzzleFlash;
    public Transform firePoint;

    Vector3 firePos;
    Vector3 recoilRot;
    Quaternion ogRot;

    public float bullets;
    protected override void Start()
    {
        base.Start();
        bullets = data.maxBullets;
        itemData = data;

        ogRot = Quaternion.Euler(data.rot);
        firePos = firePoint.transform.position;
    }

    float recoilTimer;
    Vector3 recoilStartRot;


    //this is stupid and bad
    void Update()
    {
        if (currentState == ItemState.Player)
        {
            if (recoilTimer > 0)
            {
                float t = 1f - (recoilTimer / data.recoilSpd);
                recoilRot = Vector3.Lerp(recoilStartRot, Vector3.zero, t);
                recoilTimer -= Time.deltaTime;
            }
            else
            {
                recoilRot = Vector3.zero;
                firePos = firePoint.transform.position;
            }
            transform.localRotation = Quaternion.Euler(recoilRot) * ogRot;
        }
        else 
        {
            recoilRot = Vector3.zero;
            firePos = firePoint.transform.position;
        }
    }

    public void FireRecoil()
    {
        recoilStartRot = recoilRot + new Vector3(-data.recoilMag, Random.Range(-data.recoilMag * 0.5f, data.recoilMag * 0.5f), 0f);
        recoilRot = recoilStartRot;
        recoilTimer = data.recoilSpd;
    }

    public void FireVFX()
    {
        GameObject obj = Instantiate(muzzleFlash, firePos, firePoint.rotation);
        Destroy(obj, .2f);
    }

    public void FireRay(Vector3 dir)
    {
        Ray ray = new Ray(firePos, dir);
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
        GameObject bulletObj = Instantiate(data.bulletPrefab, firePos, Quaternion.LookRotation(dir));
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.dir = dir;
        bullet.vel = data.vel;
        bullet.dmg = data.dmg;

        StartCoroutine(HandleTracer(dir, false));
    }

    IEnumerator HandleTracer(Vector3 dir, bool ray)
    {
        TrailRenderer tracer = Instantiate(data.tracerPrefab, firePos, Quaternion.identity);

        float elapsedTime = 0f;
        if (ray)
        {
            while (elapsedTime < data.tracerDuration)
            {
                tracer.transform.position = Vector3.Lerp(firePos, dir, elapsedTime / data.tracerDuration);
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
