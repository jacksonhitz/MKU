using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    public bool target;

    public GameObject pointer;

    void Start()
    {
        pointer.SetActive(false);
    }

    void Update()
    {
        if (target)
        {
            pointer.SetActive(true);
        }
    }
}
