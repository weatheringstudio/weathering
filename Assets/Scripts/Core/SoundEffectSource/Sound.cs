
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

        void PlayDefaultSound();

        //void PlayGrassSound();
        //void PlayWoodSound();
        //void PlayStoneSound();
        //void PlayClothSound();
    }

    public class Sound : MonoBehaviour, ISound
    {
        public static ISound Ins;


        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip defaultSound;

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
            audioSource.PlayOneShot(sound);
        }
        public void Play(string sound) {
            AudioClip audioClip = Get(sound);
            audioSource.PlayOneShot(audioClip);
        }

        private Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();
        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;

            foreach (var sound in Sounds) {
                dict.Add(sound.name, sound);
            }
        }

        private const string defaultSoundName = "mixkit-cool-interface-click-tone-2568";
        public void PlayDefaultSound() {
            if (defaultSound == null) defaultSound = Get(defaultSoundName);
            audioSource.PlayOneShot(defaultSound, 0.2f);
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

