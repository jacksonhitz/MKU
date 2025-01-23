using UnityEngine;
using System.Collections;

public class EnemyGun : MonoBehaviour
{
    public float reloadMag;
    public float reloadSpd;
    public float fireRate;
    public float bulletSpd;

    
    public GameObject projectilePrefab;
    public Transform firePoint;


    public TrailRenderer tracerPrefab; 
    Vector3 rot = Vector3.zero;
    Vector3 originalRot;
    Vector3 originalPos;


    public SoundManager soundManager;
    private CamController cam;



    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition;
        cam = FindObjectOfType<CamController>();
    }

    void Update()
    {
        rot = Vector3.Lerp(rot, Vector3.zero, reloadSpd * Time.deltaTime);
        transform.localEulerAngles = originalRot + rot;
    }

    public void Recoil()
    {
        rot += new Vector3(-reloadMag, 0, 0f);
    }

    public void Shoot(Transform player)
    {
        Recoil();

        Vector3 targetDir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = targetDir * bulletSpd;

        TrailRenderer tracer = Instantiate(tracerPrefab, firePoint.position, Quaternion.identity);
        StartCoroutine(HandleTracer(tracer, firePoint.position, player.position));
    }

    IEnumerator HandleTracer(TrailRenderer tracer, Vector3 start, Vector3 target)
    {
        tracer.transform.position = start;

        float travelTime = Vector3.Distance(start, target) / bulletSpd;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / travelTime;

            tracer.transform.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        tracer.transform.position = target;

        yield return new WaitForSeconds(tracer.time);

        Destroy(tracer.gameObject);
    }

}
