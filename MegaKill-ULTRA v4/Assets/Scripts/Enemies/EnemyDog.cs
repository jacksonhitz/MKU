using System.Collections;
using UnityEngine;

public class EnemyDog : Enemy
{
    bool isAttacking;

    protected override void DropItem(){}
    protected override void Start()
    {
        attackRange = 2f;
    }
    protected override IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);

        player.Hit();
        animator.SetTrigger("Attack");
        soundManager.EnemySFX(sfx, attackClip);

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
}