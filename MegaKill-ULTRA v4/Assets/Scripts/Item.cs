using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool available;
    public bool enemyHas;
    public Material def;
    public Material glow;

    public Renderer rend;
    Coroutine scaleCoroutine;
    Vector3 originalScale;
    GameManager gameManager;
    PlayerController player;
    float scaleDuration = 0.5f;
    public bool isHovering;
    public bool thrown;

    void Awake()
    {
        originalScale = transform.localScale;

        gameManager = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<PlayerController>();
    }
    void Start()
    {
        if (enemyHas)
        {
            CollidersOff();
        }
    }

    public void Use()
    {
        player.Throw(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (thrown)
        {
            thrown = false;
            if (collision.gameObject.CompareTag("NPC"))
            {
                collision.gameObject.GetComponent<Enemy>()?.Hit();
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                collider = GetComponentInParent<Collider>();
            }
            if (collider != null)
            {
                Physics.IgnoreCollision(collision.collider, collider);
            }
        }
    }

    public void Dropped()
    {
        CollidersOn();

        transform.SetParent(null);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 randomDirection = new Vector3(
            Random.Range(-.1f, .1f),
            Random.Range(0.25f, .5f),
            Random.Range(-.1f, .1f)
        );

        rb.AddForce(randomDirection * 5f, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(
            Random.Range(-20f, 20f),
            Random.Range(-20f, 20f),
            Random.Range(-20f, 20f)
        );

        rb.AddTorque(randomTorque, ForceMode.Impulse);
    }

    public void CollidersOn()
    {
        Debug.Log("colliders on");
        
        available = true;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = true;

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = true;
        }
    }
    public void CollidersOff()
    {
        Debug.Log("colliders off");
        
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = false;

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }
    }

    void OnMouseEnter()
    {
        if (available && !gameManager.isIntro)
        {
            isHovering = true;
        }
    }
    void OnMouseExit()
    {
        isHovering = false;
    }

    public void DefaultMat()
    {
        if (rend != null)
        {
            rend.material = def;
        }
    }
    public void GlowMat()
    {
        if (rend != null)
        {
            rend.material = glow;
        }
    }
}
