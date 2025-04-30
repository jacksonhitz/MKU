using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Russia : MonoBehaviour
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
            gameManager.Testing();
            gameManager.CollectItems();
        }
    }
}
