using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Audio/Sound")]
public class SoundData : ScriptableObject
{
    public enum SoundType
    {
        Music,
        Sfx,
        Dialogue
    }
    public SoundType soundType;

    public AudioClip clip;
    public float volume;
}


