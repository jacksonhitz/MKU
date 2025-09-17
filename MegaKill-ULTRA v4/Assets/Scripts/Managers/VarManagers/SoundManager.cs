using System.Collections.Generic;
using AudioSystem;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityUtils;
using Debug = UnityEngine.Debug;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [Header("3D Settings")]
    [SerializeField]
    private float minDistance = 5f;

    [SerializeField]
    private float maxDistance = 30f;

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

    private void Start()
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
        for (int i = activeSoundEmitters.Count - 1; i >= 0; i--)
        {
            activeSoundEmitters[i].Stop();
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
