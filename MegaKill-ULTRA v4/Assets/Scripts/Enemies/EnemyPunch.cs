using System.Collections;
using UnityEngine;

public class EnemyPunch : Enemy
{
    protected override void DefaultValues()
    {
        attackRange = 6f;
        attackRate = 1f;
        damage = 10f;
    }

    protected override void CallAttack()
    {
        StartCoroutine(Punch());
    }

    public override void CallUse()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Punch()
    {
        animator.SetTrigger("Punch");
        yield return new WaitForSeconds(0.2f);

        IHitable iHit = target.GetComponent<IHitable>();
        iHit?.Hit(damage);

        sound.CreateSoundBuilder().WithPosition(transform.position).Play("Punch");
        yield break;
    }

    IEnumerator Shoot()
    {
        animator.SetTrigger("Shoot");
        yield break;
    }
}
