using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float runSpd = 5f;
    public float jumpForce = 5f;
    public float throwForce = 50f;
    public float gravity;
    public float range;
    public Camera cam;
    public Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.05f;

    private Vector3 movement;
    public bool isGrounded;

    public enum ItemState { None, Item, Env }
    public Transform leftHand;
    public Transform rightHand;
    public MonoBehaviour leftScript;
    public MonoBehaviour rightScript;

    UX ux;
    SoundManager soundManager;
    GameManager gameManager;
    BulletTime bulletTime;
    float health;
    float maxHealth = 100;

    private float pickupRange = 10f;
    bool isDead;
    public Animator swingAnim; 
    public Animator punchRAnim; 
    public Animator punchLAnim; 
    public Renderer punchR;
    public Renderer punchL;
    public Collider punchRange;
    public Collider batRange;



    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>(); 
        gameManager = FindObjectOfType<GameManager>(); 
        bulletTime = FindObjectOfType<BulletTime>(); 
        ux = FindObjectOfType<UX>(); 
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.freezeRotation = true;
        health = maxHealth;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            gameManager.StartLvl();
        }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            Move();
        }
        else
        {
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
        }

        if (!gameManager.isIntro)
        {
            HandleInput();
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            gameManager.HighlightItems();
        }
        else
        {
            gameManager.HighlightOff();
        }
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                CallRight();
            }
            else
            {
                CallLeft();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CallRight();
        }
    }
    void CallLeft()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Throw(leftScript);
        }
        else
        {
            HandleUse(leftScript, true); 
        }
    }
    void CallRight()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Throw(rightScript);
        }
        else
        {
            HandleUse(rightScript, false); 
        }
    }

    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        movement = transform.right * horzInput + transform.forward * vertInput;
        
        rb.velocity = new Vector3(movement.x * runSpd, rb.velocity.y, movement.z * runSpd);

        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }


    void HandleUse(MonoBehaviour itemScript, bool left)
    {
        if ((left && leftScript == null) || !left && rightScript == null)
        {
            Punch(left);
        }
        else if (itemScript != null)
        {
            itemScript.Invoke("Use", 0f);
        }
    }

    void Punch(bool left)
    {
        if (left)
        {
            StartCoroutine(PunchOn(punchL));
            StartCoroutine(PunchOff(punchL));
            punchLAnim.SetTrigger("Punch"); 
        }
        else
        {
            StartCoroutine(PunchOn(punchR));
            StartCoroutine(PunchOff(punchR));
            punchRAnim.SetTrigger("Punch"); 
        }
    }

    public void Melee(Collider range)
    {
        Collider[] hitColliders = Physics.OverlapBox(range.bounds.center, range.bounds.extents, range.transform.rotation);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("NPC")) 
            {
                hit.GetComponentInParent<Enemy>()?.Hit();
            }
        }
    }

    IEnumerator PunchOn(Renderer punch)
    {
        yield return new WaitForSeconds(0.2f);
        punch.enabled = true;
        punchRange.enabled = true;
        Melee(punchRange);
    }
    IEnumerator PunchOff(Renderer punch)
    {
        yield return new WaitForSeconds(0.5f);
        punch.enabled = false;
        punchRange.enabled = false;
    }

    void Interact()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, range, LayerMask.GetMask("Ground", "Item")) && hit.transform.CompareTag("Item"))
        {
            Item item = hit.transform.GetComponent<Item>(); 
            if (item != null && item.available) 
            {
                if (Vector3.Distance(transform.position, hit.transform.position) <= pickupRange)
                {
                    Pickup(hit.transform.gameObject);
                }
            }
        }
    }

    void Pickup(GameObject item)
    {
        if (leftScript == null)
        {
            InstantiateItem(item, leftHand);
            Destroy(item);
        }
        else if (rightScript == null)
        {
            InstantiateItem(item, rightHand);
            Destroy(item);
        }
    }

    void InstantiateItem(GameObject item, Transform hand)
{
    if (item == null) return;

    GameObject newItem = Instantiate(item, hand.position, Quaternion.identity);
    newItem.transform.SetParent(hand.transform);
    Rigidbody rb = newItem.GetComponent<Rigidbody>();
    rb.isKinematic = true;
    Item env = newItem.GetComponent<Item>();
    env.available = false;
    env.DefaultMat();

    if (newItem.TryGetComponent<Revolver>(out var revolver))
    {
        newItem.transform.localPosition = new Vector3(0f, -0.125f, 0.7f);
        newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        if (hand == leftHand) leftScript = revolver;
        else rightScript = revolver;
    }
    else if (newItem.TryGetComponent<Shotgun>(out var shotgun))
    {
        newItem.transform.localPosition = new Vector3(0f, -0.3f, 0.8f);
        newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);

        if (hand == leftHand) leftScript = shotgun;
        else rightScript = shotgun;
    }
    else if (newItem.TryGetComponent<Bat>(out var bat))
    {
        newItem.transform.localPosition = hand == leftHand ? new Vector3(-0.7f, 0.195f, 2f) : new Vector3(0.7f, 0.195f, 2f);
        newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 125f);

        if (hand == leftHand) leftScript = bat;
        else rightScript = bat;
    }
    else if (newItem.TryGetComponent<Meth>(out var meth))
    {
        newItem.transform.localPosition = hand == leftHand ? new Vector3(0.05f, -0.275f, 0.5f) : new Vector3(-0.05f, -0.275f, 0.5f);
        newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

        if (hand == leftHand) leftScript = meth;
        else rightScript = meth;
    }
    else
    {
        newItem.transform.localPosition = hand == leftHand ? new Vector3(0.05f, -0.275f, 0.5f) : new Vector3(-0.05f, -0.275f, 0.5f);
        newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

        if (hand == leftHand) leftScript = env;
        else rightScript = env;
    }
}


    public void Throw(MonoBehaviour itemScript)
    {
        if (itemScript == leftScript)
        {
            leftScript = null;
        }
        else
        {
            rightScript = null;
        }
        Debug.Log("thrown");
        
        itemScript.transform.SetParent(null);

        Item item = itemScript.GetComponent<Item>();
        item.available = true;
        item.thrown = true;

        Rigidbody rb = itemScript.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 throwDirection;
        if (Physics.Raycast(ray, out hit))
        {
            throwDirection = (hit.point - itemScript.transform.position).normalized;
        }
        else
        {
            throwDirection = ray.direction;
        }

        rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        rb.AddTorque(Vector3.right * -75f, ForceMode.VelocityChange);
    }

    public void SwingBat()
    {
        swingAnim.SetTrigger("Swing");
    }

    public void Hit()
    {
        health -= 4;
        ux.UpdateHealth(health, maxHealth);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            gameManager.CallDead();
            soundManager.Flatline();
        }
        else if (health > 0)
        {
            soundManager.Heartbeat();
        }
    }
}
