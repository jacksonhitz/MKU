using UnityEngine;

public class Bullet : MonoBehaviour
{
    float lifeTime = 5f;
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float vel;
    [HideInInspector] public float dmg;

    Rigidbody rb;
    int nullLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        nullLayer = LayerMask.NameToLayer("Null");
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);

        Physics.IgnoreLayerCollision(gameObject.layer, nullLayer, true);
    }

    void FixedUpdate()
    {
        rb.velocity = dir * vel;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("bullet hit: " + collision.gameObject.name);
        
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
            player?.health.Hit(dmg);
        }
        Destroy(gameObject);
    }
}
