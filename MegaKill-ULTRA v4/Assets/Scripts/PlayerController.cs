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
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;

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

    float pickupRange = 10f;
    bool isDead;
    public Animator swingAnim;
    public Animator punchRAnim;
    public Animator punchLAnim;
    public Renderer punchR;
    public Renderer punchL;
    public Collider punchRange;
    public Collider batRange;

    public bool rooted;
    bool canPunch = true;
    float punchCooldown = .75f;

    CharacterController characterController;
    Vector3 velocity;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        bulletTime = FindObjectOfType<BulletTime>();
        ux = FindObjectOfType<UX>();
        characterController = GetComponent<CharacterController>(); 

    }

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (!rooted)
        {
            Move();
        }

        if (!gameManager.isIntro)
        {
            HandleInput();
        }
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                HandleUse(rightScript, false);
            }
            else
            {
                HandleUse(leftScript, true);
            }
        }
        else if (Input.GetMouseButton(1))
        {
            HandleUse(rightScript, false);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (leftScript != null)
            {
                Throw(leftScript);
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (rightScript != null)
            {
                Throw(rightScript);
            }
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

    void Move()
{
    float horzInput = Input.GetAxis("Horizontal");
    float vertInput = Input.GetAxis("Vertical");
    movement = transform.right * horzInput + transform.forward * vertInput;

    // Apply movement input
    characterController.Move(movement * runSpd * Time.deltaTime);
    gravity = -9.81f * 10; 

    // Apply gravity manually to the position (directly alter the Y position)
    Vector3 gravityEffect = new Vector3(0f, gravity, 0f);
    characterController.Move(gravityEffect * Time.deltaTime);
}


    void Punch(bool left)
    {
        if (!canPunch) return;
        canPunch = false;
        StartCoroutine(PunchCooldown());
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

    IEnumerator PunchCooldown()
    {
        yield return new WaitForSeconds(punchCooldown);
        canPunch = true;
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
        soundManager.Punch();
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

        Transform firePoint = newItem.transform.Find("FirePoint");
        if (firePoint != null)
        {
            firePoint.SetParent(hand);
            firePoint.localPosition = new Vector3(0f, -0.15f, 1.5f);
            firePoint.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        Item env = newItem.GetComponent<Item>();
        gameManager.CollectItems();
        env.CollidersOff();
        env.DefaultMat();

        if (newItem.TryGetComponent<Revolver>(out var revolver))
        {
            newItem.transform.localPosition = new Vector3(0f, -0.15f, 0.35f);
            newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            if (hand == leftHand) leftScript = revolver;
            else rightScript = revolver;
        }
        else if (newItem.TryGetComponent<Shotgun>(out var shotgun))
        {
            newItem.transform.localPosition = new Vector3(0f, -0.3f, 0.7f);
            newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);

            if (hand == leftHand) leftScript = shotgun;
            else rightScript = shotgun;
        }
        else if (newItem.TryGetComponent<MG>(out var mg))
        {
            newItem.transform.localPosition = new Vector3(0f, -0.3f, 0.7f);
            newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            if (hand == leftHand) leftScript = mg;
            else rightScript = mg;
        }
        else if (newItem.TryGetComponent<Bat>(out var bat))
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(-0.7f, 0.3f, 2f) : new Vector3(0.7f, 0.195f, 2f);
            newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 125f);

            if (hand == leftHand) leftScript = bat;
            else rightScript = bat;
        }
        else if (newItem.TryGetComponent<Meth>(out var meth))
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(-0.05f, -0.275f, 0.5f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = meth;
            else rightScript = meth;
        }
        else if (newItem.TryGetComponent<Beer>(out var beer))
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(-0.05f, -0.275f, 0.5f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = beer;
            else rightScript = beer;
        }
        else
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(-0.05f, -0.275f, 0.5f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = env;
            else rightScript = env;
        }
    }

    public void Throw(MonoBehaviour itemScript)
    {
        soundManager.Toss();

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

        item.thrown = true;
        item.CollidersOn();

        Rigidbody itemRb = itemScript.GetComponent<Rigidbody>();
        itemRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        itemRb.AddTorque(Vector3.right * -75f, ForceMode.VelocityChange);
    }

    public void SwingBat()
    {
        swingAnim.SetTrigger("Swing");
    }

    public void Heal()
    {
        soundManager.Gulp();
        health += 4;
        if (health > 100)
        {
            health = 100;
        }
        ux.UpdateHealth(health, maxHealth);
    }

    public void Hit()
    {
        Debug.Log("PLAYER HIT");

        health -= 4;
        ux.UpdateHealth(health, maxHealth);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            gameManager.CallDead();
            soundManager.PlayerDeath();
        }
        else if (health > 0)
        {
            soundManager.PlayerHit();
        }
    }
}
