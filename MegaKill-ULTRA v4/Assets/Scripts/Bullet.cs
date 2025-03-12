using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public Vector3 direction;
    public float vel;
    private BulletTime bulletTime;
    private Rigidbody rb;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        bulletTime = FindAnyObjectByType<BulletTime>();
        rb = GetComponent<Rigidbody>();
        Debug.Log("bulletSpawned");
    }

    void FixedUpdate()
    {
        if (bulletTime != null && bulletTime.isSlow)
        {
            rb.velocity = direction * (vel * 0.25f);
        }
        else
        {
            rb.velocity = direction * vel;
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
                playerController.Hit();
                Debug.Log("player collider hit");
            }
        }
        
        Destroy(gameObject); 
    }
}