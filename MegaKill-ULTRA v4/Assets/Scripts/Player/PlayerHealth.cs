using System;
using System.Collections;
using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHitable
{
    [SerializeField]
    [ProgressBar("Health", 100, EColor.Red)]
    float health;
    float maxHealth = 100;
    UEye uEye;

    [ResetOnPlay]
    public static event Action PlayerDied = delegate { };

    void Awake()
    {
        uEye = FindAnyObjectByType<UEye>();
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

        if (!StateManager.IsActive || !(health <= 0) || !enabled)
            return;
        SoundManager.Instance.CreateSoundBuilder().Play("PlayerDeath");
        PlayerDied?.Invoke();
    }

    [ConsoleMethod("Kill", "Kills the player")]
    public static void Kill()
    {
        var player = FindObjectOfType<PlayerController>();
        player.health.Hit(999);
    }
}
