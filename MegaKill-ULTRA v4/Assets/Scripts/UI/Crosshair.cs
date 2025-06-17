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
        SceneScript.StateChanged += StateChange;
    }

    void OnDisable()
    {
        SceneScript.StateChanged -= StateChange;
    }

    void StateChange(StateManager.SceneState state)
    {
        if (StateManager.IsActive)
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
