using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rehearsal : MonoBehaviour
{
    InteractionManager interacts;

    void Awake()
    {
        interacts = FindObjectOfType<InteractionManager>();
    }
    void Start()
    {
        interacts?.Collect();
        StateManager.LoadState(StateManager.GameState.REHEARSAL);
    }
}
