using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        if (gameManager != null)
        {
            gameManager.Title();
            gameManager.CollectItems();
        }
    }
}
