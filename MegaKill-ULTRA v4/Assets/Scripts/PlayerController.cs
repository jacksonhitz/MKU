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
    public float reach;
    public Camera cam;
    public CamController camController;
    public CharacterController controller;
    Vector3 vel;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;
    public Transform currentCar;

    public enum State { foot, driving }
    public State currentState = State.foot;

    public Gun gun;
    public UX ux;

    bool hasMoved = false;
    bool hasFired = false;
    bool hasReloaded = false;

    int health;
    int maxHealth = 100; 

    GameManager gameManager;
    SoundManager soundManager;

    public bool isDead = false;

    [SerializeField] FloatingHealthBar healthBar; 

    public int weapon = 0;

    public GameObject revolver;
    public GameObject shotgun;

    float swapCooldown = .5f;
    bool onSwap = false;

    // track if weapons have been picked up
    bool hasRevolver = false;
    bool hasShotgun = false;


    void Start()
    {
        health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);
        soundManager.Hammer();

        // disable weapons at start of game
        revolver.SetActive(false);
        shotgun.SetActive(false);
        weapon = -1; // No weapon equipped

        transform.Rotate(0f, 90f, 0f);
    }

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        healthBar = GetComponentInChildren<FloatingHealthBar>(); 
    }

    void Update()
    {
        Move();

        if (Input.GetKey(KeyCode.E))
        {
            Interact();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (!onSwap)
            {
                if (weapon == 1)
                {
                    weapon = 0;
                    revolver.SetActive(true);
                    shotgun.SetActive(false);
                    ux.UpdateAmmo();
                    soundManager.Hammer();
                }
                else if (weapon == 0)
                {
                    weapon = 1;
                    shotgun.SetActive(true);
                    revolver.SetActive(false);
                    ux.UpdateAmmo();
                    soundManager.Pump();
                }
                StartCoroutine(Swap());
            }
        }

        // Allow switching only if the weapon has been picked up
        if (Input.GetKey(KeyCode.Alpha1) && hasRevolver)
        {
            weapon = 0;
            revolver.SetActive(true);
            shotgun.SetActive(false);
            ux.UpdateAmmo();
            soundManager.Hammer();
        }

        if (Input.GetKey(KeyCode.Alpha2) && hasShotgun)
        {
            weapon = 1;
            shotgun.SetActive(true);
            revolver.SetActive(false);
            ux.UpdateAmmo();
            soundManager.Pump();
        }


        if (Input.GetKey(KeyCode.Alpha3))
        {
            weapon = 2;
            shotgun.SetActive(false);
            revolver.SetActive(false);
            ux.UpdateAmmo();
        }


        if (Input.GetButtonDown("Fire1"))
        {
            if (weapon == 1)
            {
                if(gun.shells > 0)
                {
                    gun.ShotgunFire();
                    ux.UpdateAmmo();
                }
                else
                {
                    soundManager.Empty();
                }
            }
            if(weapon == 0)
            {
                if(gun.bullets > 0)
                {
                    gun.RevolverFire();
                    ux.UpdateAmmo();
                }
                else
                {
                    soundManager.Empty();
                }
            }


            if (!hasFired)
            {
                ux.Reload();    
                hasFired = true;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            gun.Reload();
            if (!hasReloaded)
            {
                ux.Slow();
                hasReloaded = true;
            }
        }

        if (!controller.isGrounded)
        {
            vel.y += gravity * Time.deltaTime;
            controller.Move(vel * Time.deltaTime);
        }
    }

    IEnumerator Swap()
    {
        onSwap = true;
        yield return new WaitForSeconds (swapCooldown);
        onSwap = false;


    }

    void Bail()
    {
        transform.position += Vector3.left * 0.2f;
        currentState = State.foot;
    }

    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && vel.y < 0)
        {
            vel.y = -2f;
            
        }
        vel.y += gravity * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        horzInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horzInput + transform.forward * vertInput;

        if (!hasMoved && (horzInput != 0 || vertInput != 0))
        {
            ux.Kill();
            hasMoved = true;
        }

        controller.Move(move * runSpd * Time.deltaTime);
    }

    void PickupWeapon(GameObject weaponObject)
    {
        // Disable currently equipped weapon to prevent overlapping models
        if (weapon == 0) revolver.SetActive(false);
        if (weapon == 1) shotgun.SetActive(false);

        // Enable the picked-up weapon and update tracking
        if (weaponObject.name.Contains("Revolver"))
        {
            revolver.SetActive(true);
            weapon = 0;
            hasRevolver = true; // Mark as picked up
        }
        else if (weaponObject.name.Contains("Shotgun"))
        {
            shotgun.SetActive(true);
            weapon = 1;
            hasShotgun = true; // Mark as picked up
        }

        Destroy(weaponObject); // Remove the pickup item
    }

    void Interact()
{
    // fires raycast to check if player is looking at weapon
    RaycastHit hit;
    Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

    if (Physics.Raycast(ray, out hit, range))
    {
        if (hit.transform.CompareTag("WeaponPickups"))
        {
            /*if (hit.transform.CompareTag("Car"))
            {
                float gap = Vector3.Distance(transform.position, hit.transform.position);
                if (gap <= reach)
                {
                    currentState = State.driving;
                    currentCar = hit.transform;
                }
            }*/
            PickupWeapon(hit.transform.gameObject); // pick up the weapon
        }
    }
}

    public void Hit()
    {
        health -= 4;
        healthBar.UpdateHealthBar(health, maxHealth); 

        if (health <= 0)
        {
            isDead = true;
            gameManager.fadeOut = true;
            soundManager.Flatline();
            StartCoroutine(Dead());
            Debug.Log("KILLED");
            //ux.gameObject.SetActive(false);
        }
        else
        {
            soundManager.Heartbeat();
        }
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds (5f);

        SceneManager.LoadScene(0);
    }
}
