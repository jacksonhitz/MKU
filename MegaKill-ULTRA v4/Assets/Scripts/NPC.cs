using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameManager gameManager;

    public Civilian civilian;
    public bool cop;

    public GameObject blood;
    AudioSource sfx;
    public AudioClip squelch;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    sfx = GetComponent<AudioSource>();

        if (!cop)
        {
            civilian = GetComponent<Civilian>();
        }
    }

    public void Hit()
    {
        Instantiate(blood, transform.position, Quaternion.identity);

        this.gameObject.SetActive(false);

        if (cop)
        {
            gameManager.Score(100);
        }
        else
        {
            if (civilian.target)
            {
                gameManager.Score(1000);
                gameManager.ChooseTarget();
            }
            gameManager.Score(50);
        } 
    }
}
