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
    UX ux;
    GameManager gameManager;
    SceneLoader sceneLoader;

    public bool on;


    void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
        ux = FindObjectOfType<UX>();
        gameManager = FindAnyObjectByType<GameManager>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        enemies = new List<Enemy>(); 
    }
    void Update()
    {
        if (enemyHolder.transform.childCount == 0)
        {
            sceneLoader.Win();
        }
    }

    public void Active()
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
                ux.PopUp("LOOK DOWN");
                
                player.rooted = true;
                Vector3 spawnPos = player.transform.position;
                spawnPos.y -= 1f;
                Instantiate(hands, spawnPos, Quaternion.identity);
            }
        }
    }
}