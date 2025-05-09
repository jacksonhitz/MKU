using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearhead : MonoBehaviour
{
    ItemManager itemManager;

    void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
    }

    void Start()
    {
        StateManager.LoadState(StateManager.GameState.SPEARHEAD);

        itemManager?.CollectItems();
    }
}
