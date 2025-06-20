using System;
using System.Diagnostics;
using UnityEngine;

public class TripManager : MonoBehaviour
{
    [ResetOnPlay]
    public static TripManager Instance { get; private set; }

    public int trip = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        trip = 1;
    }
}
