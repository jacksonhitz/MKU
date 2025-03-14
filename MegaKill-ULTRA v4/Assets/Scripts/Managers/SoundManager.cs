using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    public enum GameSpeed
    {
        Regular,
        Slow
    }
    
    public AudioSource music;
    public AudioSource sfx;
    public AudioSource playerSfx;
    public AudioSource dialogue;

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

    public AudioClip line0;
    public AudioClip line1;
    public AudioClip line2;
    public AudioClip line3;
    public AudioClip line4;
    public AudioClip line5;
    public AudioClip line6;
    public AudioClip line7;

    public AudioClip revShot;
    public AudioClip revEmpty;
    public AudioClip shotShot;
    public AudioClip shotEmpty;
    public AudioClip mgShot;
    public AudioClip mgEmpty;
    public AudioClip punch;
    public AudioClip toss;
    public AudioClip tossHit;
    public AudioClip playerHit;
    public AudioClip heartbeat;
    public AudioClip playerDeath;
    public AudioClip enemyDeath;
    public AudioClip enemyHit;
    public AudioClip enemyStun;
    public AudioClip gulp;
    public AudioClip pillEmpty;
    public AudioClip batSwing;

    Settings settings;

    List<AudioClip> tracks;
    List<AudioClip> lines;
    
    int trackIndex = -1;
    int lineIndex = 0;
    
    public GameSpeed currentSpeed = GameSpeed.Regular;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        settings = FindObjectOfType<Settings>();
        tracks = new List<AudioClip> { title, witch, acid, could, dj, all, hott, threes, life, real, four };
        lines = new List<AudioClip> { line1, line2, line3, line4, line5, line6, line7 };
    }

    void Start()
    {
        NewTrack(); 
    }

    void Update()
    {
        UpdateVolume();
    }
    public void ResetMusic()
    {
        trackIndex = -1;
        NewTrack();
    }

    public void Pause()
    {
        music.Pause();
        sfx.Pause();
        dialogue.Pause();
    }

    public void Play()
    {
        music.Play();
        if (StateManager.state == StateManager.GameState.Intro)
        {
            dialogue.Play();
        }
    }

    public void UpdateVolume()
    {
        float normalizedMusic = settings.musicVolume / 100f;
        float normalizedSfx = settings.sfxVolume / 100f;
        music.volume = normalizedMusic / 3;
        sfx.volume = normalizedSfx;
        dialogue.volume = normalizedSfx;
    }

    public void EnemySFX(AudioSource source, AudioClip sound)
    {
        float normalizedSfx = settings.sfxVolume / 100f;
        source.volume = normalizedSfx;
        
        source.clip = sound;
        source.pitch = currentSpeed == GameSpeed.Slow ? 0.75f : 1f;
        source.Play();  
    }

    public void StopMusic() => music.Stop();
    public void StopLine() => dialogue.Stop();

    public void NewLine()
    {
        lineIndex = (lineIndex + 1) % lines.Count;
        dialogue.clip = lines[lineIndex];
        dialogue.Play();
    }

    public void NewTrack()
    {
        if (tracks.Count > 1)
        {
            trackIndex = (trackIndex + 1) % tracks.Count;
            music.clip = tracks[trackIndex];
            music.Play();
            InvokeNewTrack(); 
        }
    }

    void InvokeNewTrack()
    {
        float trackLength = music.clip.length;
        Invoke(nameof(NewTrack), trackLength);
    }

    public void RevShot() => PlaySfx(revShot);
    public void RevEmpty() => PlaySfx(revEmpty);
    public void ShotShot() => PlaySfx(shotShot);
    public void ShotEmpty() => PlaySfx(shotEmpty);
    public void MGShot() => PlaySfx(mgShot);
    public void MGEmpty() => PlaySfx(mgEmpty);
    public void BatSwing() => PlaySfx(batSwing);
    public void Punch() => PlaySfx(punch);
    public void Toss() => PlaySfx(toss);
    public void TossHit() => PlaySfx(tossHit);
    public void Heartbeat() => PlaySfx(heartbeat);
    public void PlayerHit() => PlaySfx(playerHit);
    public void PlayerDeath() => PlayerSfx(playerDeath);
    public void Gulp() => PlaySfx(gulp);
    public void PillEmpty() => PlaySfx(pillEmpty);

    void PlaySfx(AudioClip clip)
    {
        sfx.clip = clip;
        sfx.Play();
    }

    void PlayerSfx(AudioClip clip)
    {
        playerSfx.clip = clip;
        playerSfx.Play();
    }

    public void SetSpeed(GameSpeed speed)
    {
        currentSpeed = speed;
        float pitchValue = speed == GameSpeed.Slow ? 0.75f : 1f;
        music.pitch = pitchValue;
        sfx.pitch = pitchValue;
    }
    public void FadeIn(AudioSource source, float duration)
    {
        StartCoroutine(FadeInCoroutine(source, duration));
    }

    public void FadeOut(AudioSource source, float duration)
    {
        StartCoroutine(FadeOutCoroutine(source, duration));
    }
    private IEnumerator FadeInCoroutine(AudioSource source, float duration)
    {
        source.volume = 0f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            source.volume = Mathf.Lerp(0f, 1f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        source.volume = 1f;  
    }
    private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        source.volume = 0f;  
        source.Stop();
    }
}
