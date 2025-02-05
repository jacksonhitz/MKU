using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeapon : MonoBehaviour
{
    public Transform weapon;
    public LayerMask enemyLayer;
    public Animator animator;
    public PlayerController player;
    public SphereCollider hitbox;
    SoundManager soundManager;
    GameManager gameManager;
    bool isSwinging;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Attack()
    {
        if (!isSwinging)
        {
            animator.SetBool("Swing", true);
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        isSwinging = true;
        yield return new WaitForSeconds(0.3f);
        Hit();
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Swing", false);
        isSwinging = false;
    }

    void Hit()
    {

        Collider[] colliders = Physics.OverlapSphere(hitbox.bounds.center, hitbox.radius * hitbox.transform.lossyScale.x);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                Enemy enemy = collider.transform.parent?.parent?.GetComponent<Enemy>();
                if (enemy == null)
                {
                    enemy = collider.transform.GetComponent<Enemy>();
                }
                enemy?.Hit();
                soundManager.Squelch();
                gameManager.phase++;
            }
        }
    }
}
