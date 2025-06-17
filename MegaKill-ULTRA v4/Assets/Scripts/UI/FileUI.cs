using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileUI : MonoBehaviour
{
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
