using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Anim")]
    public Animator swingAnim;
    public Animator punchRAnim;
    public Animator punchLAnim;
    public Animator throwRAnim;
    public Renderer punchR;
    public Renderer punchL;

    [Header("Combat")]
    public Collider punchRange;

    bool canPunch = true;
    float punchCooldown = 0.75f;

    PlayerController controller;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void Punch(Transform hand)
    {
        if (!canPunch) return;
        canPunch = false;
        StartCoroutine(PunchCooldown());

        if (hand == controller.left)
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
        SoundManager.Instance.Play("Punch");
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

    public void Melee(Collider range)
    {
        Collider[] hits = Physics.OverlapBox(range.bounds.center, range.bounds.extents, range.transform.rotation);
        foreach (Collider hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                IHitable iHit = hit.GetComponentInParent<IHitable>();
                iHit?.Hit(5f);
               // break;
               ScoreManager.Instance?.AddMeleeScore();

            }
        }
    }

}
