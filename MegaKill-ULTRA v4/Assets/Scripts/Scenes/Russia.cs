using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Russia : MonoBehaviour
{
    ItemManager itemManager;

    void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
    }

    void Start()
    {
        StateManager.Testing();

        if (itemManager != null) itemManager.CollectItems();
    }
}
