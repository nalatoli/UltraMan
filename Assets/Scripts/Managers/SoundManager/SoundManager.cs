using System.Collections;
using System.Collections.Generic;
using UltraMan.Extensions;
using UltraMan.Managers.SoundManagerHelpers;
using UnityEngine;

namespace UltraMan.Managers
{
    /// <summary>
    /// Manages sound and music with Sounds. 
    /// Sounds are played on the SoundManager's embedded audiosources.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        #region Properties 

        /// <summary> List of globally available sound effects. </summary>
        [Tooltip("List of globally available sound effects. " +
            "The names of these sound effects are extracted, making them searchable by other scripts." +
            "(Ex: 'Blip.mp3' becomes searchable by 'Blip'."), SerializeField]
        private List<Sound> globalSFX = null;

        private static Dictionary<string, Sound> globalSFXmap;

        /// <summary> Audiosource that music will play on. </summary>
        private static AudioSource musicSource;

        /// <summary> Audiosources that sound will play on. </summary>
        private static List<AudioSource> soundSources;

        /// <summary> Current music the music source is playing. </summary>
        private static Music currentMusic;

        /// <summary> Instance to start coroutines with. </summary>
        private static SoundManager instance;

        /// <summary> Coroutine tracker. </summary>
        private static IEnumerator running;

        #endregion

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            /* Initialize Instance (Throw Error If More Than One) */
            if (instance != null)
                Debug.LogWarning("More than one Sound Manager in this scene.");
            else
                instance = this;

            /* Get Components */
            soundSources = new List<AudioSource>(GetComponents<AudioSource>());
            musicSource = soundSources[soundSources.Count - 1];
            soundSources.RemoveAt(soundSources.Count - 1);
            currentMusic = null;

            /* Initialize Global Sound Effect Map */
            globalSFXmap = new Dictionary<string, Sound>();
            foreach (Sound sound in globalSFX)
                globalSFXmap.Add(sound.clip.name, sound);

        }

        /// <summary> Gets a sound by name from the Soundmanager's list of internal sounds. </summary>
        /// <param name="soundName"> String to look for. </param>
        /// <returns> The sound by name. Null if no sound found. </returns>
        public static Sound GetSound(string soundName)
        {
            /* Return Sound By Name If Sound Exsists */
            if (globalSFXmap.ContainsKey(soundName))
                return globalSFXmap[soundName];

            /* Otherwise, Warn User and Return Null */
            Debug.LogWarning("No sound named " + soundName);
            return null;
        }

        /// <summary> Plays a sound on the SoundManager by sound with altered pitch. </summary>
        /// <param name="sound"> Sound to play. </param>
        /// <param name="pitch"> Normalized pitch offset of sound. 1.1 is higer pitch and 0.9 is lower pitch. </param>
        /// <returns> Duration of the sound effect. </returns>
        public static float PlaySound(Sound sound, float pitch)
        {
            /* Find Unused Audio Source */
            AudioSource source = GetAvailableSource();

            /* Play Sound At Default Clip Settings */
            source.volume = sound.volume;
            source.clip = sound.clip;
            source.pitch = pitch;
            source.Play();

            /* Return Clip Duration */
            return source.clip.length;
        }

        /// <summary> Plays a sound on the SoundManager by sound. </summary>
        /// <param name="sound"> Sound to play. </param>
        /// <returns> Duration of the sound effect. </returns>
        public static float PlaySound(Sound sound)
        {
            return PlaySound(sound, 1);
        }

        /// <summary> Plays a sound on the SoundManager by name. </summary>
        /// <param name="name"> Sound by name to play. </param>
        /// <returns> Duration of the sound effect. </returns>
        public static float PlaySound(string name)
        {
            return PlaySound(GetSound(name), 1);
        }

        /// <summary> Plays music on specialized music source. </summary>
        /// <param name="music"> Music to play. </param>
        public static void PlayMusic(Music music)
        {
            if (music != currentMusic)
                instance.StartInterruptableCoroutine(StartMusic(music), ref running);
        }

        /// <summary> Starts music loop at time = 0, and keeps it looping from timeStart to timeEnd. </summary>
        /// <param name="music"> Music to play. </param>
        private static IEnumerator StartMusic(Music music)
        {
            /* Record This Music */
            currentMusic = music;

            /* Adjust Volume Grauually */
            while (musicSource.volume != music.sound.volume)
            {
                musicSource.volume = Mathf.MoveTowards(musicSource.volume, music.sound.volume, Time.deltaTime);
                yield return null;
            }

            /* Start Song At Beginining */
            musicSource.Stop();
            musicSource.clip = music.sound.clip;
            musicSource.time = 0;
            musicSource.Play();

            /* While Coroutine is Running */
            while (music.dyanamicLoop)
            {
                /* Wait Until The End of Loop */
                yield return new WaitUntil(() => musicSource.time >= music.loopEnd);

                /* Jump Back To The Beginning of the Loop */
                musicSource.time = music.loopStart;

            }
        }

        /// <summary> Gradually ends a song. </summary>
        /// <param name="speed"> Speed at which to decrease the volume. </param>
        public static void StopMusic(float speed)
        {
            if (currentMusic != null)
                instance.StartInterruptableCoroutine(EndSong(speed), ref running);
        }

        /// <summary> Stops music gradually. </summary>
        /// <param name="speed"> Speed to change volume at. </param>
        private static IEnumerator EndSong(float speed)
        {
            /* Adjust Volume Grauually */
            while (musicSource.volume != 0)
            {
                musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0, Time.deltaTime * speed);
                yield return null;
            }

            /* Stop Song anc Clear Reference To Music */
            musicSource.Stop();
            currentMusic = null;
        }

        /// <summary> 
        /// Gets an availble audio source. If all are playing clips, gets the source with least duration left.
        /// </summary>
        /// <returns> Available audiosource within SoundManager gameobejct. </returns>
        private static AudioSource GetAvailableSource()
        {
            /* Initialize Default Source and Remaining Duratio */
            AudioSource pendingSource = soundSources[0];
            float lowestRemainingDuration = 1;

            /* For Each Audio Source */
            foreach (AudioSource source in soundSources)
            {
                /* Return Source If It Is NOT Playing Anything */
                if (!source.isPlaying || source.clip == null)
                    return source;

                /* Else (Source Is Playing a Clip) */
                else
                {
                    /* Get The Remaining Duration On This Source's Clip */
                    float remainingDuration = 1 - (source.time / source.clip.length);

                    /* Record This Source For Pending Sound Replacment If The Remaining Duration is Lowest */
                    if (remainingDuration < lowestRemainingDuration)
                    {
                        pendingSource = source;
                        lowestRemainingDuration = remainingDuration;

                    }
                }
            }

            /* If ALL Sources Are Playing Sounds, Return The One With The Lowest Duration Left */
            return pendingSource;
        }
    }
}
