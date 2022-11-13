using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraMan.Managers.SoundManagerHelpers
{
    /// <summary> 
    /// Audioclip wrapper is embedded with audio properties. 
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        /// <summary> Clip to play when this sound is played. </summary>
        [Tooltip("Clip to play when this sound is played.")]
        public AudioClip clip;

        /// <summary> Volume of clip when it is played. </summary>
        [Tooltip("Volume of clip when it is played."), Range(0f, 1f)]
        public float volume = 0.7f;

        /// <summary> Creates playable sound with default volume (70%). </summary>
        /// <param name="clip"> Clip to generate a playable sound from. </param>
        public Sound(AudioClip clip)
        {
            this.clip = clip;
            volume = 0.7f;
        }

        /// <summary> Creates playable sound with specified volume. </summary>
        /// <param name="clip"> Clip to generate a playable sound from. </param>
        /// <param name="volume"> Default normalized volume of clip [0, 1]. </param>
        public Sound(AudioClip clip, float volume)
        {
            this.clip = clip;
            this.volume = Mathf.Clamp(volume, 0, 1);
        }
    }
}
