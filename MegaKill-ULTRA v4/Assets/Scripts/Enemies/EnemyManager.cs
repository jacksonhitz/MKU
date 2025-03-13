using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject hands; 
    [SerializeField] GameObject enemyHolder;
    public List<Enemy> enemies;
    float spawnInterval = 30f; 
    PlayerController player;
    UX ux;
    GameManager gameManager;

    public bool on;


    void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
        ux = FindObjectOfType<UX>();
        gameManager = FindAnyObjectByType<GameManager>();
        enemies = new List<Enemy>(); 
    }

    public void Active()
    {
        enemyHolder.SetActive(true);
        //CallHands();
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
        if (enemies.Count == 0)
        {
            gameManager.Exit();
        }
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
                ux.PopUp("LOOK DOWN");
                
                player.rooted = true;
                Vector3 spawnPos = player.transform.position;
                spawnPos.y -= 1f;
                Instantiate(hands, spawnPos, Quaternion.identity);
            }
        }
    }
}