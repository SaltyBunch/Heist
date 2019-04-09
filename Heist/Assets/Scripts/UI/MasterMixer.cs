using UnityEngine;
using UnityEngine.Audio;

namespace UI
{
    public class MasterMixer : MonoBehaviour
    {
        private bool isPaused;

        [SerializeField] private AudioMixer masterMixer;

        [SerializeField] private AudioMixerSnapshot pausedSnap;

        [SerializeField] private AudioMixerSnapshot unpausedSnap;

        public bool CanPause { get; set; }

        public void UpdateMasterVolume(float lvl)
        {
            masterMixer.SetFloat("MasterVolume", lvl);
        }

        public float GetMasterVolume()
        {
            float lvl = 0;
            masterMixer.GetFloat("MasterVolume", out lvl);
            return lvl;
        }

        public void UpdateMusicVolume(float lvl)
        {
            masterMixer.SetFloat("MusicVolume", lvl);
        }
        public float GetMusicVolume()
        {
            float lvl = 0;
            masterMixer.GetFloat("MusicVolume", out lvl);
            return lvl;
        }

        public void UpdateSFXVolume(float lvl)
        {
            masterMixer.SetFloat("SFXVolume", lvl);
        }
        public float GetSFXVolume()
        {
            float lvl = 0;
            masterMixer.GetFloat("SFXVolume", out lvl);
            return lvl;
        }
        
        public void UpdateVoiceVolume(float lvl)
        {
            masterMixer.SetFloat("VoiceVolume", lvl);
        }
        public float GetVoiceVolume()
        {
            float lvl = 0;
            masterMixer.GetFloat("VoiceVolume", out lvl);
            return lvl;
        }

        public void Pause()
        {
            if (CanPause)
            {
                isPaused = isPaused == false ? true : false;

                if (isPaused)
                    pausedSnap.TransitionTo(0.1f);
                else
                    unpausedSnap.TransitionTo(1f);
            }
        }
    }
}