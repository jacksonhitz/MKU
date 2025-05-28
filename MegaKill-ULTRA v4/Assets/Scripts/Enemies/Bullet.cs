using UnityEngine;

public class Bullet : MonoBehaviour
{
    float lifeTime = 5f;
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float vel;
    [HideInInspector] public float dmg;
    BulletTime bulletTime;
    Rigidbody rb;
    int nullLayer;

    void Awake()
    {
        bulletTime = FindAnyObjectByType<BulletTime>();
        rb = GetComponent<Rigidbody>();
        nullLayer = LayerMask.NameToLayer("Null");
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
        Debug.Log("bulletSpawned");

        Physics.IgnoreLayerCollision(gameObject.layer, nullLayer, true);
    }

    void FixedUpdate()
    {
        if (bulletTime != null && bulletTime.isSlow)
        {
            rb.velocity = dir * (vel * 0.25f);
        }
        else
        {
            rb.velocity = dir * vel;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("bullet hit: " + collision.gameObject.name);
        
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.Hit(dmg);
            }
        }
        Destroy(gameObject);
    }
}
