using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float runSpd = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float range;
    public Camera cam;
    public CamController camController;
    public Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.2f;

    private Vector3 movement;
    private bool isGrounded;

    public enum WeaponState { None, Revolver, Shotgun, Bat }
    public WeaponState[] weaponSlots = new WeaponState[3];
    public int currentSlot = -1;
    public WeaponState currentWeapon = WeaponState.None;

    public MeleeWeapon melee;
    public Gun gun;
    public UX ux;
    Tutorial tutorial;

    private float health;
    private float maxHealth = 100;
    public float focus;
    private float maxFocus = 100;

    GameManager gameManager;
    SoundManager soundManager;

    public bool isDead = false;
    private float pickupRange = 10f;

    public GameObject revolver;
    public GameObject shotgun;
    public GameObject bat;

    private float swapCooldown = 0.25f;
    private bool onSwap = false;

    private bool hasRevolver = false;
    private bool hasShotgun = false;
    private bool hasBat = false;
    public bool hasWeapon = false;

    public BulletTime bulletTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        health = maxHealth;
        focus = maxFocus;

        ux.UpdateHealth(health, maxHealth);
        ux.UpdateFocus(focus, maxFocus);

        revolver.SetActive(false);
        shotgun.SetActive(false);
        bat.SetActive(false);

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i] = WeaponState.None;
        }
    }

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        tutorial = FindObjectOfType<Tutorial>();
    }

    void Update()
    {
        if (!gameManager.isIntro)
        {
            Move();
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetKey(KeyCode.LeftShift) && focus > 0)
        {
            focus -= 0.025f;
            ux.UpdateFocus(focus, maxFocus);
            bulletTime.Slow();

            if (tutorial.currentState == Tutorial.State.Slow)
            {
                tutorial.States(Tutorial.State.Reload);

                if (hasRevolver)
                {
                    tutorial.passed = true;
                }
            }
        }
        else
        {
            bulletTime.Reg();
        }

        if (Input.GetKey(KeyCode.Q) && !onSwap)
        {
            if (tutorial.currentState == Tutorial.State.Swap)
            {
                tutorial.States(Tutorial.State.Off);
                tutorial.passed = true;
            }
            
            CycleWeapons();
            StartCoroutine(SwapCooldown());
        }

        if (Input.GetKey(KeyCode.Alpha1) && hasBat) EquipWeapon(WeaponState.Bat);
        if (Input.GetKey(KeyCode.Alpha2) && hasRevolver) EquipWeapon(WeaponState.Revolver);
        if (Input.GetKey(KeyCode.Alpha3) && hasShotgun) EquipWeapon(WeaponState.Shotgun);

        if (Input.GetButtonDown("Fire1")) HandleFire();

        if (Input.GetKey(KeyCode.R) && (currentWeapon == WeaponState.Shotgun || currentWeapon == WeaponState.Revolver))
        {
            if (tutorial.currentState == Tutorial.State.Reload)
            {
                tutorial.States(Tutorial.State.Swap);
                tutorial.passed = true;
            }
            gun.Reload();
        }
    }

    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float horzInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        movement = transform.right * horzInput + transform.forward * vertInput;
        
        if ((horzInput > 0 || vertInput > 0) && tutorial.currentState == Tutorial.State.WASD)
        {
            tutorial.States(Tutorial.State.Jump);
            tutorial.passed = true;
        }

        rb.velocity = new Vector3(movement.x * runSpd, rb.velocity.y, movement.z * runSpd);

        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            if (tutorial.currentState == Tutorial.State.Jump)
            {
                if(hasBat)
                {
                    tutorial.index++;
                    tutorial.States(Tutorial.State.Kill);
                    tutorial.passed = true;
                }
                else
                {
                    tutorial.States(Tutorial.State.Grab);
                    tutorial.passed = true;
                }
            }
        }
    }

    void EquipWeapon(WeaponState weapon)
    {
        hasWeapon = true;
        currentWeapon = weapon;
        revolver.SetActive(weapon == WeaponState.Revolver);
        shotgun.SetActive(weapon == WeaponState.Shotgun);
        bat.SetActive(weapon == WeaponState.Bat);
        ux.UpdateAmmo();

        if (weapon == WeaponState.Revolver && tutorial.currentState == Tutorial.State.Reload)
        {
            tutorial.passed = true;
        }
    }

    void HandleFire()
    {
        if (tutorial.currentState == Tutorial.State.Kill)
        {
            tutorial.States(Tutorial.State.Slow);
            tutorial.passed = true;
        }
        
        if (currentWeapon == WeaponState.Revolver || currentWeapon == WeaponState.Shotgun)
        {
            gun.FireWeapon();
            ux.UpdateAmmo();
        }
        else if (currentWeapon == WeaponState.Bat)
        {
            melee.Attack();
        }
    }

    void Interact()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, range, LayerMask.GetMask("Ground", "Item")) && hit.transform.CompareTag("Item"))
        {
            float distanceToPickup = Vector3.Distance(transform.position, hit.transform.position);
            if (distanceToPickup <= pickupRange)
            {
                Pickup(hit.transform.gameObject);
                
                
                if (tutorial.currentState == Tutorial.State.Grab)
                {
                    tutorial.States(Tutorial.State.Kill);
                    tutorial.passed = true;
                }
            }
        }
    }

    void Pickup(GameObject item)
    {
        Item itemScript = item.GetComponent<Item>();

        if (item.name.Contains("Pills"))
        {
            focus = maxFocus;
            ux.UpdateFocus(focus, maxFocus);
        }
        else
        {
            if (item.name.Contains("Revolver") && !hasRevolver) { AddWeaponToSlot(WeaponState.Revolver); hasRevolver = true; }
            else if (item.name.Contains("Shotgun") && !hasShotgun) { AddWeaponToSlot(WeaponState.Shotgun); hasShotgun = true; }
            else if (item.name.Contains("Bat") && !hasBat) { AddWeaponToSlot(WeaponState.Bat); hasBat = true; }
        }
        Destroy(item);
    }

    void AddWeaponToSlot(WeaponState weapon)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == WeaponState.None)
            {
                weaponSlots[i] = weapon;
                currentSlot = i;
                EquipWeapon(weapon);
                break;
            }
        }
    }

    void CycleWeapons()
    {
        do
        {
            currentSlot = (currentSlot + 1) % weaponSlots.Length;
            if (weaponSlots[currentSlot] != WeaponState.None)
            {
                EquipWeapon(weaponSlots[currentSlot]);
                break;
            }
        } while (true);
    }

    IEnumerator SwapCooldown()
    {
        onSwap = true;
        yield return new WaitForSeconds(swapCooldown);
        onSwap = false;
    }

    public void Hit()
    {
        health -= 4;
        ux.UpdateHealth(health, maxHealth);


        if (health <= 0)
        {
            isDead = true;
            gameManager.CallDead();
            soundManager.Flatline();
        }
        else
        {
            soundManager.Heartbeat();
        }
    }
}
