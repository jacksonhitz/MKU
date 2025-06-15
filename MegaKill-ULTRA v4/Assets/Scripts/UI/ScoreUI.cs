using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; set; }
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
