using System;
using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public static event Action<(Type enemyType, int enemiesRemaining)> EnemyKilled;

    [SerializeField]
    GameObject hands;

    [SerializeField]
    GameObject enemyHolder;
    public List<Enemy> enemies;

    void Awake()
    {
        Instance = this;
        enemies = new List<Enemy>();
    }

    void Start()
    {
        Active();
    }

    void OnEnable()
    {
        StateManager.LevelChanged += LevelChange;

        LevelChange(StateManager.Level);
    }

    void OnDisable()
    {
        StateManager.LevelChanged -= LevelChange;
    }

    void LevelChange(StateManager.GameState state)
    {
        if (StateManager.IsActive)
            Active();
        else
            return;

        // TODO: PULL THIS INTO SCENE SCRIPTS
        foreach (Enemy enemy in enemies)
        {
            if (enemy.currentState == Enemy.EnemyState.Static)
            {
                int rand = Random.Range(0, 3);
                if (rand == 0)
                    StartCoroutine(DanceTimer(enemy));
            }
        }
    }

    IEnumerator DanceTimer(Enemy enemy)
    {
        int delay = Random.Range(0, 10);
        yield return new WaitForSeconds(delay);
        enemy.isDance = true;
    }

    void Active()
    {
        enemyHolder.SetActive(true);
        CollectEnemies();
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
