using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDied : MonoBehaviour
{
    AudioSource sfx;
    SoundManager soundManager;
    [SerializeField] AudioClip deadClip;
    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        soundManager.EnemySFX(sfx, deadClip);
    }
}
