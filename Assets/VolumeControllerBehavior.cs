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
        musicVolumeSlider.value = 0.1f;
        effectsVolumeSlider.value = 0.35f;
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetAudioTypeVolume(AudioType.MUSIC, value);
    }

    private void OnEffectsVolumeChanged(float value)
    {
        AudioManager.Instance.SetAudioTypeVolume(AudioType.EFFECT, value);
    }


}
