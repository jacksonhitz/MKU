using System.Collections;
using UnityEngine;

public class EnemyDog : Enemy
{
    protected override void DropItem(){}
    protected override void Start()
    {
        attackRange = 2f;
        attackRate = 3f;
        dmg = 10f;
    }
    protected override void CallAttack()
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);

        player.Hit(dmg);
        soundManager.EnemySFX(sfx, attackClip);

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
}