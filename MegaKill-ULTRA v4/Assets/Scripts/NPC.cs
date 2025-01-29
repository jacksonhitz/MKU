using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject blood;
    AudioSource sfx;
    public AudioClip squelch;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        sfx = GetComponent<AudioSource>();
    }

    public void Hit()
    {
        Instantiate(blood, transform.position, Quaternion.identity);

        //gameManager.RemoveEnemy(this);

        this.gameObject.SetActive(false);
        gameManager.Score(100);
    }
}
