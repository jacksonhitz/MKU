using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    public AudioSource music;
    public AudioSource sfx;
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

    List<AudioClip> tracks;
    List<AudioClip> lines;
    
    int trackIndex = -1;
    int lineIndex = -1;

    bool sfxPlaying;
    bool dialoguePlaying;

    [HideInInspector] public float musicVol;
    [HideInInspector] public float sfxVol;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        tracks = new List<AudioClip> { witch, acid, could, dj, all, hott, threes, life, real, four };
        lines = new List<AudioClip> { line1, line2, line3, line4, line5, line6, line7 };
    }

    void Start()
    {
        Title();
        StateChange(StateManager.State);
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.TITLE: Title(); break;
            case StateManager.GameState.INTRO: Intro(); break;
            case StateManager.GameState.TUTORIAL: Tutorial(); break;
            case StateManager.GameState.TANGO: Tango(); break;
            case StateManager.GameState.PAUSED: Paused(); break;
            case StateManager.GameState.FIGHT: Fight(); break;
        }
    }

    void Title()
    {
        music.clip = title;
        music.Play();
    }
    void Intro()
    {
        music.Stop();
        dialogue.Play();
    }
    void Tutorial()
    {
        dialogue.Stop();
        music.clip = hott;
        music.Play();
    }
    void Tango()
    {
        dialogue.Stop();
        music.clip = witch;
        music.Play();
    }
    void Fight()
    {
        dialogue.Stop();
        music.clip = acid;
        music.Play();

    }


    public void Paused()
    {
        Debug.Log("sound paused");
        
       
        sfxPlaying = sfx.isPlaying;
        dialoguePlaying = sfx.isPlaying;
        
        music.Pause();
        sfx.Pause();
        dialogue.Pause();
    }
    public void Unpaused()
    {
        music.Play();

        if (sfxPlaying)
        {
            sfxPlaying = false;
            sfx.Play();
        }
        if (dialoguePlaying)
        {
            dialoguePlaying = false;
            dialogue.Play();
        }
    }

    void NewTrack()
    {
        if (tracks.Count > 1)
        {
            trackIndex = (trackIndex + 1) % tracks.Count;
            music.clip = tracks[trackIndex];
            music.Play();
        }
    }

    public void EnemySFX(AudioSource source, AudioClip sound)
    {
        source.volume = sfx.volume;
        source.clip = sound;
        source.Play();  
    }

    public void NewLine()
    {
        lineIndex = (lineIndex + 1) % lines.Count;
        dialogue.clip = lines[lineIndex];
        dialogue.Play();

        Debug.Log("dialogue playing");
    }
    public void StopLine()
    {
        dialogue.Stop();
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
    public void PlayerDeath() => PlaySfx(playerDeath);
    public void Gulp() => PlaySfx(gulp);
    public void PillEmpty() => PlaySfx(pillEmpty);

    void PlaySfx(AudioClip clip)
    {
        sfx.clip = clip;
        sfx.Play();
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
