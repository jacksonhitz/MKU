using System.Collections;
using UnityEngine;

public class EnemyBat : Enemy
{
    
    Item item;
    protected override void DropItem()
    {
        item.Dropped();
    }
    protected override void Start()
    {
        attackRange = 5f;
        attackRate = 5f;
        dmg = 10f;
    }
    protected override void CallAttack()
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        player.Hit(dmg);
        soundManager.EnemySFX(sfx, attackClip);

        yield break;
    }
}