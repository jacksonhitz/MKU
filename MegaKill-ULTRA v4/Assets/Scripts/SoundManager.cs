using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource sfx;
    public AudioSource enemySfx;

    public AudioClip title;
    public AudioClip witch;
    public AudioClip hott;
    public AudioClip threes;
    public AudioClip life;
    public AudioClip real;
    public AudioClip could;
    public AudioClip dj;
    public AudioClip four;
    public AudioClip all;

    public AudioClip gunShot;
    public AudioClip reload;
    public AudioClip empty;
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
        tracks = new List<AudioClip> { witch, dj, could, all, hott, threes, life, real, four, };
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
            music.volume = .05f;
        }
        else
        {
            music.volume = .01f;
        }
    }

    public void Squelch()
    {
        enemySfx.clip = squelch;
        enemySfx.Play();
    }

    public void Gunshot() => PlaySfx(gunShot);
    
    public void Reload() => PlaySfx(reload);
    public void Empty() => PlaySfx(empty);
    public void Heartbeat() => PlaySfx(heartbeat);
    public void Flatline() => PlaySfx(flatline);

    private void PlaySfx(AudioClip clip)
    {
        sfx.clip = clip;
        sfx.Play();
    }
}
