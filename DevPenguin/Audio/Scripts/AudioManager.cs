using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DevPenguin.Utilities
{
    public class AudioManager : MonoBehaviour
    {
        #region Declarations
        public static AudioManager instance;

        [Header("Background Music")]
        [SerializeField] AudioMixerGroup musicAudioMixer;
        [SerializeField] string musicVolumeParameterName = "MusicVolume";
        [Range(-88, 10)]
        [SerializeField] float musicMaxVolume = -10;
        [Range(-88, 10)]
        [SerializeField] float musicMinVolume = -88;
        [SerializeField] float musicStartPitch = 1;
        [SerializeField] SoundMusic[] soundMusics;
        [Space(2f)]

        [Header("Sound Effects")]
        [SerializeField] string sfxVolumeParameterName = "SfxVolume";
        [SerializeField] AudioMixerGroup sfxAudioMixer;
        [Range(-88, 10)]
        [SerializeField] float maxSfxVolume = 0;
        [Range(-88, 10)]
        [SerializeField] private float minSfxVolume = -88;
        [SerializeField] SoundEffect[] soundEffects;
        [Space(2f)]

        private bool _isMusicOn;
        private bool _isSfxOn;
        private AudioSource _musicAudioSource;
        private List<AudioSource> _sfxAudioSourceList;
        #endregion

        #region Getters
        public bool IsMusicOn => _isMusicOn;
        public bool IsSfxOn => _isSfxOn;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Singleton pattern.
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update.
        private void Start()
        {
            // Set up sound effects.
            SetupSfx();
            SetupMusics();
            UpdateMusicPitch(musicStartPitch);
        }
        #endregion

        #region Helper Methods

        #region Music
        public void PlayMusic(string musicName)
        {
            if (_musicAudioSource.clip)
            {
                if (_musicAudioSource.clip.name == musicName)
                    return;
            }

            for (int i = 0; i < soundMusics.Length; i++)
            {
                if (soundMusics[i].name.Equals(musicName))
                {
                    _musicAudioSource.Stop();
                    _musicAudioSource.volume = soundMusics[i].startVolume;
                    _musicAudioSource.clip = soundMusics[i].audioClip;
                    _musicAudioSource.Play();
                    break;
                }
            }
        }

        public void PauseMusic()
        {
            if (_musicAudioSource.isPlaying)
                _musicAudioSource.Stop();
        }

        public void ResumeMusic()
        {
            if (!_musicAudioSource.isPlaying)
                _musicAudioSource.Play();
        }

        public void RestartMusic()
        {
            _musicAudioSource.time = 0;
        }

        public void UpdateMusicPitch(float pitch)
        {
            _musicAudioSource.pitch = pitch;
        }

        public void SetIsmusicOn(bool isOn)
        {
            _isMusicOn = isOn;
            musicAudioMixer.audioMixer.SetFloat(musicVolumeParameterName, (isOn) ? musicMaxVolume : musicMinVolume);
        }

        private void SetupMusics()
        {
            GameObject _newMusicAudioSource = new GameObject();
            _newMusicAudioSource.transform.parent = transform;
            _newMusicAudioSource.AddComponent<AudioSource>();
            _musicAudioSource = _newMusicAudioSource.GetComponent<AudioSource>();
            _musicAudioSource.playOnAwake = true;
            _musicAudioSource.name = "MusicAudioSource";
            _musicAudioSource.loop = true;
            _musicAudioSource.outputAudioMixerGroup = musicAudioMixer;
            PlayMusic(soundMusics[0].name);
        }
        #endregion

        #region SFX
        public void PlaySfx(string effectName)
        {
            try
            {
                for (int i = 0; i < soundEffects.Length; i++)
                {
                    if (soundEffects[i].name.Equals(effectName))
                    {
                        if (!_sfxAudioSourceList[i].isPlaying)
                            _sfxAudioSourceList[i].Play();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e + " " + effectName);
            }
        }

        public void SetIsSfxOn(bool isOn)
        {
            _isSfxOn = isOn;
            musicAudioMixer.audioMixer.SetFloat(sfxVolumeParameterName, (isOn) ? maxSfxVolume : minSfxVolume);
        }

        private void SetupSfx()
        {
            _sfxAudioSourceList = new List<AudioSource>();
            for (int i = 0; i < soundEffects.Length; i++)
            {
                GameObject _newSfxAudioSource = new GameObject();
                _newSfxAudioSource.transform.parent = transform;
                _newSfxAudioSource.AddComponent<AudioSource>();
                _sfxAudioSourceList.Add(_newSfxAudioSource.GetComponent<AudioSource>());
                _sfxAudioSourceList[i].name = soundEffects[i].name + "AudioSource";
                _sfxAudioSourceList[i].playOnAwake = soundEffects[i].shouldPlayOnAwake;
                _sfxAudioSourceList[i].clip = soundEffects[i].audioClip;
                _sfxAudioSourceList[i].loop = soundEffects[i].doesLoop;
                _sfxAudioSourceList[i].volume = soundEffects[i].startVolume;
                _sfxAudioSourceList[i].outputAudioMixerGroup = sfxAudioMixer;
            }
        }
        #endregion

        #endregion
    }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip audioClip;
        public bool shouldPlayOnAwake;
        public bool doesLoop;
        public float startVolume = 0.5f;
    }

    [System.Serializable]
    public class SoundMusic
    {
        public string name;
        public AudioClip audioClip;
        public float startVolume = 0.5f;
    }
}
