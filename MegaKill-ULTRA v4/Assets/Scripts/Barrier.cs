using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the invisible barrier!");
        }
    }
}
