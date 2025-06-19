using System;
using System.Collections.Generic;
using System.Diagnostics;
using IngameDebugConsole;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using SceneState = StateManager.SceneState;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public static event Action<(Type enemyType, int enemiesRemaining)> EnemyKilled = delegate { };

    [SerializeField]
    GameObject hands;

    [SerializeField]
    GameObject enemyHolder;
    public List<Enemy> enemies = new();

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

        SoundManager.Instance.Play("EnemyDeath", enemy.transform.position);
    }

    [Conditional("UNITY_EDITOR")]
    private void OnApplicationQuit()
    {
        EnemyKilled = null;
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
