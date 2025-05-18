using System.Collections;
using UnityEngine;

public class EnemyPunch : Enemy
{
    
    protected override void Start()
    {
        attackRange = 6f;
        attackRate = 1f;
        dmg = 10f;
    }
    protected override void CallAttack()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        yield return new WaitForSeconds(0.2f);

        IHitable iHit = target.GetComponent<IHitable>();
        iHit?.Hit(dmg);

        soundManager.EnemySFX(sfx, attackClip);
        yield break;
    }
}