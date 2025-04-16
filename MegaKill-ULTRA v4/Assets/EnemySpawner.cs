using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public int enemyCount = 500;
    public float spawnRadius = 100f;
    public float minDistanceBetweenEnemies = 2f;
    public int maxSampleAttemptsPerEnemy = 10;

    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        int spawned = 0;

        while (spawned < enemyCount)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPoint.y = transform.position.y;

            // Check if point is on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
            {
                // Check distance from other spawn points
                bool tooClose = false;
                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(pos, hit.position) < minDistanceBetweenEnemies)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                // Instantiate enemy
                Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                spawnPositions.Add(hit.position);
                spawned++;
            }
        }
    }
}

