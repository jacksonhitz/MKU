using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityUtils;

namespace AudioSystem
{
    [RequireComponent(typeof(MusicManager))]
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        const float crossFadeTime = 1.0f;
        readonly Queue<SoundData> playlist = new();
        MusicEmitter current;
        MusicEmitter previous;
        private float fading;

        [ReadOnly]
        public string currentSong;

        [ReadOnly]
        public string previousSong;

        public void Clear() => playlist.Clear();

        public void AddToPlaylist(SoundData data)
        {
            playlist.Enqueue(data);
            if (current == null && previous == null)
            {
                PlayNextTrack();
            }
        }

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out SoundData nextTrack))
            {
                Play(nextTrack);
            }
        }

        public void Play(SoundData data)
        {
            // if (current && current.Data == data)
            //     return;

            if (previous)
            {
                Destroy(previous);
                previous = null;
            }

            previous = current;
            previousSong = currentSong;
            current = gameObject.AddComponent<MusicEmitter>();
            current.Initialize(data);
            currentSong = data.name;

            current.Play();

            fading = 0.001f;
        }

        public void Stop(bool fadeOut = true)
        {
            if (previous)
            {
                previous.Stop();
                Destroy(previous);
                previous = null;
                previousSong = currentSong;
            }

            if (fadeOut)
            {
                fading = 0.001f;
            }

            if (current)
            {
                previous = current;
                current = null;
                currentSong = null;
            }
        }

        private void Update()
        {
            HandleCrossFade();

            if (current && !current.IsPlaying && playlist.Count > 0)
            {
                PlayNextTrack();
            }
        }

        private void HandleCrossFade()
        {
            if (fading <= 0f)
                return;

            fading += Time.unscaledDeltaTime;

            float fraction = Mathf.Clamp01(fading / crossFadeTime);

            // Logarithmic fade
            float logFraction = fraction.ToLogarithmicFraction();
            if (previous)
                previous.Volume = 1.0f - logFraction;
            if (current)
                current.Volume = logFraction;

            if (fraction < 1)
                return;
            fading = 0.0f;
            if (!previous)
                return;
            Destroy(previous);
            previous = null;
            previousSong = currentSong;
        }
    }
}
