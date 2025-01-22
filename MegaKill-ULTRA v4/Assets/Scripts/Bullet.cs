using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;
    public float speed = 20f;
    public LayerMask hitMask;

    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector3 targetPosition)
    {
        direction = (targetPosition - transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null && !playerController.isDead)
                {
                    playerController.Hit();
                }
            }
            Destroy(gameObject);
        }
    }
}

