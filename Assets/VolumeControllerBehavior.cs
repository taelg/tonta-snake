using UnityEngine;
using UnityEngine.UI;

public class VolumeControllerBehavior : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;

    private void Awake()
    {
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
    }

    private void Start()
    {
        musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        effectsVolumeSlider.value = AudioManager.Instance.GetEffectsVolume();
        LoadSettingFromPlayerPrefs();

    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetAudioTypeVolume(AudioType.MUSIC, value);
        SaveCurrentSettings();
    }

    private void OnEffectsVolumeChanged(float value)
    {
        AudioManager.Instance.SetAudioTypeVolume(AudioType.EFFECT, value);
        SaveCurrentSettings();
    }

    private void SaveCurrentSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolumeSlider.value);
        PlayerPrefs.Save();
    }

    private void LoadSettingFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("MusicVolume") || !PlayerPrefs.HasKey("EffectsVolume"))
            return;

        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        effectsVolumeSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
    }


}
