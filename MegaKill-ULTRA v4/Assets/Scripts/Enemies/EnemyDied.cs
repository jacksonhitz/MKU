using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDied : MonoBehaviour
{
    AudioSource sfx;
    [SerializeField] AudioClip deadClip;
    void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        SoundManager.Instance.EnemySFX(sfx, deadClip);
    }
}
