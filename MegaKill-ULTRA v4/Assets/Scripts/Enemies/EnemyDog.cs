using System.Collections;
using UnityEngine;

public class EnemyDog : Enemy
{
    protected override void DropItem() { }

    protected void Start()
    {
        attackRange = 2f;
        attackRate = 2f;
        damage = 10f;
    }

    protected override void CallAttack()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        // SoundManager.Instance.EnemySFX(sfx, attackClip);
        player.health.Hit(damage);
        // SoundManager.Instance.EnemySFX(sfx, attackClip);

        yield break;
    }
}
