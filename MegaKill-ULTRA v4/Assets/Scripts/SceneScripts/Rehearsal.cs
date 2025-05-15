using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rehearsal : MonoBehaviour
{
    ItemManager itemManager;
    Dialogue file;

    void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
    }

    void Start()
    {
        StateManager.LoadState(StateManager.GameState.REHEARSAL);

        itemManager?.CollectItems();
    }
}
