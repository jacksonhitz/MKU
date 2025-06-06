using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Audio Sources")]
    public AudioSource music;
    public AudioSource sfx;
    public AudioSource dialogue;

    [Header("3D Settings")]
    [SerializeField] float minDistance = 5f;
    [SerializeField] float maxDistance = 30f;
    [SerializeField] AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    Dictionary<string, SoundData> soundLookup = new();

    void Awake()
    {
        Instance = this;

        SoundData[] loadedSounds = Resources.LoadAll<SoundData>("Sounds");
        foreach (var sound in loadedSounds)
        {
            if (!soundLookup.ContainsKey(sound.name))
                soundLookup.Add(sound.name, sound);
        }
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
            case StateManager.GameState.TUTORIAL: Play("Hot"); break;
            case StateManager.GameState.REHEARSAL: Play("Acid"); break;
            case StateManager.GameState.TANGO: Play("Witch"); break;
            case StateManager.GameState.TANGO2: Play("Magic"); break;
            case StateManager.GameState.SABLE: Play("4L"); break;
            case StateManager.GameState.SPEARHEAD: Play("DJ"); break;
            case StateManager.GameState.SCORE: MusicOff(); break;
        }

    }

    public SoundData GetSound(string soundName)
    {
        if (soundLookup.TryGetValue(soundName, out SoundData sound))
        {
            if (sound.clips.Length != 0) return sound;
        }
        Debug.LogWarning($"Sound '{soundName}' not found.");
        return null;
    }

    public void MusicOff()
    {
        music.Stop();
    }

    public void Stop()
    {
        sfx.Stop();
        dialogue.Stop();
    }

    //2D Grabber
    public void Play(string soundName)
    {
        SoundData sound = GetSound(soundName);
        Debug.Log("Sound: " + sound);
        if (sound != null)
            Play(sound);
    }

    //3D Grabber
    public void Play(string soundName, Vector3 pos)
    {
        SoundData sound = GetSound(soundName);
        if (sound != null)
            Play(sound, pos);
    }


    //2D Player
    public void Play(SoundData sound)
    {
        AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];

        Debug.Log("Sound: " + sound);

        switch (sound.soundType)
        {
            case SoundData.SoundType.Music:
                music.clip = clip;
                music.volume = sound.volume * music.volume;
                music.Play();
                Debug.Log("Music: " + sound);
                break;
            case SoundData.SoundType.Sfx:
                sfx.clip = clip;
                sfx.volume = sound.volume * sfx.volume;
                sfx.Play();
                break;
            case SoundData.SoundType.Dialogue:
                dialogue.clip = clip;
                dialogue.volume = sound.volume * sfx.volume;
                dialogue.Play();
                break;
        }
    }


    //3D Player
    public void Play(SoundData sound, Vector3 pos)
    {
        AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];

        GameObject audioHolder = new GameObject("Holding: " + clip.name);
        audioHolder.transform.position = pos;

        AudioSource audioSource = audioHolder.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, falloffCurve);
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = sound.volume * sfx.volume;

        audioSource.Play();
        Destroy(audioHolder, clip.length + 0.1f);
    }
}
