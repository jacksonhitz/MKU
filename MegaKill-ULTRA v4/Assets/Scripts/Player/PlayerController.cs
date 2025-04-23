using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHit
{
    public float runSpd;
    public float throwForce;
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

    SoundManager soundManager;
    GameManager gameManager;
    Settings settings;
    UEye uEye;
    FestivalManager festivalManager;
    PopUp popUp;

    float health;
    float maxHealth = 100;

    float interactRange = 20f;
    public bool isDead;
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

    float verticalVelocity;
    public float jumpHeight = 5f;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        settings = FindObjectOfType<Settings>();
        uEye = FindObjectOfType<UEye>();
        popUp = FindObjectOfType<PopUp>();
        festivalManager = FindObjectOfType<FestivalManager>();

        characterController = GetComponent<CharacterController>(); 
    }

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (StateManager.IsActive())
        {
            HandleInput();
        }
        Move();
    }

    public void SpeedUp()
    {
        runSpd += 0.3f;
    }

    void HandleInput()
    {
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (leftScript == null)
            {
                Interact(true);
            }
            else
            {
                Throw(leftScript);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (rightScript == null)
            {
                Interact(false);
            }
            else
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
        if (StateManager.State == StateManager.GameState.Testing)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        movement = transform.right * horzInput + transform.forward * vertInput;

        if (!rooted)
        {
            characterController.Move(movement * runSpd * Time.deltaTime);
        }

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += -9.81f * 10 * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !rooted)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * -9.81f * 10);
        }

        Vector3 finalMove = (movement * runSpd) + Vector3.up * verticalVelocity;
        characterController.Move(finalMove * Time.deltaTime);
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
            Debug.Log("hit1");
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("hit2");
                Enemy enemy = hit.GetComponentInParent<Enemy>();
                enemy.Hit(50);
            }
        }
    }

    IEnumerator PunchOn(Renderer punch)
    {
        yield return new WaitForSeconds(0.2f);

        if (soundManager != null)
        {
            soundManager.Punch();
        }
    
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

    void Interact(bool left)
    {
        Debug.Log("interacted");

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, range, LayerMask.GetMask("Ground", "Item", "NPC")))
        {
            int hitLayer = hit.transform.gameObject.layer;

            if (hitLayer == LayerMask.NameToLayer("Item"))
            {
                Item item = hit.transform.GetComponent<Item>();
                if (item != null && item.currentState == Item.ItemState.Available)
                {
                    if (Vector3.Distance(transform.position, hit.transform.position) <= interactRange)
                    {
                        Pickup(hit.transform.gameObject, left);
                        item.SetState();
                    }
                }
            }
            else if (hitLayer == LayerMask.NameToLayer("NPC"))
            {
                Debug.Log("NPC");
                Transform current = hit.transform;
                Enemy enemy = null;

                while (current != null)
                {
                    enemy = current.GetComponent<Enemy>();
                    if (enemy != null)
                        break;
                    current = current.parent;
                }

                if (enemy != null && enemy.friendly)
                {
                    if (Vector3.Distance(transform.position, hit.transform.position) <= interactRange)
                    {
                        if (festivalManager != null)
                        {
                            festivalManager.Dosed(enemy);
                        }
                    }
                }
            }
        }
    }


    void Pickup(GameObject item, bool left)
    {
        if (left)
        {
            InstantiateItem(item, leftHand);
            Destroy(item);
        }
        else
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

        if (gameManager != null)
        {
            gameManager.CollectItems();
        }

        env.CollidersOff();
        env.DefaultMat();

        if (newItem.TryGetComponent<Revolver>(out var revolver))
        {
            newItem.transform.localPosition = new Vector3(0f, -0.15f, 0.5f);
            newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            if (hand == leftHand) leftScript = revolver;
            else rightScript = revolver;
        }
        else if (newItem.TryGetComponent<Shotgun>(out var shotgun))
        {
            newItem.transform.localPosition = new Vector3(0f, -0.35f, 0.5f);
            newItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);

            if (hand == leftHand) leftScript = shotgun;
            else rightScript = shotgun;
        }
        else if (newItem.TryGetComponent<MG>(out var mg))
        {
            newItem.transform.localPosition = new Vector3(0f, -0.3f, 0.5f);
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
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(0, -0.3f, 0.4f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand)
            {
                leftScript = meth;
                meth.left = true;
            }
            else
            {
                rightScript = meth;
                meth.left = false;
            }
        }
        else if (newItem.TryGetComponent<Beer>(out var beer))
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(0f, -0.3f, 0.4f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = beer;
            else rightScript = beer;
        }
        else if (newItem.TryGetComponent<Coffee>(out var coffee))
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(0f, -0.3f, 0.4f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = coffee;
            else rightScript = coffee;
        }
        else if (newItem.TryGetComponent<Cola>(out var cola))
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(0f, -0.3f, 0.4f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = cola;
            else rightScript = cola;
        }
        else
        {
            newItem.transform.localPosition = hand == leftHand ? new Vector3(0f, -0.3f, 0.4f) : new Vector3(0f, -0.4f, 0.4f);
            newItem.transform.localRotation = Quaternion.Euler(-110f, hand == leftHand ? -90f : 90f, 0f);

            if (hand == leftHand) leftScript = env;
            else rightScript = env;
        }
    }

    public void Throw(MonoBehaviour itemScript)
    {
        if (soundManager != null)
        {
            soundManager.Toss();
        }
    
        if (itemScript == leftScript)
        {
            leftScript = null;
        }
        else
        {
            rightScript = null;
        }

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
        item.currentState = Item.ItemState.Available;

        Rigidbody itemRb = itemScript.GetComponent<Rigidbody>();
        Quaternion initialRotation = itemScript.transform.rotation;

        itemRb.velocity = throwDirection * throwForce;
        itemRb.angularVelocity = Vector3.right * -10f;
        itemScript.transform.rotation = initialRotation;
    }

    public void SwingBat()
    {
        swingAnim.SetTrigger("Swing");
    }

    public void Heal(float heal)
    {
        health += heal;
        if (health > 100)
        {
            health = 100;
        }

        uEye.UpdateHealth(health);
    }

    public void Hit(float dmg)
    {
        health -= dmg;

        if (health <= 0 && !isDead)
        {
            isDead = true;

            if (gameManager != null)
            {
                gameManager.CallDead();
            }

            if (soundManager != null)
            {
                soundManager.PlayerDeath();
            }
        }
        else
        {
            if (soundManager != null)
            {
                soundManager.PlayerHit();
            }
        }

        uEye.UpdateHealth(health);
    }
}
