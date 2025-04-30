using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    void Start()
    {
        StateManager.LoadState(StateManager.GameState.TITLE);
    }
}
