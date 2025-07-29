using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using KBCore.Refs;
using UnityEngine;
using UnityUtils;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using SceneState = StateManager.SceneState;

public class EnemyManager : ValidatedMonoBehaviour
{
    [ResetOnPlay]
    public static EnemyManager Instance { get; private set; }

    [ResetOnPlay("delegate { }")]
    public static event Action<(Type enemyType, int enemiesRemaining)> EnemyKilled = delegate { };

    [SerializeField]
    GameObject hands;

    [SerializeField]
    GameObject enemyHolder;
    public List<Enemy> enemies = new();

    [SerializeField, Anywhere(Flag.Optional)]
    private GameObject spawnerHolder;

    public bool EnemySpawning
    {
        get => spawnerHolder.activeInHierarchy;
        set => spawnerHolder.SetActive(value);
    }

    private bool Active
    {
        get => enemyHolder.activeInHierarchy;
        set
        {
            enemyHolder.SetActive(value);
            if (enemyHolder.activeInHierarchy)
                CollectEnemies();
        }
    }

    void Awake()
    {
        Instance = this;
        Active = false;
        EnemySpawning = false;
    }

    void OnEnable()
    {
        SceneScript.StateChanged += LevelChange;
    }

    void OnDisable()
    {
        SceneScript.StateChanged -= LevelChange;
    }

    void LevelChange(SceneState sceneState)
    {
        Active = StateManager.IsActive;
    }

    void CollectEnemies()
    {
        enemies.Clear();
        enemies.AddRange(FindObjectsOfType<Enemy>());
    }

    public void Brawl()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.dosed)
            {
                enemy.currentState = Enemy.EnemyState.Brawl;
            }
        }

        List<Enemy> nonDosedEnemies = new List<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.dosed)
            {
                nonDosedEnemies.Add(enemy);
            }
        }

        for (int i = 0; i < nonDosedEnemies.Count; i++)
        {
            Enemy temp = nonDosedEnemies[i];
            int randomIndex = Random.Range(i, nonDosedEnemies.Count);
            nonDosedEnemies[i] = nonDosedEnemies[randomIndex];
            nonDosedEnemies[randomIndex] = temp;
        }
    }

    public void Kill(Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.gameObject);
        EnemyKilled?.Invoke((enemy.GetType(), enemies.Count));

        SoundManager
            .Instance.CreateSoundBuilder()
            .WithPosition(enemy.transform.position)
            .Play("EnemyDeath");
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
        enemy.transform.SetParent(spawnerHolder.transform);
    }

    [ConsoleMethod("KillAllEnemies", "Kills all enemies in the scene.")]
    public static void KillAllEnemiesCommand()
    {
        if (!Instance)
        {
            Debug.LogError("No enemy manager in scene.");
            return;
        }

        int count = Instance.enemies.Count;
        while (Instance.enemies.Count > 0)
        {
            Instance.Kill(Instance.enemies[0]);
        }
        Debug.Log($"Killed {count - Instance.enemies.Count} enemies.");
    }
}
