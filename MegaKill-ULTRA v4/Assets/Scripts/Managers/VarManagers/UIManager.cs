using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [ResetOnPlay]
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
}
