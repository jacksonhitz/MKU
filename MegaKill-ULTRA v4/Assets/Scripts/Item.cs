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
    Rigidbody rb;
    float scaleDuration = 0.5f;
    public bool isHovering;
    public bool thrown;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        originalScale = transform.localScale;
        
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
        Debug.Log("collision");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("player collision");
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                collider = GetComponentInParent<Collider>();
            }
            if (collider != null)
            {
                Physics.IgnoreCollision(collision.collider, collider);
                Debug.Log("worked");
            }
        }
        else if(thrown)
        {
            thrown = false;
            available = true;
            if (collision.gameObject.CompareTag("NPC"))
            {
                collision.gameObject.GetComponent<Enemy>()?.Hit();
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
        
        if(!thrown)
        {
            available = true;
        }

        rb.useGravity = true;
        rb.isKinematic = false;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = true;

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
        }
    }
    public void CollidersOff()
    {
        Debug.Log("colliders off");
        
        available = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        
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
