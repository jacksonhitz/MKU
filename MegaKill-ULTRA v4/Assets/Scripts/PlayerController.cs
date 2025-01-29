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



    void Start()
    {
        health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);

        soundManager.Hammer();

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

        if (Input.GetKey(KeyCode.Alpha1))
        {
            weapon = 0;
            revolver.SetActive(true);
            shotgun.SetActive(false);
            ux.UpdateAmmo();
            soundManager.Hammer();
        }
        if (Input.GetKey(KeyCode.Alpha2))
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
                ux.Police();
                hasReloaded = true;
            }
        }

        if (!controller.isGrounded)
        {
            vel.y += gravity * Time.deltaTime;
            controller.Move(vel * Time.deltaTime);
        }
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

    void Interact()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.transform.CompareTag("Car"))
            {
                float gap = Vector3.Distance(transform.position, hit.transform.position);
                if (gap <= reach)
                {
                    currentState = State.driving;
                    currentCar = hit.transform;
                }
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

        SceneManager.LoadScene(1);
    }
}
