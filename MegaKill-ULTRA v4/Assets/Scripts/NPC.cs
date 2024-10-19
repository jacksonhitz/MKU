using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    bool target;

    public GameObject pointer;
    public GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        
        pointer.SetActive(false);
    }

    public void Target()
    {
        target = true;
        pointer.SetActive(true);
    }

    public void Hit()
    {
        Destroy(this.gameObject);
        if (target)
        {
            gameManager.ChooseTarget();
        }
    }
}
