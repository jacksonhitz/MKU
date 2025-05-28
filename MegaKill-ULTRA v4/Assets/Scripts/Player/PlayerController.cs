using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHit
{
    [Header("Movement")]
    public float runSpd;
    public float throwForce;
    public float gravity;
    public float range;
    public float groundDistance;
    public float jumpHeight = 5f;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("References")]
    public Camera cam;
    public Transform leftHand;
    public Transform rightHand;
    public MonoBehaviour leftScript;
    public MonoBehaviour rightScript;
    public Animator swingAnim;
    public Animator punchRAnim;
    public Animator punchLAnim;
    public Animator throwRAnim;
    public Renderer punchR;
    public Renderer punchL;
    public Collider punchRange;
    public Collider batRange;

    [Header("Status")]
    public bool isGrounded;
    public bool isDead;
    public bool rooted;

    float verticalVelocity;
    float health;
    float maxHealth = 100;
    bool canPunch = true;
    float punchCooldown = 0.75f;
    float interactRange = 20f;

    CharacterController characterController;
    SoundManager soundManager;
    UEye uEye;
    PopUp popUp;
    Festival festival;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        uEye = FindObjectOfType<UEye>();
        popUp = FindObjectOfType<PopUp>();
        festival = FindObjectOfType<Festival>();
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
            MoveInputs();
        }
    }
    // PLAYERINPUTS
    void MoveInputs() { }

    public void Move(Vector2 moveDir, bool jump)
    {
        isGrounded = StateManager.State == StateManager.GameState.TESTING || Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 movement = transform.right * moveDir.x + transform.forward * moveDir.y;

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
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (jump && isGrounded && !rooted)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 finalMove = (movement * runSpd) + Vector3.up * verticalVelocity;
        characterController.Move(finalMove * Time.deltaTime);
    }

    public void UseLeft()
    {
        if (leftScript == null)
            Punch(true);
        else
            leftScript.Invoke("Use", 0f);
    }

    public void UseRight()
    {
        if (rightScript == null)
            Punch(false);
        else
            rightScript.Invoke("Use", 0f);
    }

    public void Left()
    {
        if (leftScript == null)
            Interact(true);
        else
            Throw(leftScript);
    }

    public void Right()
    {
        if (rightScript == null)
            Interact(false);
        else
        {
            Throw(rightScript);
            throwRAnim.SetTrigger("Throw");
        }
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

    IEnumerator PunchOn(Renderer punch)
    {
        yield return new WaitForSeconds(0.2f);
        soundManager?.Punch();
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
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.TryGetComponent(out Item item))
            {
                Pickup(item.gameObject, left);
            }
        }
    }

    void Pickup(GameObject item, bool left)
    {
        if (left && leftScript != null) return;
        if (!left && rightScript != null) return;

        if (item.TryGetComponent(out MonoBehaviour script))
        {
            if (left)
            {
                InstantiateItem(item, leftHand);
                leftScript = script;
            }
            else
            {
                InstantiateItem(item, rightHand);
                rightScript = script;
            }
            item.SetActive(false);
        }
    }

    void InstantiateItem(GameObject item, Transform hand)
    {
        GameObject clone = Instantiate(item, hand.position, hand.rotation);
        clone.transform.SetParent(hand);
        clone.GetComponent<Collider>().enabled = false;
        clone.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Throw(MonoBehaviour itemScript)
    {
        Transform hand = itemScript == leftScript ? leftHand : rightHand;
        itemScript.Invoke("StopUse", 0f);

        GameObject item = hand.GetChild(0).gameObject;
        item.transform.SetParent(null);
        item.SetActive(true);
        item.GetComponent<Collider>().enabled = true;
        item.GetComponent<Rigidbody>().isKinematic = false;
        item.GetComponent<Rigidbody>().AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        if (itemScript == leftScript)
            leftScript = null;
        else
            rightScript = null;
    }

    public void SwingBat()
    {
        swingAnim.SetTrigger("Swing");
    }

    public void Heal(float heal)
    {
        if (isDead) return;
        health += heal;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void Hit(float dmg)
    {
        if (isDead) return;
        health -= dmg;
        if (health <= 0)
            StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(3f);
        StateManager.LoadState(StateManager.GameState.DEAD);
    }

    public void Melee(Collider range)
    {
        Collider[] hits = Physics.OverlapBox(range.bounds.center, range.bounds.extents, range.transform.rotation);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out IHit iHit))
            {
                iHit.Hit(20f);
            }
        }
    }
}
