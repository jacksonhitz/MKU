using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityUtils;
using Random = UnityEngine.Random;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }
        public SoundData.SoundType Type => Data.soundType;

        private AudioSource audioSource;
        Coroutine playingCoroutine;

        private void Awake()
        {
            audioSource = gameObject.GetOrAdd<AudioSource>();
        }

        public void Initialize(SoundData data)
        {
            Data = data;
            audioSource.InitializeSource(data);
        }

        public void Play()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        private IEnumerator WaitForSoundToEnd()
        {
            yield return UniTask.WaitWhile(() => audioSource.isPlaying).ToCoroutine();
            Stop();
        }

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
    }
}
