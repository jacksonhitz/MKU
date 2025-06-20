using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [ResetOnPlay]
    public static ScoreUI Instance { get; private set; }
    public bool Visible
    {
        get => image.enabled;
        set => image.enabled = value;
    }

    [SerializeField]
    private SpriteRenderer image;

    private void Awake()
    {
        Instance = this;
        Visible = false;
    }
}
