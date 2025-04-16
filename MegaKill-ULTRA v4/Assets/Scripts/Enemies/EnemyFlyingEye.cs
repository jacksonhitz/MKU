using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyFlyingEye : Enemy
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    Item item;
    Rigidbody rb;

    Vector3 rot = Vector3.zero;
    float moveSpeed = 5f;
    float hoverHeight = 2f;
    float hoverSmoothness = 8f;
    float bulletSpd = 10f;
    float targetAdjust = 0.25f;
    
    protected override void DropItem() {}

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        attackRange = 25f;
        attackRate = 5f;
        dmg = 20f;
    }

    protected override void Hit(bool lethal)
    {
        Dead();
    }

    protected override void CallAttack(GameObject target)
    {
        StartCoroutine(Attack());
    }

    void Pathfind()
    {
        if (los && Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
             //startCoroutine(AttackCheck());
            LookTowards(player.transform.position);
        }
        else if (detectedPlayer && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            Vector3 targetPosition = player.transform.position + Vector3.up * hoverHeight;
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = new Vector3(direction.x * moveSpeed, Mathf.Lerp(rb.velocity.y, direction.y * moveSpeed, Time.deltaTime * hoverSmoothness), direction.z * moveSpeed);
            LookTowards(player.transform.position);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("Atk");
        soundManager.EnemySFX(sfx, attackClip);

        Vector3 targetPos = player.transform.position;
        targetPos.y += targetAdjust;

        Vector3 targetDir = (targetPos - firePoint.position).normalized;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        Rigidbody rb = bulletObj.GetComponent<Rigidbody>();

        rb.velocity = targetDir * bulletSpd;

        bullet.vel = bulletSpd;
        bullet.dir = targetDir;
        bullet.dmg = dmg;

        yield break;
    }
}
