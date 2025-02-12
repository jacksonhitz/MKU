using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;
    public LayerMask hitMask;

    public Vector3 direction;
    public float vel;
    BulletTime bulletTime;
    Rigidbody rb;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        bulletTime = FindAnyObjectByType<BulletTime>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (bulletTime.isSlowed)
        {
            rb.velocity = direction * (vel * 0.25f);
        }
        else
        {
            rb.velocity = direction * vel;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("hit called");

        if (collider.CompareTag("Player"))
        {
            Debug.Log("player hit");
            PlayerController playerController = collider.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.Hit();
            }
        }
    }
}
