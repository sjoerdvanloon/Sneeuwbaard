using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Version 1.1
/// 1.0 ShootThemAgainInTheBody version 
/// 1.1 Changes for DeliveryDraiver: 
///     - use Player tag instead of Player type
///     - made properties  private and SerializeField
/// </summary>
[RequireComponent(typeof(SoundLibrary))]
public class AudioManager : MonoBehaviour
{
    public enum Channel { Master, Sfx, Music }
    public static AudioManager Instance;

    [SerializeField] float _masterVolumePercent = 1f;
    [SerializeField] float _sfxVolumePercent = 1f;
    [SerializeField] float _musicVolumePercent = 1f;
    [SerializeField] bool _logPlaySound = false;

    Transform _playerTransform;
    AudioSource sfx2dSource;
    AudioSource[] MusicSources;
    int activeMusicSourceIndex = 0;
    Transform audioListener;
    SoundLibrary soundLibrary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Singletons
            DontDestroyOnLoad(gameObject); // Let it live between scenes

            soundLibrary = GetComponent<SoundLibrary>();

            MusicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                var newMusicSource = new GameObject($"Music source {i + 1}");
                newMusicSource.transform.parent = transform;
                MusicSources[i] = newMusicSource.AddComponent<AudioSource>();
            }

            var newSfx2dSource = new GameObject("2d sfx source");
            newSfx2dSource.transform.parent = transform;
            sfx2dSource = newSfx2dSource.AddComponent<AudioSource>();

            audioListener = FindObjectOfType<AudioListener>().transform;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _playerTransform = player.transform;

            _masterVolumePercent = PlayerPrefs.GetFloat("master vol", _masterVolumePercent);
            _sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", _sfxVolumePercent);
            _musicVolumePercent = PlayerPrefs.GetFloat("music vol", _musicVolumePercent);
        }
        else
        {
            Destroy(gameObject);

        }
    }

    void Update()
    {
        if (_playerTransform != null)
        {
            audioListener.position = _playerTransform.position;
        }
    }

    public void SetVolume(Channel channel, float volumePercent)
    {
        switch (channel)
        {
            case Channel.Master:
                _masterVolumePercent = volumePercent;
                break;
            case Channel.Sfx:
                _sfxVolumePercent = volumePercent;
                break;
            case Channel.Music:
                _musicVolumePercent = volumePercent;
                break;
            default:
                break;
        }

        MusicSources[0].volume = _musicVolumePercent * _masterVolumePercent;
        MusicSources[1].volume = _musicVolumePercent * _masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", _masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", _sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", _musicVolumePercent);
        PlayerPrefs.Save();

    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 2)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex; // Flip between the two;

        var activeMusicSource = MusicSources[activeMusicSourceIndex];
        activeMusicSource.clip = clip;
        activeMusicSource.Play();

        StartCoroutine(AnimateMusicCrossFade(fadeDuration));

    }


    IEnumerator AnimateMusicCrossFade(float duration)
    {
        float percent = 0;
        var activeMusicSource = MusicSources[activeMusicSourceIndex];
        var deactiveMusicSource = MusicSources[1 - activeMusicSourceIndex];

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            var volume = _musicVolumePercent * _masterVolumePercent;
            activeMusicSource.volume = Mathf.Lerp(0, volume, percent);
            deactiveMusicSource.volume = Mathf.Lerp(volume, 0, percent);
            yield return null;
        }
    }

    /// <summary>
    /// Play a sound at a specific position in 3d space
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="position"></param>
    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            return;
        }

        var volume = _sfxVolumePercent * _masterVolumePercent;

        if (_logPlaySound)
            print($"Play sound {clip.name} at {volume * 100}%");
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    /// <summary>
    /// Play a sound at a specific position in 3d space
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="position"></param>
    public void PlaySound(string soundName, Vector3 position)
    {
        var sound = soundLibrary.GetAudioClip(soundName);
        if (sound != null)
        {
            PlaySound(sound, position);
        }
    }

    /// <summary>
    /// Play a 2d sound
    /// </summary>
    /// <param name="soundName"></param>
    public void PlaySound(string soundName)
    {
        var sound = soundLibrary.GetAudioClip(soundName);
        if (sound != null)
        {
            var volume = _sfxVolumePercent * _masterVolumePercent;

            if (_logPlaySound)
                Debug.Log($"Play 2d sound {sound.name} at {volume * 100}%");

            sfx2dSource.PlayOneShot(sound, _sfxVolumePercent * _masterVolumePercent);
        }
    }
}