using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject hands; 
    [SerializeField] GameObject enemyHolder;
    public List<Enemy> enemies;
    float spawnInterval = 20f; 
    PlayerController player;

    void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();

        enemies = new List<Enemy>(); 
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.TITLE: break;
            case StateManager.GameState.INTRO: break;
            case StateManager.GameState.TUTORIAL: break;
            case StateManager.GameState.FIGHT: Fight(); break;
            case StateManager.GameState.PAUSED: break;
            case StateManager.GameState.OUTRO: break;
            case StateManager.GameState.TESTING: Fight(); break;
        }
    }

    void Fight()
    {
        Debug.Log("called enemies");
        enemyHolder.SetActive(true);
        CallHands();
        CollectEnemies();
    }
    public void CollectEnemies()
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
                //enemy.StartCoroutine(enemy.StartBrawlAggression());

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
        //for (int i = 0; i < numToConvert; i++)
        //{
        //    nonDosedEnemies[i].currentState = Enemy.EnemyState.Brawl;
        //}
    }


    public void Kill(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void CallHands()
    {
        //StartCoroutine(SpawnHands());
    }

    IEnumerator SpawnHands()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

           // if (!player.rooted)
            {
                //ui.PopUp("LOOK DOWN");
                
              //  player.rooted = true;
                Vector3 spawnPos = player.transform.position;
                spawnPos.y -= 1f;
                Instantiate(hands, spawnPos, Quaternion.identity);
            }
        }
    }
}