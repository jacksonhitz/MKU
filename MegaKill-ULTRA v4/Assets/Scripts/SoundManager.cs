using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource sfx;

    public AudioClip title;
    public AudioClip witch;
    public AudioClip hott;
    public AudioClip threes;
    public AudioClip life;
    public AudioClip real;

    public AudioClip gunShot;
    public AudioClip reload;
    public AudioClip empty;
    public AudioClip heartbeat;
    public AudioClip flatline;

    public GameManager gameManager;

    public bool controller;

    void Awake()
    {
        if (controller)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (controller)
        {
            music.clip = title;
            music.Play();
        }
    }

    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    public void PhaseCheck()
    {
        music.Stop();
        
        if (gameManager.phase == 1)
        {
            Hott();
        }
        if (gameManager.phase == 2)
        {
            Threes();
        }
        if (gameManager.phase == 3)
        {
            Real();
        }
        if (gameManager.phase == 4)
        {
            Life();
        }
        if (gameManager.phase == 5)
        {
            Witch();
        }
    }

    public void Hott()
    {
        music.clip = hott;
        music.Play();
    }
    
    public void Witch()
    {
        music.clip = hott;
        music.Play();
    }
    public void Threes()
    {
        music.clip = hott;
        music.Play();
    }
    public void Real()
    {
        music.clip = real;
        music.Play();
    }
    public void Life()
    {
        music.clip = hott;
        music.Play();
    }

    public void Gunshot()
    {
        sfx.clip = gunShot;
        sfx.Play();
    }
    public void Reload()
    {
        sfx.clip = reload;
        sfx.Play();
    }
    public void Empty()
    {
        sfx.clip = empty;
        sfx.Play();
    }

    public void Heartbeat()
    {
        sfx.clip = heartbeat;
        sfx.Play();
    }
    public void Flatline()
    {
        sfx.clip = flatline;
        sfx.Play();
    }


}
