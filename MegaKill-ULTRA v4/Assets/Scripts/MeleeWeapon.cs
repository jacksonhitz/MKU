using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    public float damage = 25f;
    public float swingRange = 2f;
    public float swingAngle = 60f;
    public float swingSpeed = 10f;

    public Transform weapon;
    public LayerMask enemyLayer;
    public Animator animator;

    bool isSwinging;
    float initialRotation;

    void Start()
    {
        initialRotation = this.transform.localEulerAngles.x;
    }

    public void Attack()
    {
        if (!isSwinging)
        {
            animator.SetBool("BatSwing", true);
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        isSwinging = true;

        yield return new WaitForSeconds(1f);
        animator.SetBool("BatSwing", false);
        DealDamage();
        isSwinging = false;
    }

    void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(this.transform.position, swingRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

            if (angleToEnemy <= swingAngle / 2f)
            {
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, swingRange);
    }
}
