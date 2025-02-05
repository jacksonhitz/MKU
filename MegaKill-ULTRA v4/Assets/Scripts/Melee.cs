using System.Collections;
using UnityEngine;

public class Melee : MonoBehaviour
{
    float fireRate = 1f;
    float range = 2f;
    Enemy enemy;
    GameObject player;
    PlayerController playerController;
    bool isAttacking = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (enemy.los && InRange() && !isAttacking)
        {
            StartCoroutine(CallAttack());
        }
    }

    bool InRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    IEnumerator CallAttack()
    {
        isAttacking = true;
        Attack();
        yield return new WaitForSeconds(fireRate);
        isAttacking = false;
    }

    void Attack()
    {
        playerController.Hit();
    }
}