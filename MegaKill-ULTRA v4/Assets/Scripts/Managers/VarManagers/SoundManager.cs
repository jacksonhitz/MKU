using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
        [Header("Audio Sources")]
    public AudioSource music;
    public AudioSource sfx;        
    public AudioSource dialogue;
    
    [Header("3D Sound Settings")]
    public float minDistance = 5f;
    public float maxDistance = 30f;
    public AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public int spatialPoolSize = 10;
    private List<AudioSource> spatialAudioPool;
    
    [Header("Music Tracks")]
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

    [Header("Dialogue")]
    public AudioClip line0;
    public AudioClip line1;
    public AudioClip line2;
    public AudioClip line3;
    public AudioClip line4;
    public AudioClip line5;
    public AudioClip line6;
    public AudioClip line7;

    [Header("2D SFX")]
    public AudioClip heartbeat;
    public AudioClip playerDeath;
    public AudioClip pillEmpty;
    public AudioClip giveDrug;

    public AudioClip[] gulpSounds;

    public void Gulp()
    {
        if (gulpSounds != null && gulpSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, gulpSounds.Length);
        
            PlaySfx(gulpSounds[randomIndex]);
        }
        else
        {
            Debug.LogWarning("No gulp sounds assigned to SoundManager!");
        }
    }

    public AudioClip[] punchSounds;

    public void Punch()
    {
        if (punchSounds != null && punchSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, punchSounds.Length);
            PlaySfx(punchSounds[randomIndex]);
        }
        else if (punch != null)
        {
            PlaySfx(punch);
        }
        else
        {
            Debug.LogWarning("No punch sounds assigned to SoundManager!");
        }
    }

    [Header("3D SFX")]
    public AudioClip revShot;
    public AudioClip revEmpty;
    public AudioClip shotShot;
    public AudioClip shotEmpty;
    public AudioClip mgShot;
    public AudioClip mgEmpty;
    public AudioClip batSwing;
    public AudioClip punch;
    public AudioClip toss;
    public AudioClip tossHit;
    public AudioClip playerHit;
    public AudioClip enemyDeath;
    public AudioClip enemyHit;
    public AudioClip enemyStun;

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

        // Initialize spatial audio pool
        InitializeSpatialAudioPool();
            
        // Configure audio sources for 2D/3D
        ConfigureAudioSources();
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
        dialoguePlaying = dialogue.isPlaying;
        
        sfx.Pause();
        dialogue.Pause();
        
        // Also pause all spatial audio sources
        foreach (AudioSource source in spatialAudioPool)
        {
            if (source.isPlaying)
                source.Pause();
        }
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
        
        // Unpause all spatial audio sources
        foreach (AudioSource source in spatialAudioPool)
        {
            if (source.gameObject.activeSelf && !source.isPlaying)
                source.Play();
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
        if (source == null || sound == null) return;
        
        // Configure the source for 3D spatial sound
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Custom;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, falloffCurve);
        
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

    // 3D
    //public void PlaySpatialRevShot(Vector3 position) => PlaySpatialSound(revShot, position);
    //public void PlaySpatialRevEmpty(Vector3 position) => PlaySpatialSound(revEmpty, position);
    //public void PlaySpatialShotShot(Vector3 position) => PlaySpatialSound(shotShot, position);
    //public void PlaySpatialShotEmpty(Vector3 position) => PlaySpatialSound(shotEmpty, position);
    //public void PlaySpatialMGShot(Vector3 position) => PlaySpatialSound(mgShot, position);
    //public void PlaySpatialMGEmpty(Vector3 position) => PlaySpatialSound(mgEmpty, position);
    //public void PlaySpatialBatSwing(Vector3 position) => PlaySpatialSound(batSwing, position);
    //public void PlaySpatialToss(Vector3 position) => PlaySpatialSound(toss, position);
    //public void PlaySpatialTossHit(Vector3 position) => PlaySpatialSound(tossHit, position);
    //public void PlaySpatialPlayerHit(Vector3 position) => PlaySpatialSound(playerHit, position);
    //public void PlaySpatialEnemyDeath(Vector3 position) => PlaySpatialSound(enemyDeath, position);
    //public void PlaySpatialEnemyHit(Vector3 position) => PlaySpatialSound(enemyHit, position);
    //public void PlaySpatialEnemyStun(Vector3 position) => PlaySpatialSound(enemyStun, position);

    public void RevShot() => PlaySpatialSound(revShot, Camera.main.transform.position);
    public void RevEmpty() => PlaySpatialSound(revEmpty, Camera.main.transform.position);
    public void ShotShot() => PlaySpatialSound(shotShot, Camera.main.transform.position);
    public void ShotEmpty() => PlaySpatialSound(shotEmpty, Camera.main.transform.position);
    public void MGShot() => PlaySpatialSound(mgShot, Camera.main.transform.position);
    public void MGEmpty() => PlaySpatialSound(mgEmpty, Camera.main.transform.position);
    public void BatSwing() => PlaySpatialSound(batSwing, Camera.main.transform.position); 
    public void Toss() => PlaySpatialSound(toss, Camera.main.transform.position);
    public void TossHit() => PlaySpatialSound(tossHit, Camera.main.transform.position);
    public void PlayerHit() => PlaySpatialSound(playerHit, Camera.main.transform.position);
    //public void EnemyHit(Vector3 position) => PlaySpatialSound(enemyHit, position);
    public void EnemyDeath() => PlaySpatialSound(enemyDeath, Camera.main.transform.position);
    public void EnemyHit() => PlaySpatialSound(enemyHit, Camera.main.transform.position);
    public void EnemyStun() => PlaySpatialSound(enemyStun, Camera.main.transform.position);
    

    // 2D
    public void Heartbeat() => PlaySfx(heartbeat);
    public void PlayerDeath() => PlaySfx(playerDeath);
    public void PillEmpty() => PlaySfx(pillEmpty);
    public void GiveDrug() => PlaySfx(giveDrug);

    /*
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
    public void PlayerHit() => PlaySfx(playerHit);
    public void Gulp() => PlaySfx(gulp);
    */

    void PlaySfx(AudioClip clip)
    {
        sfx.clip = clip;
        sfx.Play();
    }

    // <--! Shit that Leon added that hopefully works !-->

    private void InitializeSpatialAudioPool()
    {
        spatialAudioPool = new List<AudioSource>();
        
        GameObject poolParent = new GameObject("SpatialAudioPool");
        poolParent.transform.parent = this.transform;
        
        // Create the specified number of audio sources in the pool
        for (int i = 0; i < spatialPoolSize; i++)
        {
            GameObject audioObj = new GameObject("SpatialAudioSource_" + i);
            audioObj.transform.parent = poolParent.transform;
            
            AudioSource source = audioObj.AddComponent<AudioSource>();
            
            // Configure for 3D spatial audio
            source.spatialBlend = 1.0f;  // Fully 3D
            source.rolloffMode = AudioRolloffMode.Custom;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, falloffCurve);
            source.playOnAwake = false;
            source.loop = false;
            
            spatialAudioPool.Add(source);
            
            // Deactivate initially
            audioObj.SetActive(false);
        }
        
        Debug.Log($"Initialized spatial audio pool with {spatialPoolSize} sources");
    }    

    private void ConfigureAudioSources()
    {
        // Ensure music and dialogue are 2D
        music.spatialBlend = 0f;
        dialogue.spatialBlend = 0f;
        sfx.spatialBlend = 0f;
    }

    private AudioSource GetSpatialAudioSource()
    {
        foreach (AudioSource source in spatialAudioPool)
        {
            if (!source.isPlaying)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }
        
        // If all sources are active, use the oldest one (first in the list)
        AudioSource oldestSource = spatialAudioPool[0];
        oldestSource.Stop();
        oldestSource.gameObject.SetActive(true);
        
        // Move to the end of the list (round-robin)
        spatialAudioPool.RemoveAt(0);
        spatialAudioPool.Add(oldestSource);
        
        return oldestSource;
    }

    public void PlaySpatialSound(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (clip == null) return;
        
        AudioSource source = GetSpatialAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = sfxVol * volumeMultiplier;
        source.Play();
        
        // Auto-disable the GameObject when done playing
        StartCoroutine(ReturnToPoolWhenFinished(source));
    }

    private IEnumerator ReturnToPoolWhenFinished(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length + 0.1f);
        source.gameObject.SetActive(false);
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
