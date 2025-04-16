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

    void Start()
    {
        Off();
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
        if (StateManager.IsActive())
        {
            On();
        }
        else
        {
            Off();
        }
    }
    public void On()
    {
        obj.SetActive(true);
    }
    void Off()
    {
        obj.SetActive(false);
    }
}
