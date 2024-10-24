using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameManager gameManager;

    public Civilian civilian;
    public bool cop;

    public GameObject blood;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (!cop)
        {
            civilian = GetComponent<Civilian>();
        }
    }

    public void Hit()
    {
        Instantiate(blood, transform.position, Quaternion.identity);

        if (cop)
        {
            gameManager.Score(100);
        }
        else
        {
            if (civilian.target)
            {
                gameManager.Score(200);
                gameManager.UpPhase();
            }
            gameManager.Score(50);
        } 

        Destroy(this.gameObject);
    }
}
