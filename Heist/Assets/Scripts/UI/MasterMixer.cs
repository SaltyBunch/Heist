using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UI
{

    public class MasterMixer : MonoBehaviour
    {
        
        [SerializeField]
        AudioMixer masterMixer;

        [SerializeField]
        AudioMixerSnapshot pausedSnap;
        [SerializeField]
        AudioMixerSnapshot unpausedSnap;

        private bool isPaused;
        public bool CanPause { get; set; }

        public void UpdateMasterVolume(float lvl)
        {
            masterMixer.SetFloat("MasterVolume", lvl);
        }

        public void UpdateMusicVolume(float lvl)
        {
            masterMixer.SetFloat("MusicVolume", lvl);
        }

        public void UpdateSFXVolume(float lvl)
        {
            masterMixer.SetFloat("SFXVolume", lvl);
        }

        public void Pause()
        {
            if (CanPause)
            {
                isPaused = isPaused == false ? true : false;

                if (isPaused)
                {
                    pausedSnap.TransitionTo(0.1f);
                }
                else
                {
                    unpausedSnap.TransitionTo(1f);
                }
            }
        }
    }
}
