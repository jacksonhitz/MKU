using System.Collections;
using UnityEngine;

public class EnemyPunch : Enemy
{
    
    Item item;
    protected override void DropItem()
    {
        if (item != null)
        {
            item.Dropped();
        }
    }
    protected override void Start()
    {
        attackRange = 3f;
        attackRate = 1f;
        dmg = 5f;
    }
    protected override void CallAttack(GameObject target)
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        yield return new WaitForSeconds(0.2f);
        target.Hit(dmg)
        soundManager.EnemySFX(sfx, attackClip);

        yield break;
    }
}