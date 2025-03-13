using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 10f; 

    public void StartSpawn()
    {
        StartCoroutine(CallSpawn());
    }

    private IEnumerator CallSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Spawn();
        }
    }

    private void Spawn()
    {
        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableIndexes.Add(i);
        }

        for (int i = 0; i < Mathf.Min(3, spawnPoints.Length); i++)
        {
            int randomIndex = Random.Range(0, availableIndexes.Count);
            int spawnPointIndex = availableIndexes[randomIndex];

            Instantiate(enemyPrefab, spawnPoints[spawnPointIndex].position, Quaternion.identity);

            availableIndexes.RemoveAt(randomIndex);
        }
    }
}
