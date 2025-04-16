using System.Collections;
using UnityEngine;

public class MG : MonoBehaviour
{
    PlayerController player;
    SoundManager soundManager;
    GameManager gameManager;
    Rigidbody rb;

    float bullets = 12f;
    float tracerDuration = 0.2f;
    [SerializeField] TrailRenderer tracerPrefab;

    [SerializeField] Transform firePoint;

    int bulletsPerShot = 1;
    float spreadAngle = 1.5f;

    private bool canFire = true;
    float fireRate = 0.15f;
    float mag = 20f;  
    float spd = 5f;   
    Vector3 rot = Vector3.zero;

    Vector3 originalRot;
    Vector3 originalPos;
    

    public ParticleSystem muzzleFlash;

    private float lastActionTime = -0.15f; 


    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (rb.isKinematic)
        {
            rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
            transform.localEulerAngles = originalRot + rot;
        }
    }

    public void Use()
    {
        if (canFire)
        {
            if (bullets > 0)
            {
                StartCoroutine(FireCooldown());
                Recoil();  
                soundManager.MGShot();
                muzzleFlash?.Play();
                bullets--; 

                for (int i = 0; i < bulletsPerShot; i++)
                {
                    Vector3 spread = new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
                    Quaternion rotation = Quaternion.Euler(player.cam.transform.eulerAngles + spread);
                    Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
                    Hitscan(ray);
                }
            }
            else
            {
                if (Time.time - lastActionTime >= 0.5f)
                {
                    soundManager.MGEmpty();
                    //ui.PopUp("EMPTY");
                    lastActionTime = Time.time;
                }
            } 
        }
    }

    public void Recoil()
    {
        rot += new Vector3(-mag, 0, 0f);
    }

    void Hitscan(Ray ray)
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

    IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
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
