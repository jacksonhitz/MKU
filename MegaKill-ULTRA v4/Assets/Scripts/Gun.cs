using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    float mag = 25f;
    float spd = 5f;
    Vector3 rot = Vector3.zero;

    Vector3 originalRot;
    Vector3 originalPos;
    Vector3 stowedPos = new Vector3(-0.5f, -0.5f, 0f);
    Vector3 stowedRot = new Vector3(45f, 0f, 0f);

    public PlayerController player;
    public SoundManager soundManager;

    public float bullets = 6f;
    public float shells = 2f;
    public float reloadBackAmount = 0.2f;
    public float reloadSpeed = 2f;
    public float tracerDuration = 0.2f;
    public float scaredRad = 50f;

    public LayerMask npcMask;
    public TrailRenderer tracerPrefab;
    public Transform firePoint;
    public int pellets = 12;
    public float spreadAngle = 5f;

    private bool isStowing = false;
    private float switchSpeed = 5f;
    private bool canFire = true;
    float fireRate = .5f;

    public Transform revolver;
    public Transform shotgun;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
        if (player.weapon == 0)
        {
            revolver.localEulerAngles = originalRot + rot;
        }
        if (player.weapon == 1)
        {
            shotgun.localEulerAngles = originalRot + new Vector3(-90f, 0f, 180f) + rot;
        }
    }

    public void StowWeapon(bool stow)
    {
        isStowing = stow;
        StopAllCoroutines();
        if (stow)
            StartCoroutine(SmoothTransition(stowedPos, stowedRot));
        else
            StartCoroutine(SmoothTransition(originalPos, originalRot));
    }

    private IEnumerator SmoothTransition(Vector3 targetPos, Vector3 targetRot)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 startRot = transform.localEulerAngles;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime);
            transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, elapsedTime);
            elapsedTime += Time.deltaTime * switchSpeed;
            yield return null;
        }

        transform.localPosition = targetPos;
        transform.localEulerAngles = targetRot;
    }

    public void Recoil()
    {
        rot += new Vector3(-mag, 0, 0f);
    }

    public void RevolverFire()
    {
        if (!canFire || bullets <= 0) return;
        StartCoroutine(FireCooldown());

        Recoil();
        soundManager.Gunshot();
        bullets--;

        Ray ray = player.cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Hitscan(ray);
    }

    public void ShotgunFire()
    {
        if (!canFire || shells <= 0) return;
        StartCoroutine(FireCooldown());

        Recoil();
        soundManager.Gunshot();
        shells--;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 spread = new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
            Quaternion rotation = Quaternion.Euler(player.cam.transform.eulerAngles + spread);
            Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);

            Hitscan(ray);
        }
    }

    void Hitscan(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, player.range))
        {
            if (hit.transform.CompareTag("NPC"))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                enemy?.Hit();
            }

            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, hit.point));
        }
        else
        {
            Vector3 fallbackDestination = ray.origin + ray.direction * 100f;
            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, fallbackDestination));
        }
    }

    public void Reload()
    {
        soundManager.Reload();
        rot += new Vector3(mag, 0, 0f);

        if (player.weapon == 1)
        {
            shells = 2f;
            player.ux.UpdateAmmo();
        }
        else if (player.weapon == 0)
        {
            bullets = 6f;
            player.ux.UpdateAmmo();
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