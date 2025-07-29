using System.Collections.Generic;
using AudioSystem;
using Lean.Pool;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityUtils;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [Header("Audio Sources")]
    public AudioSource music;

    public AudioSource dialogue;

    [Header("3D Settings")]
    [SerializeField]
    private float minDistance = 5f;

    [SerializeField]
    private float maxDistance = 30f;

    [SerializeField]
    private AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Prefabs")]
    [SerializeField]
    [Required]
    private GameObject sfxPrefab;

    [Header("Mixers")]
    [SerializeField]
    [Required]
    private AudioMixerGroup musicMixer;

    [SerializeField]
    [Required]
    private AudioMixerGroup sfxMixer;

    [SerializeField]
    [Required]
    private AudioMixerGroup dialogueMixer;

    [Header("Pool Settings")]
    [SerializeField]
    SoundEmitter soundEmitterPrefab;

    [SerializeField]
    bool collectionCheck = true;

    [SerializeField]
    int defaultCapacity = 10;

    [SerializeField]
    int maxPoolSize = 100;

    [SerializeField]
    int maxSoundInstances = 30;

    private readonly Dictionary<string, SoundData> soundLookup = new();
    public LinkedList<SoundEmitter> FrequentSoundEmitters { get; set; } = new();
    IObjectPool<SoundEmitter> soundEmitterPool;
    readonly List<SoundEmitter> activeSoundEmitters = new();

    protected override void Awake()
    {
        base.Awake();
        SoundData[] loadedSounds = Resources.LoadAll<SoundData>("Sounds");
        foreach (SoundData sound in loadedSounds)
            soundLookup.TryAdd(sound.name, sound);
    }

    void Start()
    {
        InitializePool();
    }

    public SoundData GetSound(string soundName)
    {
        if (soundLookup.TryGetValue(soundName, out SoundData sound))
        {
            if (sound.clips.Length != 0)
            {
                return sound;
            }
        }
        Debug.LogWarning($"Sound '{soundName}' not found.");
        return null;
    }

    public void MusicOff()
    {
        music.Stop();
    }

    public void Stop(SoundData.SoundType type)
    {
        switch (type)
        {
            case SoundData.SoundType.Music:
                music.Stop();
                break;
            case SoundData.SoundType.Sfx:
                // sfx.Stop();
                break;
            case SoundData.SoundType.Dialogue:
                dialogue.Stop();
                break;
        }
    }

    // //2D Grabber
    // public void Play(string soundName)
    // {
    //     SoundData sound = GetSound(soundName);
    //     if (sound)
    //     {
    //         Play(sound);
    //     }
    // }
    //
    // //3D Grabber
    // public void Play(string soundName, Vector3 pos)
    // {
    //     SoundData sound = GetSound(soundName);
    //     if (sound)
    //     {
    //         Play(sound, pos);
    //     }
    // }
    //
    // //2D Player
    // public void Play(SoundData sound)
    // {
    //     AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];
    //     var emitter = soundEmitterPool.Get();
    //     var source = emitter.Node;
    //     source.clip = clip;
    //     source.volume = sound.volume;
    //     source.spatialBlend = 0f;
    //
    //     switch (sound.soundType)
    //     {
    //         case SoundData.SoundType.Music:
    //             source.loop = true;
    //             source.outputAudioMixerGroup = musicMixer;
    //             Debug.Log("Music: " + sound);
    //             break;
    //         case SoundData.SoundType.Sfx:
    //             source.outputAudioMixerGroup = sfxMixer;
    //             Debug.Log("Sound: " + sound);
    //             break;
    //         case SoundData.SoundType.Dialogue:
    //             source.loop = true;
    //             source.outputAudioMixerGroup = dialogueMixer;
    //             break;
    //     }
    //
    //     source.Play();
    // }

    //3D Player
    public void Play(SoundData sound, Vector3 pos)
    {
        AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];

        var audioHolder = new GameObject("Holding: " + clip.name);
        audioHolder.transform.position = pos;

        var audioSource = audioHolder.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, falloffCurve);
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = sound.volume;

        audioSource.Play();
        Destroy(audioHolder, clip.length + 0.1f);
    }

    public SoundBuilder CreateSoundBuilder() => new SoundBuilder(this);

    public bool CanPlaySound(SoundData data)
    {
        if (!data.frequentSound)
            return true;

        if (FrequentSoundEmitters.Count < maxSoundInstances)
            return true;
        try
        {
            FrequentSoundEmitters.First.Value.Stop();
            return true;
        }
        catch
        {
            Debug.Log("SoundEmitter is already released");
        }
        return false;
    }

    public SoundEmitter Get()
    {
        return soundEmitterPool.Get();
    }

    public void ReturnToPool(SoundEmitter soundEmitter)
    {
        soundEmitterPool.Release(soundEmitter);
    }

    public void StopAll()
    {
        foreach (var soundEmitter in activeSoundEmitters)
        {
            soundEmitter.Stop();
        }

        FrequentSoundEmitters.Clear();
    }

    void InitializePool()
    {
        soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
        );
    }

    SoundEmitter CreateSoundEmitter()
    {
        var soundEmitter = Instantiate(soundEmitterPrefab);
        soundEmitter.gameObject.SetActive(false);
        return soundEmitter;
    }

    void OnTakeFromPool(SoundEmitter soundEmitter)
    {
        soundEmitter.gameObject.SetActive(true);
        activeSoundEmitters.Add(soundEmitter);
    }

    void OnReturnedToPool(SoundEmitter soundEmitter)
    {
        if (soundEmitter.Node != null)
        {
            FrequentSoundEmitters.Remove(soundEmitter.Node);
            soundEmitter.Node = null;
        }
        soundEmitter.gameObject.SetActive(false);
        activeSoundEmitters.Remove(soundEmitter);
    }

    void OnDestroyPoolObject(SoundEmitter soundEmitter)
    {
        Destroy(soundEmitter.gameObject);
    }
}
