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

    public AudioClip gunShot;
    public AudioClip reload;
    public AudioClip empty;
    public AudioClip heartbeat;
    public AudioClip flatline;

    public AudioClip squelch;

    public GameManager gameManager;
    public bool controller;

    private List<AudioClip> musicTracks;
    private int currentTrackIndex = 0;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        musicTracks = new List<AudioClip> { witch, hott, threes, life, real, could };
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
        if (controller && !music.isPlaying)
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
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;
        music.clip = musicTracks[currentTrackIndex];
        music.Play();
    }

    public void Squelch()
    {
        enemySfx.clip = squelch;
        enemySfx.Play();
    }

    public void Hott() => PlayTrack(hott);
    public void Witch() => PlayTrack(witch);
    public void Threes() => PlayTrack(threes);
    public void Real() => PlayTrack(real);
    public void Life() => PlayTrack(life);

    private void PlayTrack(AudioClip track)
    {
        music.clip = track;
        music.Play();
    }

    public void Gunshot()
    {
        sfx.clip = gunShot;
        sfx.Play();
    }
    
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
