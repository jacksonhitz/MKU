using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource sfx;
    public AudioSource enemySfx;

    public AudioClip title;
    public AudioClip acid;
    public AudioClip witch;
    public AudioClip hott;
    public AudioClip threes;
    public AudioClip life;
    public AudioClip real;
    public AudioClip could;
    public AudioClip dj;
    public AudioClip four;
    public AudioClip all;

    public AudioClip revShot;
    public AudioClip revReload;
    public AudioClip revEmpty;

    public AudioClip shotShot;
    public AudioClip shotReload;
    public AudioClip shotEmpty;


    public AudioClip heartbeat;
    public AudioClip flatline;

    public AudioClip squelch;

    public GameManager gameManager;
    public bool controller;

    List<AudioClip> tracks;
    int trackIndex = 0;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        tracks = new List<AudioClip> { acid, witch, could, dj, all, hott, threes, life, real, four, };
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
        if (!music.isPlaying)
        {
            NewTrack();
        }
    }

    public void Stop()
    {
        music.Stop();
    }

    public void NewTrack()
    {
        trackIndex = (trackIndex + 1) % tracks.Count;
        music.clip = tracks[trackIndex];
        music.Play();

        if (music.clip == dj || music.clip == four)
        {
            music.volume = .5f;
        }
        else
        {
            music.volume = .1f;
        }
    }

    public void Squelch()
    {
        enemySfx.clip = squelch;
        enemySfx.Play();
    }
    public void RevShot() => PlaySfx(revShot);
    public void RevReload() => PlaySfx(revReload);
    public void RevEmpty() => PlaySfx(revEmpty);
    
    public void ShotShot() => PlaySfx(shotShot);
    public void ShotReload() => PlaySfx(shotReload);
    public void ShotEmpty() => PlaySfx(shotEmpty);
    public void Heartbeat() => PlaySfx(heartbeat);
    public void Flatline() => PlaySfx(flatline);

    void PlaySfx(AudioClip clip)
    {
        sfx.clip = clip;
        sfx.Play();
    }

    public void Slow()
    {
        music.pitch = 0.75f;
        sfx.pitch = 0.75f;
        enemySfx.pitch = 0.75f;
    }
    public void Reg()
    {
        music.pitch = 1;
        sfx.pitch = 1;
        enemySfx.pitch = 1;
    }
}
