using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSourcePoolBehavior musicSourcePool;
    [SerializeField] private AudioSourcePoolBehavior effectsSourcePool;
    [SerializeField] private AudioData audioData;

    private float musicVolume = 1f;
    private float effectsVolume = 1f;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        AwakeSingleton();
        StartGameMusic();
    }

    private void AwakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
            Destroy(this.gameObject);
    }

    private void StartGameMusic()
    {
        musicVolume = 0.1f;
        effectsVolume = 0.30f;
        PlayOnLoop(AudioId.GAME_MUSIC, AudioType.MUSIC);
    }

    public void PlayOnLoop(AudioId audioId, AudioType audioType)
    {
        AudioSource audioSource = GetPoolByType(audioType).GetNext();
        List<AudioClip> audioClips = audioData.GetClips(audioId);
        AudioClip selectedClip = audioClips[Random.Range(0, audioClips.Count)];
        UpdateAudioSourceVolume(audioSource, audioType);
        audioSource.loop = true;
        audioSource.clip = selectedClip;
        audioSource.Play();

    }

    public void PlayOneShot(AudioId audioId, AudioType audioType)
    {
        AudioSource audioSource = GetPoolByType(audioType).GetNext();
        List<AudioClip> audioClips = audioData.GetClips(audioId);
        AudioClip selectedClip = audioClips[Random.Range(0, audioClips.Count)];
        UpdateAudioSourceVolume(audioSource, audioType);
        audioSource.PlayOneShot(selectedClip);
        StartCoroutine(DisableAudioSourceAfterPlayback(audioSource, selectedClip.length));
    }

    public void SetAudioTypeVolume(AudioType audioType, float volume)
    {
        if (audioType == AudioType.MUSIC)
            musicVolume = volume;
        else if (audioType == AudioType.EFFECT)
            effectsVolume = volume;
        else
            Debug.LogError($"AudioManager: Invalid AudioType {audioType}");

        UpdateAllAudioSourcesVolume(audioType, volume);
    }

    private void UpdateAllAudioSourcesVolume(AudioType audioType, float volume)
    {
        foreach (AudioSource source in GetPoolByType(audioType).GetAllObjects())
            source.volume = volume;
    }

    private IEnumerator DisableAudioSourceAfterPlayback(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.gameObject.SetActive(false);
    }

    private void UpdateAudioSourceVolume(AudioSource audioSource, AudioType audioType)
    {
        if (audioType == AudioType.MUSIC)
            audioSource.volume = musicVolume;
        else if (audioType == AudioType.EFFECT)
            audioSource.volume = effectsVolume;
        else
            Debug.LogError($"AudioManager: Invalid AudioType {audioType}");
    }

    private AudioSourcePoolBehavior GetPoolByType(AudioType audioType)
    {
        if (audioType == AudioType.MUSIC)
            return musicSourcePool;
        else if (audioType == AudioType.EFFECT)
            return effectsSourcePool;

        Debug.LogError($"AudioManager: Invalid AudioType {audioType}");
        return null;
    }



}
