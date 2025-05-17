using System.Collections;
using UnityEngine;

public abstract class Item : Interactable
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public PopUp popUp;
    
    public bool thrown;

    public ItemData itemData;
    public MonoBehaviour holder;

    bool isUseable;

    public enum ItemState
    {
        Available,
        Enemy,
        Player
    }
    public ItemState currentState;

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        popUp = FindObjectOfType<PopUp>();
    }


    public virtual void Start()
    {
        FindState();
    }

    //STATE MANAGING
    public void FindState()
    {
        if (transform.parent != null)
        {
            string tag = transform.parent.tag;
            if (System.Enum.TryParse(tag, out ItemState state)) SetState(state);
            else SetState(ItemState.Available);
        }
        else SetState(ItemState.Available);
    }
    public void SetState(ItemState state)
    {
        currentState = state;
        switch (state)
        {
            case ItemState.Enemy:
                CollidersOff();
                EnemyPass();
                holder = GetComponentInParent<Enemy>();
                break;
            case ItemState.Player:
                CollidersOff();
                holder = GetComponentInParent<PlayerController>();
                break;
            case ItemState.Available:
                CollidersOn();
                holder = null;
                isUseable = true;
                break;
        }
    }
    public void EnemyPass()
    {
        holder = GetComponentInParent<Enemy>();
        var enemy = holder as Enemy;
        enemy.item = this;
    }
    
    public void PlayerGrabbed(PlayerController player)
    {
        SetState(ItemState.Player);
    }
    public void EnemyGrabbed(Enemy enemy)
    {
        SetState(ItemState.Enemy);
    }

    //USE
    IEnumerator UseTimer()
    {
        isUseable = false;
        yield return new WaitForSeconds(itemData.rate);
        isUseable = true;
    }
    public void UseCheck()
    {
        if (isUseable)
        {
            Use();
            StartCoroutine(UseTimer());
        }
    }
    public virtual void Use() { }



    public void Thrown()
    {
        transform.SetParent(null);
        SetState(ItemState.Available);
        thrown = true;
    }
    public void Dropped()
    {
        transform.SetParent(null);
        SetState(ItemState.Available);

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
}
