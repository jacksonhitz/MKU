using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    UX ux;
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        ux = FindObjectOfType<UX>(); 
    }
    void OnTriggerEnter(Collider collider)
    {
        ux.Warning();
        
    }
}
