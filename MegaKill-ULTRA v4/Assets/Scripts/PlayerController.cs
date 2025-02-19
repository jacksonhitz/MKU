using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float runSpd = 5f;
    public float jumpForce = 5f;
    public float gravity;
    public float range;
    public Camera cam;
    public Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.05f;

    private Vector3 movement;
    public bool isGrounded;

    public enum ItemState { None, Revolver, Shotgun, Bat, Meth, Env }
    public Transform leftHand;
    public Transform rightHand;
    public MonoBehaviour leftItemScript;
    public MonoBehaviour rightItemScript;
    public ItemState leftItem = ItemState.None;
    public ItemState rightItem = ItemState.None;

    UX ux;
    SoundManager soundManager;
    GameManager gameManager;
    BulletTime bulletTime;
    float health;
    float maxHealth = 100;
    public float focus;
    float maxFocus = 100;

    private float pickupRange = 10f;
    bool isDead;
    public Animator swingAnim; 
    public Animator punchRAnim; 
    public Animator punchLAnim; 
    public Renderer punchR;
    public Renderer punchL;
    public Collider meleeRange;

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
        focus = maxFocus;
        ux.UpdateHealth(health, maxHealth);
        ux.UpdateFocus(focus, maxFocus);
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            Move();
        }
        else
        {
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
        }
        HandleInput();
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
                CallLeft();
            }
            else
            {
                CallRight();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CallLeft();
        }
        else
        {
            bulletTime.Reg();
        }
    }

    void CallLeft()
    {
        if (Input.GetKey(KeyCode.LeftShift) && leftItem != ItemState.None)
        {
            leftItem = ItemState.None;
            Throw(leftItemScript);
        }
        else
        {
            HandleUse(leftItemScript, true); 
        }
    }
    void CallRight()
    {
        if (Input.GetKey(KeyCode.LeftShift) && rightItem != ItemState.None)
        {
            rightItem = ItemState.None;
            Throw(rightItemScript);
        }
        else
        {
            HandleUse(rightItemScript, false); 
        }
    }

    void Move()
    {
        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        movement = transform.right * horzInput + transform.forward * vertInput;
        rb.velocity = new Vector3(movement.x * runSpd, rb.velocity.y, movement.z * runSpd);
        if (Input.GetKey(KeyCode.Space) && isGrounded) rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    void HandleUse(MonoBehaviour itemScript, bool left)
    {
        if (itemScript != null)
        {
            itemScript.Invoke("Use", 0f);
        }
        else
        {
            Punch(left);
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

    IEnumerator PunchOn(Renderer punch)
    {
        yield return new WaitForSeconds(0.2f);
        punch.enabled = true;
        meleeRange.enabled = true;

        Collider[] hitColliders = Physics.OverlapBox(meleeRange.bounds.center, meleeRange.bounds.extents, meleeRange.transform.rotation);
        foreach (Collider hit in hitColliders)
        {
            Debug.Log("punch: " + hit.tag);
            if (hit.CompareTag("NPC")) 
            {
                hit.GetComponentInParent<Enemy>()?.Hit();
            }
        }
    }

    IEnumerator PunchOff(Renderer punch)
    {
        yield return new WaitForSeconds(0.5f);
        punch.enabled = false;
        meleeRange.enabled = false;

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
        if (rightItem == ItemState.None) EquipItem(item, rightHand, ref rightItem, ref rightItemScript);
        else if (leftItem == ItemState.None) EquipItem(item, leftHand, ref leftItem, ref leftItemScript);
    }

    void EquipItem(GameObject item, Transform hand, ref ItemState itemSlot, ref MonoBehaviour itemScript)
    {
        item.transform.SetParent(hand);
        item.transform.localRotation = Quaternion.identity; 

        Item script = item.GetComponent<Item>();
        script.available = false;

        if (item.name.Contains("Revolver"))
        {
            item.transform.localPosition = new Vector3(0f, -0.125f, 0.7f);
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); 
            itemSlot = ItemState.Revolver;
            itemScript = item.GetComponent<Revolver>();
            itemScript.Invoke("SetPos", 0f);
            Rigidbody rb = itemScript.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        else if (item.name.Contains("Shotgun"))
        {
            item.transform.localPosition = new Vector3(0f, -0.3f, 0.8f);
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f); 
            itemSlot = ItemState.Shotgun;
            itemScript = item.GetComponent<Shotgun>();
            itemScript.Invoke("SetPos", 0f);
            Rigidbody rb = itemScript.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        else if (item.name.Contains("Bat"))
        {
            if (hand == leftHand)
            {
                item.transform.localPosition = new Vector3(-0.7f, 0.195f, 2f);
            }
            else
            {
                item.transform.localPosition = new Vector3(0.7f, 0.195f, 2f);
            }
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 125f); 
            itemSlot = ItemState.Bat;
            itemScript = item.GetComponent<Bat>();
            Rigidbody rb = itemScript.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        else if (item.name.Contains("Meth"))
        {
            item.transform.localPosition = new Vector3(0f, -0.125f, 0.7f);
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); 
            itemSlot = ItemState.Meth;
            itemScript = item.GetComponent<Meth>();
            Rigidbody rb = item.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void Throw(MonoBehaviour itemScript)
    {
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

        rb.AddForce(throwDirection * 30f, ForceMode.VelocityChange);
        rb.AddTorque(Vector3.right * -50f, ForceMode.VelocityChange);
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
