using System;
using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioHelper
    {
        public static IEnumerator CrossFade(AudioSource audioSrc1, AudioSource audioSrc2, AudioClip fadeIntoClip,
            float duration)
        {
            duration = Math.Abs(duration) < 0.001 ? Time.deltaTime : duration;
            var vol = audioSrc1.volume;
            float time = 0;
            audioSrc2.volume = 0;
            audioSrc2.clip = fadeIntoClip;
            audioSrc2.Play();
            audioSrc2.loop = true;

            do
            {
                audioSrc1.volume -= vol / duration * Time.deltaTime;
                audioSrc2.volume += vol / duration * Time.deltaTime;
                yield return null;
                time += Time.deltaTime;
            } while (time < duration);

            audioSrc1.Stop();
        }
    }
}