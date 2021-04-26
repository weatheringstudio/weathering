
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface ISound
    {



        string PlayingMusicName { get; }
        bool IsPlaying { get; }




        AudioClip Get(string sound);
        void PlayDefaultSound();
        void Play(string sound);
        void Play(AudioClip sound);
        float SoundVolume { get; set; }


        float RainDensity { set; }
        bool WeatherEnabled { get; set; }
        float WeatherVolume { get; set; }


        int MusicCount { get; }
        bool MusicEnabled { get; set; }
        float MusicVolume { get; set; }
    }

    public class SoundEnabled { }
    public class MusicEnabled { }
    public class SoundVolume { }
    public class MusicVolume { }
    public class WeatherEnabled { }
    public class WeatherVolume { }

    public class SoundMusicIndex { }

    public class Sound : MonoBehaviour, ISound
    {
        public static ISound Ins { get; private set; }

        public bool IsPlaying => musicSource.isPlaying;


        [SerializeField]
        private AudioSource sfxSource;
        [SerializeField]
        private AudioSource musicSource;
        [SerializeField]
        private AudioSource weatherSource;

        [SerializeField]
        private AudioClip defaultSound;

        [SerializeField]
        private AudioClip[] defaultMusics;

        public int MusicCount => defaultMusics.Length;

        [SerializeField]
        private AudioClip[] Sounds;




        public AudioClip Get(string sound) {
            AudioClip audioClip;
            if (!dict.TryGetValue(sound, out audioClip)) {
                throw new Exception(sound);
            }
            return audioClip;
        }

        public void Play(AudioClip sound) {
            if (Globals.Ins.Bool<SoundEnabled>()) {
                sfxSource.PlayOneShot(sound);
            }
        }
        public void Play(string sound) {
            Play(Get(sound));
        }


        private readonly Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;

            foreach (var sound in Sounds) {
                if (sound == null) {
                    continue;
                }
                dict.Add(sound.name, sound);
            }

            // musicIndex = Math.Abs((int)HashUtility.Hash((uint)TimeUtility.GetTicks())) % defaultMusics.Length;
        }
        // private IValue musicIndex;
        private void Start() {
            // musicIndex = Globals.Ins.Values.GetOrCreate<SoundMusicIndex>();
        }


        private const string defaultSoundName = "mixkit-cool-interface-click-tone-2568";
        public void PlayDefaultSound() {
            if (!Globals.Ins.Bool<SoundEnabled>()) {
                return;
            }
            if (defaultSound == null) defaultSound = Get(defaultSoundName);
            sfxSource.PlayOneShot(defaultSound);
        }

        public float SoundVolume { get => sfxSource.volume; set => sfxSource.volume = value; }




        public bool WeatherEnabled { get => weatherSource.isPlaying; set {
                // if (value) weatherSource.Play(); else weatherSource.Stop(); 
                if (value && !weatherSource.isPlaying) {
                    weatherSource.Play();
                } else if (!value && weatherSource.isPlaying) {
                    weatherSource.Stop();
                }
            }
        }

        private float weatherVolume = 0;
        public float WeatherVolume { get => weatherSource.volume; set {
                weatherVolume = value;
            } 
        }

        public float RainDensity { set {
                weatherSource.volume = value * weatherVolume;
            } 
        }


        /// <summary>
        /// music
        /// </summary>
        public string PlayingMusicName => musicSource.clip.name;
        public bool MusicEnabled {
            get => musicSource.isPlaying; set {
                if (value) {
                    if (musicSource.clip != null && musicSource.isPlaying) {
                        return;
                    }
                    musicSource.clip = defaultMusics[HashMusicIndex()];
                    musicSource.Play();
                } else {
                    musicSource.Stop();
                }
            }
        }
        private uint HashMusicIndex() {
            return (uint)(HashUtility.Hash((uint)(TimeUtility.GetSecondsInDouble() / 30)) % defaultMusics.Length);
        }

        public float MusicVolume { get => musicSource.volume; set => musicSource.volume = value; }


        /// <summary>
        /// auto pause
        /// </summary>

        private uint lastMusicIndex = 0;

        private const float silencedTime = 60f;
        private float timeSilencedAcc = 0;
        private void Update() {
            if (!musicSource.isPlaying) {
                timeSilencedAcc += Time.deltaTime;

                uint thisMusicIndex = HashMusicIndex();
                if (timeSilencedAcc > silencedTime && Globals.Ins.Bool<MusicEnabled>() && thisMusicIndex != lastMusicIndex) {
                    timeSilencedAcc = 0;
                    MusicEnabled = true;
                }
                lastMusicIndex = thisMusicIndex;
            }
            //else {
            //    float time = musicSource.time;
            //    if (time < fadeInTime + 1) {
            //        float maxVolume = MusicVolume;
            //        musicSource.volume = Mathf.Lerp(0, maxVolume, time / fadeInTime);
            //    }
            //}
        }
    }
}

