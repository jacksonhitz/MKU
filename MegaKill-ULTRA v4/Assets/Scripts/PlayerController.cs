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

    public enum WeaponState { None, Revolver, Shotgun, Bat }
    public WeaponState left = WeaponState.None;
    public WeaponState right = WeaponState.None;

    UX ux;
    SoundManager soundManager;
    GameManager gameManager;
    float health;
    float maxHealth = 100;
    public float focus;
    float maxFocus = 100;

    private float pickupRange = 10f;
    public GameObject revolverL;
    public GameObject shotgunL;
    public GameObject batL;
    public GameObject revolverR;
    public GameObject shotgunR;
    public GameObject batR;

    public Bat batLScript;
    public Gun gunLScript;
    public Bat batRScript;
    public Gun gunRScript;

    bool isDead;

    Animator animator; 
    public Renderer punch;


    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>(); 
        gameManager = FindObjectOfType<GameManager>(); 
        ux = FindObjectOfType<UX>(); 
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
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
        if (Input.GetKey(KeyCode.E)) Interact();

        if (Input.GetButtonDown("Fire1"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                HandleFire(right);
            }
            else
            {
                HandleFire(left);
            }
        }
        if (Input.GetButtonDown("Fire2")) HandleFire(right);
    }

    void Move()
    {
        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        movement = transform.right * horzInput + transform.forward * vertInput;
        rb.velocity = new Vector3(movement.x * runSpd, rb.velocity.y, movement.z * runSpd);
        if (Input.GetKey(KeyCode.Space) && isGrounded) rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    void HandleFire(WeaponState weapon)
    {
        if (weapon == left)
        {
            if (weapon == WeaponState.Revolver || weapon == WeaponState.Shotgun)
            {
                gunLScript.FireWeapon(weapon);  
            }
            else if (weapon == WeaponState.Bat)
            {
                batLScript.Attack();  
            }
            else
            {
                Punch();
            }
        }
        else
        {
            if (weapon == WeaponState.Revolver || weapon == WeaponState.Shotgun)
            {
                gunRScript.FireWeapon(weapon);  
            }
            else if (weapon == WeaponState.Bat)
            {
                batRScript.Attack();  
            }
            else
            {
                Punch();
            }
        }
    }

    void Punch()
    {
        Debug.Log("Punch!");
        if (animator != null)
        {

            StartCoroutine(PunchOn());
            StartCoroutine(PunchOff());
            animator.SetTrigger("Punch"); 
            Debug.Log("good");
        }
    }
    IEnumerator PunchOn()
    {
        yield return new WaitForSeconds(0.2f);
        punch.enabled = true;
    }

    IEnumerator PunchOff()
    {
        yield return new WaitForSeconds(0.5f);
        punch.enabled = false;
    }

    void Interact()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, range, LayerMask.GetMask("Ground", "Item")) && hit.transform.CompareTag("Item"))
        {
            if (Vector3.Distance(transform.position, hit.transform.position) <= pickupRange)
                Pickup(hit.transform.gameObject);
        }
    }

    void Pickup(GameObject item)
    {
        WeaponState newItem = WeaponState.None;
        GameObject objL = null;
        GameObject objR = null;

        if (item.name.Contains("Revolver")) { newItem = WeaponState.Revolver; objL = revolverL; objR = revolverR; }
        else if (item.name.Contains("Shotgun")) { newItem = WeaponState.Shotgun; objL = shotgunL; objR = shotgunR; }
        else if (item.name.Contains("Bat")) { newItem = WeaponState.Bat; objL = batL; objR = batR; }

        if (newItem != WeaponState.None)
        {
            if (left == WeaponState.None)
            {
                left = newItem;
                if (objL) objL.SetActive(true);
            }
            else if (right == WeaponState.None)
            {
                right = newItem;
                if (objR) objR.SetActive(true);
            }
            else
            {
                if (left != WeaponState.None)
                {
                    DropWeapon(left);
                    left = newItem;
                    if (objL) objL.SetActive(true);
                }
                else if (right != WeaponState.None)
                {
                    DropWeapon(right);
                    right = newItem;
                    if (objR) objR.SetActive(true);
                }
            }
        }
        Destroy(item);
    }

    void DropWeapon(WeaponState weapon)
    {

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
