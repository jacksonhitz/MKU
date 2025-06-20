using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FileUI : MonoBehaviour
{
    [ResetOnPlay]
    public static FileUI Instance { get; private set; }
    public bool Visible
    {
        get => file.enabled;
        set => file.enabled = value;
    }

    [SerializeField]
    SpriteRenderer file;

    void Awake()
    {
        Instance = this;
        Visible = false;
    }
}
