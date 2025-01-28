using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;    
    public GameObject copPrefab;
    public int npcCount = 250;
    public int copCount = 10;       
    public float minSpawnRadius = 5f;  
    public float spawnAreaRadius = 50f;  

    private List<Vector3> spawnedPositions = new List<Vector3>();
    public GameManager gameManager;

    void Start()
    {
        SpawnNPCs();
        SpawnCops(); // Call to spawn cops
    }

    void SpawnNPCs()
    {
        int spawnedNPCs = 0;

        while (spawnedNPCs < npcCount)
        {
            Vector3 randomPosition = GetRandomNavMeshPosition();
            
            if (IsPositionValid(randomPosition) && randomPosition != Vector3.zero)
            {
                Instantiate(npcPrefab, randomPosition, Quaternion.identity);
                spawnedPositions.Add(randomPosition);
                spawnedNPCs++;
            }
        }

        //gameManager.ChooseTarget();
    }

    void SpawnCops()
    {
        int spawnedCops = 0;

        while (spawnedCops < copCount)
        {
            Vector3 randomPosition = GetRandomNavMeshPosition();
            
            // Check if the position is valid and not the fallback Vector3.zero
            if (IsPositionValid(randomPosition) && randomPosition != Vector3.zero)
            {
                // Spawn the cop at the valid position
                Instantiate(copPrefab, randomPosition, Quaternion.identity);
                spawnedPositions.Add(randomPosition); // Keep track of the position
                spawnedCops++;
            }
        }
    }

    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnAreaRadius;
        randomDirection += transform.position;  // Adjust for spawner's position

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnAreaRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero; // Return zero if no valid position is found (fallback)
    }

    bool IsPositionValid(Vector3 newPosition)
    {
        // Check if the new position is far enough from all other spawned NPCs and cops
        foreach (Vector3 pos in spawnedPositions)
        {
            if (Vector3.Distance(pos, newPosition) < minSpawnRadius)
            {
                return false;
            }
        }
        return true;
    }
}
