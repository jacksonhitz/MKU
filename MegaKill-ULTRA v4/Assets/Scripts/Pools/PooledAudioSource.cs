using Lean.Pool;
using UnityEngine;

namespace Pools
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : Component, IPoolable
    {
        private AudioSource audioSource;

        /// <summary>Called when this poolable object is spawned.</summary>
        public void OnSpawn()
        {
            audioSource.volume = 1;
        }

        /// <summary>Called when this poolable object is despawned.</summary>
        public void OnDespawn()
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.volume = 0;
            audioSource.pitch = 1;
            audioSource.loop = false;
            audioSource.outputAudioMixerGroup = null;
        }
    }
}
