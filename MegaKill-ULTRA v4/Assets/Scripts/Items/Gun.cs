using System.Collections;
using UnityEngine;


public class Gun : Item
{
    public GunData data;
    public Transform firePoint;

    public bool canFire = true;

    public float bullets;

    Vector3 rot = Vector3.zero;
    Vector3 originalRot;

    [SerializeField] TrailRenderer tracerPrefab;
    [SerializeField] float tracerDuration;

    void Start()
    {
        originalRot = transform.localEulerAngles;
        bullets = data.maxBullets;
    }


    void Update()
    {
        if (rb.isKinematic)
        {
            rot = Vector3.Lerp(rot, Vector3.zero, data.recoilSpd * Time.deltaTime);
            transform.localEulerAngles = originalRot + rot;
        }
    }

    public void Recoil()
    {
        rot += new Vector3(-data.recoilMag, 0, 0f);
    }

    public void Hitscan(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, playerController.range))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    enemy.Hit(100);
                }
            }
            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, hit.point));
        }
    }

    public IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(data.fireRate);
        canFire = true;
    }


    IEnumerator HandleTracer(TrailRenderer tracer, Vector3 hitPoint)
    {
        tracer.transform.position = firePoint.position;
        float elapsedTime = 0f;
        while (elapsedTime < tracerDuration)
        {
            tracer.transform.position = Vector3.Lerp(firePoint.position, hitPoint, elapsedTime / tracerDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(tracer.time);
        Destroy(tracer.gameObject);
    }
}



