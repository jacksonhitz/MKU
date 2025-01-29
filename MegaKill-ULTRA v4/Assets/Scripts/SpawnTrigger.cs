using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public Spawner spawner;
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    void OnTriggerEnter(Collider collider)
    {
        spawner.StartSpawn();
        
    }
}
