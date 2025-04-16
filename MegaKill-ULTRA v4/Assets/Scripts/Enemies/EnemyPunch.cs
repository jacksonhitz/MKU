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
        attackRange = 6f;
        attackRate = 1f;
        dmg = 10f;
    }
    protected override void CallAttack(GameObject target)
    {
        StartCoroutine(Attack(target));
    }
      IEnumerator Attack(GameObject target)
    {
        animator.SetTrigger("Atk");
        yield return new WaitForSeconds(0.2f);

        IHit hittable = target.GetComponent<IHit>();
        if (hittable != null)
        {
            hittable.Hit(dmg);
        }

      //  soundManager.EnemySFX(sfx, attackClip);
        yield break;
    }
}