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
        }
    }

    IEnumerator Swing()
    {
        isSwinging = true;
        player.swingAnim.SetTrigger("Swing");
        yield return new WaitForSeconds(0.3f);
        Hit();
        soundManager.BatSwing();
        yield return new WaitForSeconds(0.5f);
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
            }
        }
    }
}
