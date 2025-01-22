using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public float mag = 75f;
    public float spd = 5f;
    public float fireRate = 0.5f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    private Vector3 rot = Vector3.zero;
    private Vector3 originalRot;
    private Vector3 originalPos;

    public SoundManager soundManager;
    private CamController cam;

    public float reloadBackAmount = 0.2f;
    public float reloadSpeed = 2f;

    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition;
        cam = FindObjectOfType<CamController>();
    }

    void Update()
    {
        rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
        transform.localEulerAngles = originalRot + rot;
    }

    public void Recoil()
    {
        rot += new Vector3(-mag, 0, 0f);
    }

    public void Shoot(Transform player)
    {
        Recoil();
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * projectileSpeed;
        }
        Bullet bullet = projectile.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(player.position);
        }
    }
}
