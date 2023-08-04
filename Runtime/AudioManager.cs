using UnityEngine;

namespace Relu.Utils
{
    public class AudioManager : Singleton<AudioManager>
    {
        private AudioSource musicSource;
        private AudioSource[] sfxSources;

        private float masterVolume = 1.0f;
        private float musicVolume = 1.0f;
        private float sfxVolume = 1.0f;

        private void Start()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            // Create multiple audio sources for sound effects to allow simultaneous playback.
            int maxSfxSources = 10;
            sfxSources = new AudioSource[maxSfxSources];
            for (int i = 0; i < maxSfxSources; i++)
            {
                sfxSources[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayMusic(AudioClip musicClip)
        {
            musicSource.clip = musicClip;
            musicSource.volume = masterVolume * musicVolume;
            musicSource.Play();
        }

        public void PlaySoundEffect(AudioClip sfxClip)
        {
            foreach (AudioSource source in sfxSources)
            {
                if (!source.isPlaying)
                {
                    source.clip = sfxClip;
                    source.volume = masterVolume * sfxVolume;
                    source.Play();
                    return;
                }
            }
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolume();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolume();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            musicSource.volume = masterVolume * musicVolume;
            foreach (AudioSource source in sfxSources)
            {
                source.volume = masterVolume * sfxVolume;
            }
        }
    }
}
