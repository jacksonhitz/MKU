using UnityEngine;
using System.Collections;

public class EnemyGun : MonoBehaviour
{
    float reloadMag = 40f;
    float reloadSpd = 2f;
    float fireRate = 5f;
    float bulletSpd = 50f;

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
        StartCoroutine(HandleTracer(tracer, bullet));
    }

    IEnumerator HandleTracer(TrailRenderer tracer, GameObject bullet)
    {
        while (bullet != null)
        {
            tracer.transform.position = bullet.transform.position;
            yield return null;
        }

        yield return new WaitForSeconds(tracer.time);

        Destroy(tracer.gameObject);
    }
}
