using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;

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
        Debug.Log("bullet hit: " + collider);

        if (collider.CompareTag("Player"))
        {
            PlayerController playerController = collider.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.Hit();
                Debug.Log("player collider hit");
            }
        }
    }
}
