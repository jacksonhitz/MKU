using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "Audio/Sound")]
public class SoundData : ScriptableObject
{
    public enum SoundType
    {
        Music,
        Sfx,
        Dialogue,
    }

    public SoundType soundType;

    public AudioClip[] clips;
    public AudioMixerGroup mixerGroup;
    public bool loop;
    public bool frequentSound;

    public bool mute;
    public bool bypassEffects;
    public bool bypassListenerEffects;
    public bool bypassReverbZones;

    public int priority = 128;
    public float volume = 1f;
    public float pitch = 1f;
    public float panStereo;
    public float spatialBlend;
    public float reverbZoneMix = 1f;
    public float dopplerLevel = 1f;
    public float spread;

    public float minDistance = 1f;
    public float maxDistance = 500f;

    [BoxGroup]
    public bool ignoreListenerVolume;
    public bool ignoreListenerPause;

    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    public void OnValidate() { }
}
