using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    protected float mag = 75f;
    protected float spd = 5f;
    protected Vector3 rot = Vector3.zero;

    protected Vector3 originalRot;
    protected Vector3 originalPos;
    protected Vector3 stowedPos = new Vector3(-0.5f, -0.5f, 0f);
    protected Vector3 stowedRot = new Vector3(45f, 0f, 0f);

    protected PlayerController player;
    protected SoundManager soundManager;
    protected GameManager gameManager;

    protected bool isStowing = false;
    protected float switchSpeed = 5f;
    protected bool canFire = true;
    protected float fireRate = 0.5f;

    public LayerMask npcMask;
    public TrailRenderer tracerPrefab;
    public float tracerDuration = 0.2f;

    protected virtual void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
    }

    public virtual void SetPos()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition;
        Debug.Log(originalPos);
    }

    protected virtual void Update()
    {
        ResetRot();
    }

    public void StowWeapon(bool stow)
    {
        isStowing = stow;
        StopAllCoroutines();
        StartCoroutine(SmoothTransition(stow ? stowedPos : originalPos, stow ? stowedRot : originalRot));
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

    protected IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    protected void Hitscan(Ray ray, Transform firePoint)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, player.range))
        {
            if (hit.transform.CompareTag("NPC"))
            {
                hit.transform.GetComponent<Enemy>()?.Hit();
            }
            TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
            StartCoroutine(HandleTracer(tracer, hit.point, firePoint));
        }
    }

    private IEnumerator HandleTracer(TrailRenderer tracer, Vector3 hitPoint, Transform firePoint)
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

    public abstract void Use();
    public abstract void ResetRot();
    public abstract void Recoil();
}