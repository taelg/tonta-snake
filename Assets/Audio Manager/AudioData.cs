using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "ScriptableObjects/Audio Data", order = 1)]
public class AudioData : ScriptableObject
{
    [SerializeField] private List<AudioDataEntry> entries;

    private Dictionary<AudioId, List<AudioClip>> audioClips;

    private void OnEnable()
    {
        LoadAudioClips();
    }

    private void LoadAudioClips()
    {
        audioClips = new Dictionary<AudioId, List<AudioClip>>();
        foreach (var entry in entries)
            audioClips.Add(entry.audioId, entry.clips);
    }

    public List<AudioClip> GetClips(AudioId audioId)
    {
        return audioClips.TryGetValue(audioId, out var clips) ? clips : null;
    }


    [System.Serializable]
    public class AudioDataEntry
    {
        public AudioId audioId;
        public List<AudioClip> clips;
    }
}
