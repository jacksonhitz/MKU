using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource dialogueSource;

    [Header("3D Settings")]
    [SerializeField] float minDistance = 5f;
    [SerializeField] float maxDistance = 30f;
    [SerializeField] AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    public float sfxVolume;
    public float musicVolume;

    void Awake()
    {
        Instance = this;
    }

    //2d sounds
    public void Play(SoundData sound)
    {
        AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];

        switch (sound.soundType)
        {
            case SoundData.SoundType.Music:
                musicSource.clip = clip;
                musicSource.volume = sound.volume * musicVolume;
                musicSource.Play();
                break;
            case SoundData.SoundType.Sfx:
                sfxSource.PlayOneShot(clip, sound.volume * sfxVolume);
                break;
            case SoundData.SoundType.Dialogue:
                dialogueSource.clip = clip;
                dialogueSource.volume = sound.volume * sfxVolume;
                dialogueSource.Play();
                break;
        }
    }


    //for spatial sounds like enemy effects
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
        audioSource.volume = sound.volume * sfxVolume;

        audioSource.Play();
        Destroy(audioHolder, clip.length + 0.1f);
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        source.volume = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            source.volume = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        source.volume = 1f;
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}
