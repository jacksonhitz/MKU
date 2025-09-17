using JetBrains.Annotations;
using UnityEngine;

namespace AudioSystem
{
    public class SoundBuilder
    {
        readonly SoundManager soundManager;
        Vector3 position = Vector3.zero;
        bool randomPitch;

        public SoundBuilder(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            this.randomPitch = true;
            return this;
        }

        /// <summary>
        /// Play the sound specified by the <see cref="SoundData"/>.
        /// </summary>
        /// <param name="soundData">The sound to play with the specified configuration.</param>
        /// <returns>The sound emitter used to play the sound, else null if the sound couldn't be played.</returns>
        [CanBeNull]
        public SoundEmitter Play(SoundData soundData)
        {
            if (soundData == null)
            {
                Debug.LogError("SoundData is null");
                return null;
            }

            if (!soundManager.CanPlaySound(soundData))
                return null;

            var soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;

            if (randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (soundData.frequentSound)
            {
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);
            }

            soundEmitter.Play();
            return soundEmitter;
        }

        /// <summary>
        /// Play the sound specified by the name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        /// <returns>The sound emitter used to play the sound, else null if the sound couldn't be found or played.</returns>'
        [CanBeNull]
        public SoundEmitter Play(string soundName)
        {
            var sound = soundManager.GetSound(soundName);
            if (sound == null)
            {
                Debug.LogError($"Sound {soundName} not found");
                return null;
            }
            return Play(sound);
        }
    }
}
