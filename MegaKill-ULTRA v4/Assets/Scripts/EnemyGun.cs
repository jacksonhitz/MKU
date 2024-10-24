using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public float mag = 75f;
    public float spd = 5f;
    public float fireRate = .5f; 
    Vector3 rot = Vector3.zero;
    Vector3 originalRot; 
    Vector3 originalPos;

    public float reloadBackAmount = 0.2f; 
    public float reloadSpeed = 2f; 
    public LayerMask hitMask; 
    void Start()
    {
        originalRot = transform.localEulerAngles;
        originalPos = transform.localPosition; 
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
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, directionToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
        {
            Debug.Log("Hit: " + hit.transform.name);
            if (hit.transform.CompareTag("Player"))
            {
                PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                playerController.Hit();
            }
        }
    }
}
