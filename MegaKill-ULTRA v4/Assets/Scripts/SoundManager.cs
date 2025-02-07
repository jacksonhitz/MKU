using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum GameSpeed
    {
        Regular,
        Slow
    }

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

    private List<AudioClip> tracks;
    private int trackIndex = 0;

    public GameSpeed currentSpeed = GameSpeed.Regular;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        tracks = new List<AudioClip> { acid, witch, could, dj, all, hott, threes, life, real, four };
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

    public void EnemySFX(AudioSource source, AudioClip sound)
    {
        source.clip = sound;
        source.pitch = currentSpeed == GameSpeed.Slow ? 0.75f : 1f;
        source.Play();  
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

        music.volume = (music.clip == dj || music.clip == four) ? 0.5f : 0.1f;
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

    public void SetSpeed(GameSpeed speed)
    {
        currentSpeed = speed;

        float pitchValue = speed == GameSpeed.Slow ? 0.75f : 1f;
        music.pitch = pitchValue;
        sfx.pitch = pitchValue;
        enemySfx.pitch = pitchValue;
    }
}
