using System.Collections;
using UnityEngine;


public class Gun : Item
{
    public GunData gunData;

    public bool canFire = true;

    public float bullets;

    public SoundManager soundManager;

    Vector3 rot = Vector3.zero;
    Vector3 originalRot;

    PlayerController player;
    Rigidbody rb;


    [SerializeField] TrailRenderer tracerPrefab;
    [SerializeField] float tracerDuration;
    [SerializeField] Transform firePoint;


    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        originalRot = transform.localEulerAngles;
        bullets = gunData.maxBullets;
    }


    void Update()
    {
        if (rb.isKinematic)
        {
            rot = Vector3.Lerp(rot, Vector3.zero, gunData.recoilSpd * Time.deltaTime);
            transform.localEulerAngles = originalRot + rot;
        }
    }

    public void Recoil()
    {
        rot += new Vector3(-gunData.recoilMag, 0, 0f);
    }

    public void Hitscan(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, player.range))
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
        yield return new WaitForSeconds(gunData.fireRate);
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



