using UltraMan.Inspector;
using UnityEngine;

namespace UltraMan.Managers.SoundManagerHelpers
{
    /// <summary> 
    /// Wrapper for sound that loops dynamically depending on loop settings. 
    /// </summary>
    [System.Serializable]
    public class Music
    {
        /// <summary> Sound to play as music. </summary>
        [Tooltip("Sound to play as music.")]
        public Sound sound;

        /// <summary> Determines if this music loops or not. </summary>
        [Tooltip("Determines if this music loops or not. " +
            "When looping, the music will start at the beginning and end at a specified point, " +
            " then loop back to another specified point.")]
        public bool dyanamicLoop;

        /// <summary> When music loops back around, it will start at this second. </summary>
        [Tooltip("When music loops back around, it will start at this second."), ConditionalHide("dyanamicLoop", false)]
        public float loopStart;

        /// <summary> When music reaches this second, it will loop back to 'loopStart'. </summary>
        [Tooltip("When music reaches this second, it will loop back to 'loopStart'."), ConditionalHide("dyanamicLoop", false)]
        public float loopEnd;

    }
}
