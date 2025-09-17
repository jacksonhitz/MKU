using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace AudioSystem
{
    public class MusicEmitter : MonoBehaviour
    {
        private AudioSource introSource;
        private AudioSource loopSource;
        private AudioSource currentSource;

        [SerializeField]
        [ReadOnly]
        private string trackName;

        public SoundData Data { get; private set; }
        public SoundData.SoundType Type => SoundData.SoundType.Music;
        public bool IsPlaying => currentSource && currentSource.isPlaying;
        public float Volume
        {
            get => currentSource?.volume ?? 0;
            set
            {
                if (currentSource != null)
                    currentSource.volume = value;
                else
                {
                    Debug.LogWarning("Trying to set volume of non-existent music emitter");
                }
            }
        }

        public void Initialize(SoundData data)
        {
            if (loopSource || currentSource)
            {
                Stop();
                if (loopSource)
                    Destroy(loopSource);
                if (introSource)
                    Destroy(currentSource);
            }

            Data = data;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.clips.Length > 0);
            if (data.clips.Length == 1)
            {
                loopSource = gameObject.AddComponent<AudioSource>();
                loopSource.InitializeSource(data, soundData => soundData.clips[0]);

                currentSource = loopSource;
            }
            else
            {
                introSource = gameObject.AddComponent<AudioSource>();
                introSource.InitializeSource(data, soundData => soundData.clips[0]);
                introSource.loop = false;
                loopSource = gameObject.AddComponent<AudioSource>();
                loopSource.InitializeSource(data, soundData => soundData.clips[1]);
                loopSource.loop = true;

                currentSource = introSource;
            }
        }

        public void Play()
        {
            SchedulePlayback();
        }

        public void Stop()
        {
            introSource?.Stop();
            loopSource?.Stop();
        }

        public void FadeAndStop(float fadeTime)
        {
            StartCoroutine(FadeOut(fadeTime));
        }

        private void SchedulePlayback()
        {
            double t0 = AudioSettings.dspTime + 0.1f;
            double clipTime1 = currentSource.clip.samples;
            clipTime1 /= currentSource.clip.frequency;
            currentSource.PlayScheduled(t0);
            trackName = currentSource.clip.name;
            if (!introSource)
                return;
            loopSource.PlayScheduled(t0 + clipTime1);

            // Update the song name in the inspector after the loop track starts
            UniTask
                .Delay(
                    TimeSpan.FromSeconds(clipTime1),
                    DelayType.UnscaledDeltaTime,
                    PlayerLoopTiming.Update,
                    destroyCancellationToken
                )
                .ContinueWith(() => trackName = loopSource.clip.name)
                .Forget();
        }

        IEnumerator FadeOut(float fadeTime)
        {
            float fading = 0.001f;
            while (fading > 0f)
            {
                fading += Time.deltaTime;

                float fraction = Mathf.Clamp01(fading / fadeTime);

                // Logarithmic fade
                float logFraction = fraction.ToLogarithmicFraction();

                if (currentSource)
                    currentSource.volume = logFraction;

                if (fraction >= 1)
                {
                    fading = 0.0f;
                }

                yield return null;
            }

            Stop();
        }

        private void OnDestroy()
        {
            Destroy(introSource);
            Destroy(loopSource);
        }
    }
}
