﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface ISound
    {
        AudioClip Get(string sound);
        void Play(string sound);
        void Play(AudioClip sound);

        string PlayingMusicName { get; }
        bool IsPlaying { get; }

        void PlayDefaultSound();
        void SetDefaultSoundVolume(float volume);
        float GetDefaultSoundVolume();

        void PlayRandomMusic();
        void PlayDefaultMusic();
        void StopDefaultMusic();
        void SetDefaultMusicVolume(float volume);
        float GetDefaultMusicVolume();
        //void PlayGrassSound();
        //void PlayWoodSound();
        //void PlayStoneSound();
        //void PlayClothSound();
    }

    public class SoundEffectDisabled { }
    public class SoundMusicEnabled { }

    public class SoundEffectVolume { }
    public class SoundMusicVolume { }

    public class SoundMusicIndex { }

    public class Sound : MonoBehaviour, ISound
    {
        public static ISound Ins { get; private set; }
        public static Sound InsImpl { get; private set; }

        public bool IsPlaying => musicSource.isPlaying;


        [SerializeField]
        private AudioSource sfxSource;
        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        private AudioClip defaultSound;

        [SerializeField]
        private AudioClip defaultMusic;
        [SerializeField]
        private AudioClip[] defaultMusics;

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
            sfxSource.PlayOneShot(sound);
        }
        public void Play(string sound) {
            AudioClip audioClip = Get(sound);
            sfxSource.PlayOneShot(audioClip);
        }

        private readonly Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();
        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;

            foreach (var sound in Sounds) {
                if (sound == null) {
                    continue;
                }
                dict.Add(sound.name, sound);
            }

            // musicIndex = Math.Abs((int)HashUtility.Hash((uint)TimeUtility.GetTicks())) % defaultMusics.Length;
        }
        private IValue musicIndex;
        private void Start() {
            musicIndex = Globals.Ins.Values.GetOrCreate<SoundMusicIndex>();
            musicIndex.Max++;
            if (Globals.Ins.Bool<SoundMusicEnabled>()) {
                PlayDefaultMusic();
            }
            SetDefaultMusicVolume(GetDefaultMusicVolume());
            SetDefaultSoundVolume(GetDefaultSoundVolume());
        }


        private const string defaultSoundName = "mixkit-cool-interface-click-tone-2568";
        public void PlayDefaultSound() {
            if (Globals.Ins.Bool<SoundEffectDisabled>()) {
                return;
            }
            if (defaultSound == null) defaultSound = Get(defaultSoundName);
            sfxSource.PlayOneShot(defaultSound);
        }
        public void SetDefaultSoundVolume(float volume) {
            sfxSource.volume = volume;
            Globals.Ins.Values.Get<SoundEffectVolume>().Max = (long)(volume * soundFactor);
        }

        private const float soundFactor = 1000f;
        public float GetDefaultSoundVolume() {
            return Globals.Ins.Values.Get<SoundEffectVolume>().Max / soundFactor;
        }

        public string PlayingMusicName => musicSource.clip.name;

        public void PlayRandomMusic() {
            musicIndex = Globals.Ins.Values.GetOrCreate<SoundMusicIndex>();
            musicIndex.Max = Math.Abs(HashUtility.Hash((uint)TimeUtility.GetTicks())) % defaultMusics.Length;
        }

        public void PlayDefaultMusic() {
            if (musicSource.isPlaying) {
                return;
            }
            if (musicIndex.Max == defaultMusics.Length) {
                musicIndex.Max = 0;
            }
            musicSource.clip = defaultMusics[musicIndex.Max];
            musicSource.Play();
            musicIndex.Max++;
        }
        public void StopDefaultMusic() {
            musicSource.clip = defaultMusic;
            musicSource.Stop();
        }
        public void SetDefaultMusicVolume(float volume) {
            musicSource.volume = volume;
            Globals.Ins.Values.Get<SoundMusicVolume>().Max = (long)(volume * soundFactor);
        }
        public float GetDefaultMusicVolume() {
            return Globals.Ins.Values.Get<SoundMusicVolume>().Max / soundFactor;
        }



        private const float silencedTime = 60f;
        private float timeAcc = 0;
        private void Update() {
            if (Globals.Ins.Bool<SoundMusicEnabled>() && !musicSource.isPlaying) {
                timeAcc += Time.deltaTime;
                if (timeAcc > silencedTime) {
                    timeAcc = 0;
                    PlayDefaultMusic();
                }
            }
        }



        //private const int length = 4;
        //private bool soundIntialized = false;
        //private void TryInitializeSound() {
        //    if (soundIntialized) return;
        //    soundIntialized = true;
        //    for (int i = 0; i < length; i++) {
        //        grassSounds.Add(Get($"grass{i + 1}"));
        //    }
        //    for (int i = 0; i < length; i++) {
        //        woodSounds.Add(Get($"wood{i + 1}"));
        //    }
        //    for (int i = 0; i < length; i++) {
        //        stoneSounds.Add(Get($"stone{i + 1}"));
        //    }
        //    for (int i = 0; i < length; i++) {
        //        clothSounds.Add(Get($"cloth{i + 1}"));
        //    }
        //}
        //private int GetIndex() {
        //    return (int)(Utility.GetTicks() / (Value.Second / 10) % length);
        //}

        //private List<AudioClip> grassSounds = new List<AudioClip>(length);
        //public void PlayGrassSound() {
        //    TryInitializeSound();
        //    Play(grassSounds[GetIndex()]);
        //}

        //private List<AudioClip> woodSounds = new List<AudioClip>(length);
        //public void PlayWoodSound() {
        //    TryInitializeSound();
        //    Play(woodSounds[GetIndex()]);
        //}

        //private List<AudioClip> stoneSounds = new List<AudioClip>(length);
        //public void PlayStoneSound() {
        //    TryInitializeSound();
        //    Play(stoneSounds[GetIndex()]);
        //}

        //private List<AudioClip> clothSounds = new List<AudioClip>(length);
        //public void PlayClothSound() {
        //    TryInitializeSound();
        //    Play(clothSounds[GetIndex()]);
        //}
    }
}

