using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    GameObject obj;
    void Awake()
    {
        obj = transform.GetChild(0).gameObject;
    }
    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        if (state != StateManager.GameState.Title)
        {
            On();
        }
        else
        {
            Off();
        }
    }
    void On()
    {
        obj.SetActive(true);
    }
    void Off()
    {
        obj.SetActive(false);
    }
}
