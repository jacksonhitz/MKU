using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyFlyingEye : Enemy
{
    [Header("Refrences")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    Item item;
    Rigidbody rb;

    Vector3 rot = Vector3.zero;
    float moveSpeed = 5f;

    float bulletSpd = 10f;
    float targetAdjust = 0.25f;
    protected override void DropItem(){}

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        attackRange = 15f;
        attackRate = 5f;
        dmg = 20f;
    }

     protected override void Hit(bool lethal)
    {
        soundManager.EnemySFX(sfx, deadClip);
        StartCoroutine(Dead());
    }

    protected override void CallAttack()
    {
        StartCoroutine(Attack());
    }

    protected override void Pathfind()
    {
        if (los && Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            StartCoroutine(AttackCheck());
            LookTowards(player.transform.position);
        }
        else if (detectedPlayer && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            LookTowards(player.transform.position);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    IEnumerator Attack()
    {
        //animator.SetTrigger("Atk");

        Vector3 targetPos = player.transform.position;
        targetPos.y = player.transform.position.y + targetAdjust;

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