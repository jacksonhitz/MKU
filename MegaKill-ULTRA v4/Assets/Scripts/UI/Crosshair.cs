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
         OnStateChanged(StateManager.State);
    }
    void OnEnable()
    {
        StateManager.OnStateChanged += OnStateChanged;
        StateManager.OnSilentChanged += OnStateChanged;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= OnStateChanged;
        StateManager.OnSilentChanged -= OnStateChanged;
    }
    void OnStateChanged(StateManager.GameState state)
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
