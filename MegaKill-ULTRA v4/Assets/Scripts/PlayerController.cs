using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    float horzInput;
    float vertInput;
    public float runSpd;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float range;
    public Camera cam;
    public CamController camController;
    public CharacterController controller;
    Vector3 vel;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;
    public Transform currentCar;

    public enum WeaponState { None, Revolver, Shotgun, Bat }
    public WeaponState[] weaponSlots = new WeaponState[3]; 
    public int currentSlot = -1;
    public WeaponState currentWeapon = WeaponState.None;

    public MeleeWeapon melee;
    public Gun gun;
    public UX ux;
    Tutorial tutorial;

    float health;
    float maxHealth = 100;
    float focus;
    float maxFocus = 100;

    GameManager gameManager;
    SoundManager soundManager;

    public bool isDead = false;
    float pickupRange = 10f;

    public GameObject revolver;
    public GameObject shotgun;
    public GameObject bat;

    float swapCooldown = 0.1f; 
    bool onSwap = false;

    bool hasRevolver = false;
    bool hasShotgun = false;
    bool hasBat = false;
    public bool hasWeapon = false;

    public BulletTime bulletTime;

    void Start()
    {
        health = maxHealth;
        focus = maxFocus / 2;

        ux.UpdateHealth(health, maxHealth);
        ux.UpdateFocus(focus, maxFocus);

        revolver.SetActive(false);
        shotgun.SetActive(false);
        bat.SetActive(false);

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i] = WeaponState.None;
        }

        transform.Rotate(0f, 90f, 0f);
    }

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        tutorial = FindObjectOfType<Tutorial>(); 
    }
    void Update()
    {
        CallInput();
    }


    void CallInput()
    {
        Move();
        if (Input.GetKey(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(focus > 0)
            {
                focus -= 0.05f;
                ux.UpdateFocus(focus, maxFocus);
                
                bulletTime.Slow();

                if (tutorial.currentState == Tutorial.State.Slow)
                {
                    tutorial.States(Tutorial.State.Grab);
                }
            }
            else
            {
                bulletTime.Reg();
            }
        }
        else
        {
            bulletTime.Reg();
        }
        if (Input.GetKey(KeyCode.Q) && !onSwap)
        {
            
            CycleWeapons();
            StartCoroutine(SwapCooldown());
        }
        if (Input.GetKey(KeyCode.Alpha1) && hasBat)
        {
            EquipWeapon(WeaponState.Bat);
        }

        if (Input.GetKey(KeyCode.Alpha2) && hasRevolver)
        {
            EquipWeapon(WeaponState.Revolver);
        }

        if (Input.GetKey(KeyCode.Alpha3) && hasShotgun)
        {
            EquipWeapon(WeaponState.Shotgun);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            HandleFire();
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (currentWeapon == WeaponState.Shotgun || currentWeapon == WeaponState.Revolver)
            {
                gun.Reload();
                if (tutorial.currentState == Tutorial.State.Reload)
                {
                    tutorial.States(Tutorial.State.Off);
                }
            }
        }
    }
    IEnumerator SwapCooldown()
    {
        onSwap = true;
        yield return new WaitForSeconds(swapCooldown);
        onSwap = false;
    }

    void EquipWeapon(WeaponState weapon)
    {
        hasWeapon = true;
        currentWeapon = weapon;
        revolver.SetActive(weapon == WeaponState.Revolver);
        shotgun.SetActive(weapon == WeaponState.Shotgun);
        bat.SetActive(weapon == WeaponState.Bat);
        ux.UpdateAmmo();
    }

    void HandleFire()
    {
        if (tutorial.currentState == Tutorial.State.WASD && (horzInput != 0 || vertInput != 0))
        {
            tutorial.States(Tutorial.State.Jump);
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

    void Move()
    {
        if (tutorial.currentState == Tutorial.State.WASD && (horzInput != 0 || vertInput != 0))
        {
            tutorial.States(Tutorial.State.Jump);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && vel.y < 0)
        {
            vel.y = -2f;
        }

        vel.y += gravity * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (tutorial.currentState == Tutorial.State.Jump)
            {
                tutorial.States(Tutorial.State.Grab);
            }
        }

        horzInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horzInput + transform.forward * vertInput;

        controller.Move(move * runSpd * Time.deltaTime);

        if (!controller.isGrounded)
        {
            vel.y += gravity * Time.deltaTime;
            controller.Move(vel * Time.deltaTime);
        }
    }

    void Pickup(GameObject item)
    {
        if (item.name.Contains("Pills"))
        {
            focus = maxFocus;
            ux.UpdateFocus(focus, maxFocus);
        }
        else
        {
            revolver.SetActive(false);
            shotgun.SetActive(false);
            bat.SetActive(false);

            if (item.name.Contains("Revolver") && !hasRevolver)
            {
                AddWeaponToSlot(WeaponState.Revolver);
                hasRevolver = true;
            }
            else if (item.name.Contains("Shotgun") && !hasShotgun)
            {
                AddWeaponToSlot(WeaponState.Shotgun);
                hasShotgun = true;
            }
            else if (item.name.Contains("Bat") && !hasBat)
            {
                AddWeaponToSlot(WeaponState.Bat);
                hasBat = true;
            }

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
                currentSlot++;
                EquipWeapon(weapon);
                break;
            }
        }
    }

    void CycleWeapons()
    {
        int startSlot = currentSlot;
        do
        {
            currentSlot = (currentSlot + 1) % weaponSlots.Length;
            if (weaponSlots[currentSlot] != WeaponState.None)
            {
                EquipWeapon(weaponSlots[currentSlot]);
                break;
            }
        } while (currentSlot != startSlot);
    }

    void Interact()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        int layerMask = LayerMask.GetMask("Ground", "Item");

        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            if (hit.transform.CompareTag("Item"))
            {
                float distanceToPickup = Vector3.Distance(transform.position, hit.transform.position);

                if (distanceToPickup <= pickupRange)
                {
                    Pickup(hit.transform.gameObject);

                    if (tutorial.currentState == Tutorial.State.Grab && (horzInput != 0 || vertInput != 0))
                    {
                        tutorial.States(Tutorial.State.Kill);
                    }
                }
            }
        }
    }

    public void Hit()
    {
        health -= 4;
        ux.UpdateHealth(health, maxHealth);

        if (health <= 0)
        {
            isDead = true;
            gameManager.fadeOut = true;
            soundManager.Flatline();
            StartCoroutine(Dead());
            Debug.Log("KILLED");
        }
        else
        {
            soundManager.Heartbeat();
        }
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
