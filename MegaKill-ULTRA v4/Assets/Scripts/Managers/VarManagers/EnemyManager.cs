using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] GameObject hands;
    [SerializeField] GameObject enemyHolder;
    public List<Enemy> enemies;
    float spawnInterval = 5f;
    PlayerController player;

    void Awake()
    {
        Instance = this;

        player = FindObjectOfType<PlayerController>();
        enemies = new List<Enemy>();
    }
    void Start()
    {
        Active();
    }



    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }

    //FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU
    List<Enemy> staticEnemies = new List<Enemy>();
    void StateChange(StateManager.GameState state)
    {
        if (StateManager.IsActive())
            Active();
        else return;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.currentState == Enemy.EnemyState.Static)
            {
                staticEnemies.Add(enemy);
            }
        }

        // Shuffle the list (Fisher-Yates shuffle)
        for (int i = staticEnemies.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (staticEnemies[i], staticEnemies[j]) = (staticEnemies[j], staticEnemies[i]);
        }

        // Set isDance = true for half of them
        int halfCount = staticEnemies.Count / 4;
        for (int i = 0; i < halfCount; i++)
        {
            staticEnemies[i].isDance = true;
        }
    }

    void Active()
    {
        enemyHolder.SetActive(true);
        // CallHands();
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

        int numToConvert = Mathf.CeilToInt(nonDosedEnemies.Count * 0.5f);

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

        if (enemies.Count == 0)
            ScenesManager.Instance.ElimCheck();
    }

    public void CallHands()
    {
        // StartCoroutine(SpawnHands());
    }

    IEnumerator SpawnHands()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector3 spawnPos = player.transform.position;
            spawnPos.y -= 4f;
            Instantiate(hands, spawnPos, Quaternion.identity);
        }
    }
}