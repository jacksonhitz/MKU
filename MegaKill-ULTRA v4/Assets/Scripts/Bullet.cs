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

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("hit called");
            
        if (collider.CompareTag("Player"))
        {
            Debug.Log("player hit");
            PlayerController playerController = collider.GetComponent<PlayerController>();
            playerController.Hit();
        }
        //Destroy(gameObject);
    }
}

