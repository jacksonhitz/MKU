using System.Collections;
using UnityEngine;

public class Item : Interactable
{
    public override Type InteractableType => Type.Item;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public PopUp popUp;

    public bool thrown;

    public ItemData itemData;
    public MonoBehaviour holder;
    protected Transform hand;

    bool isUseable;

    public enum ItemState
    {
        Available,
        Enemy,
        Player,
    }

    public ItemState currentState;

    protected override void Awake()
    {
        base.Awake();
        player = PlayerController.Instance;
        sound = SoundManager.Instance;
        rb = GetComponent<Rigidbody>();
        popUp = FindObjectOfType<PopUp>();
    }

    protected override void Start()
    {
        base.Start();
        GetState();
        isUseable = true;
    }

    //STATE MANAGING (Set State should probably be rewritten in a dict or something to match states with scripts)
    public void GetState()
    {
        if (transform.parent != null)
        {
            holder = GetComponentInParent<Enemy>();
            var enemy = holder as Enemy;
            if (enemy != null)
            {
                enemy.SetItem(this);
                SetState(ItemState.Enemy);
                return;
            }

            holder = GetComponentInParent<PlayerController>();
            player = holder as PlayerController;
            if (player != null)
            {
                SetState(ItemState.Player);
                return;
            }
        }
        SetState(ItemState.Available);
    }

    public void SetState(ItemState state)
    {
        currentState = state;
        switch (state)
        {
            case ItemState.Enemy:
                CollidersOff();
                break;
            case ItemState.Player:
                CollidersOff();
                break;
            case ItemState.Available:
                CollidersOn();
                holder = null;
                break;
        }
    }

    public void Grabbed(Transform newHand)
    {
        GetState();
        hand = newHand;
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
        Debug.Log("check1");
        if (isUseable)
        {
            StartCoroutine(UseTimer());
            Use();
            Debug.Log("check2");
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

        isInteractable = true;

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
                collision.gameObject.GetComponent<Enemy>()?.Hit(5);
                //add score here/style points
                ScoreManager.Instance?.AddThrowScore();
            }
        }
    }

    public void CollidersOn()
    {
        rb.useGravity = true;
        rb.isKinematic = false;

        if (TryGetComponent(out MeshCollider meshCollider))
            meshCollider.enabled = true;

        if (TryGetComponent(out CapsuleCollider capsuleCollider))
            capsuleCollider.enabled = true;
    }

    public void CollidersOff()
    {
        rb.useGravity = false;
        rb.isKinematic = true;

        if (TryGetComponent(out MeshCollider meshCollider))
            meshCollider.enabled = false;

        if (TryGetComponent(out CapsuleCollider capsuleCollider))
            capsuleCollider.enabled = false;
    }
    protected override void OnInteract()
    {

    }
}
