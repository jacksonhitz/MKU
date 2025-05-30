using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bat : MonoBehaviour
{
    public LayerMask enemyLayer;
    PlayerController player;
    SphereCollider hitbox;
    SoundManager soundManager;
    GameManager gameManager;
    Renderer rend;
    bool isSwinging;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        hitbox = GetComponentInChildren<SphereCollider>();
        rend = GetComponentInChildren<Renderer>();
        player = FindObjectOfType<PlayerController>();
    }

    public void Use()
    {
        if (!isSwinging)
        {
            rend.enabled = false;
            StartCoroutine(Swing());
            rend.enabled = true;
        }
    }

    IEnumerator Swing()
    {
        isSwinging = true;
      //  player.SwingBat();
        yield return new WaitForSeconds(0.3f);
      //  player.combat.Melee(player.combat.batRange);
       // soundManager.BatSwing();
        yield return new WaitForSeconds(0.5f);
        isSwinging = false;
    }

    void Hit()
    {
        Collider[] colliders = Physics.OverlapSphere(hitbox.bounds.center, hitbox.radius * hitbox.transform.lossyScale.x);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.transform.parent?.parent?.GetComponent<Enemy>();
                if (enemy == null)
                {
                    enemy = collider.transform.GetComponent<Enemy>();
                }
                enemy?.Hit(100);
            }
        }
    }
}
