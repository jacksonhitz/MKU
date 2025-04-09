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

    public bool on;

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
            case StateManager.GameState.Title:
                
                break;
            case StateManager.GameState.Intro:
                
                break;
            case StateManager.GameState.Tutorial:
                
                break;
            case StateManager.GameState.Lvl:
                Lvl();
                break;
            case StateManager.GameState.Outro:

                break;
        }
    }

    void Lvl()
    {
        enemyHolder.SetActive(true);
        CallHands();
        CollectEnemies();
    }
    public void CollectEnemies()
    {
        enemies.Clear(); 
        enemies.AddRange(FindObjectsOfType<Enemy>()); 
    }

    public void EnemyDead(Enemy enemy)
    {
        enemies.Remove(enemy);
        
    }

    public void CallHands()
    {
        StartCoroutine(SpawnHands());
    }

    IEnumerator SpawnHands()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!player.rooted)
            {
                //ui.PopUp("LOOK DOWN");
                
                player.rooted = true;
                Vector3 spawnPos = player.transform.position;
                spawnPos.y -= 1f;
                Instantiate(hands, spawnPos, Quaternion.identity);
            }
        }
    }
}