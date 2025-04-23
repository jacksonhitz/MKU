using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Material def;
    public Material glow;

    Renderer rend;
    Rigidbody rb;

    public bool isHovering;
    public bool thrown;

    string parent;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        SetState();
    }

    public virtual void Use() { }

    public void SetState()
    {
        GameObject parentObj = transform.parent.gameObject;
        parent = parentObj.tag;

        switch (parent)
        {
            case "Enemy":
                currentState = ItemState.Enemy;
                CollidersOff();
                break;
            case "Player":
                currentState = ItemState.Player;
                CollidersOff();
                break;
            default:
                currentState = ItemState.Available;
                CollidersOn();
                break;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("player collision");

            Collider ownCollider = GetComponent<Collider>() ?? GetComponentInParent<Collider>();
            if (ownCollider != null)
            {
                Physics.IgnoreCollision(collision.collider, ownCollider);
                Debug.Log("worked");
            }
        }
        else if (thrown)
        {
            thrown = false;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>()?.Hit(50);
            }
        }
    }

    public void Dropped()
    {
        transform.SetParent(null);
        SetState();


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

        rb.useGravity = true;
        rb.isKinematic = false;

        if (TryGetComponent(out MeshCollider meshCollider))
            meshCollider.enabled = true;

        if (TryGetComponent(out CapsuleCollider capsuleCollider))
            capsuleCollider.enabled = true;
    }
    public void CollidersOff()
    {
        Debug.Log("colliders off");

        rb.useGravity = false;
        rb.isKinematic = true;

        if (TryGetComponent(out MeshCollider meshCollider))
            meshCollider.enabled = false;

        if (TryGetComponent(out CapsuleCollider capsuleCollider))
            capsuleCollider.enabled = false;
    }


    // HIGHLIGHT ITEM
    void OnMouseEnter()
    {
        isHovering = true;
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
