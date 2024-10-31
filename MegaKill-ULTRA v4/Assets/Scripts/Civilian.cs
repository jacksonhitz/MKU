using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    public bool target;

    public GameObject pointer;
    AudioSource sfx;

    public AudioClip scream1;
    public AudioClip scream2;
    public AudioClip scream3;

    AudioClip[] screams;

    void Start()
    {
        pointer.SetActive(false);
        sfx = GetComponent<AudioSource>();

        screams = new AudioClip[] { scream1, scream2, scream3 };
    }

    public void Scared()
    {
        sfx.clip = screams[Random.Range(0, screams.Length)];
        sfx.Play();
        Debug.Log("playedScream");
    }

    void Update()
    {
        if (target)
        {
            pointer.SetActive(true);
        }
    }
}
