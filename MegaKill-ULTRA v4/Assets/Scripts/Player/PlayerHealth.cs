using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHitable
{
    [SerializeField] float health;
    float maxHealth = 100;
    UEye uEye;
    bool isDead;

    void Awake()
    {
        uEye = FindObjectOfType<UEye>();
    }

    void Start()
    {
        health = maxHealth;
    }

    public void Heal(float heal)
    {
        health = Mathf.Min(health + heal, maxHealth);
        uEye.UpdateHealth(health);
    }

    public void Hit(float dmg)
    {
        health -= dmg;
        uEye.UpdateHealth(health);

        if (!isDead && health <= 0)
        {
            isDead = true;
            StartCoroutine(StateManager.LoadState(StateManager.PREVIOUS, 3f));
            SoundManager.Instance.Play("PlayerDeath");
        }
    }
}
