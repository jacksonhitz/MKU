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
        StateManager.LoadState(StateManager.GameState.SABLE);

        if (itemManager != null) itemManager.CollectItems();
    }
}
