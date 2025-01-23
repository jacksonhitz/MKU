using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float lifeTime;
    public LayerMask hitMask;

    Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit called");
        
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            Debug.Log(other);
            
            if (other.CompareTag("Player"))
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null && !playerController.isDead)
                {
                    playerController.Hit();
                    Debug.Log("player hit");
                }
            }
            Destroy(gameObject);
        }
    }
}

