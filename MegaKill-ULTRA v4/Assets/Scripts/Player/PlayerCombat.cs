using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : ValidatedMonoBehaviour
{
    private static readonly int PunchKey = Animator.StringToHash("Punch");

    [Header("Anim")]
    public Animator swingAnim;
    public Animator punchRAnim;
    public Animator punchLAnim;
    public Animator throwRAnim;
    public Renderer punchR;
    public Renderer punchL;

    [Header("Combat")]
    [SerializeField]
    private Collider punchRange;

    [SerializeField]
    float punchCooldown = 0.75f;

    [SerializeField, HideInInspector, Self]
    PlayerController controller;

    bool canPunch = true;

    private void OnEnable()
    {
        InputManager.PlayerActionMap.UseLeft.performed += UseLeftOnPerformed;
        InputManager.PlayerActionMap.UseRight.performed += UseRightOnPerformed;
    }

    private void OnDisable()
    {
        InputManager.PlayerActionMap.UseLeft.performed -= UseLeftOnPerformed;
        InputManager.PlayerActionMap.UseRight.performed -= UseRightOnPerformed;
    }

    private void UseLeftOnPerformed(InputAction.CallbackContext obj)
    {
        if (!controller.items.leftItem)
            Punch(controller.left);
    }

    private void UseRightOnPerformed(InputAction.CallbackContext obj)
    {
        if (!controller.items.rightItem)
            Punch(controller.right);
    }

    private void Punch(Transform hand)
    {
        if (!canPunch)
            return;
        canPunch = false;
        StartCoroutine(PunchCooldown());

        if (hand == controller.left)
        {
            StartCoroutine(PunchOn(punchL));
            StartCoroutine(PunchOff(punchL));
            punchLAnim.SetTrigger(PunchKey);
        }
        else
        {
            StartCoroutine(PunchOn(punchR));
            StartCoroutine(PunchOff(punchR));
            punchRAnim.SetTrigger(PunchKey);
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
        SoundManager.Instance.CreateSoundBuilder().Play("Punch");
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

    private void Melee(Collider range)
    {
        Collider[] hits = Physics.OverlapBox(
            range.bounds.center,
            range.bounds.extents,
            range.transform.rotation
        );
        foreach (Collider hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                IHitable iHit = hit.GetComponentInParent<IHitable>();
                iHit?.Hit(5f);
                ScoreManager.Instance?.AddMeleeScore();
            }
        }
    }
}
