using UnityEngine;
using UnityEngine.UI;

public class SnakeSpeedControllerBehavior : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField][Range(1, 5)] private int initialSnakeSpeed = 3;
    [Space]
    [Header("Internal")]
    [SerializeField] private Slider snakeSpeedSlider;
    [SerializeField] private SnakeBehavior snake;

    private void Awake()
    {
        snakeSpeedSlider.value = 3;
        snakeSpeedSlider.onValueChanged.AddListener(OnChangeSnakeSpeed);
    }

    private void Start()
    {
        LoadSettingFromPlayerPrefs();
    }

    private void OnChangeSnakeSpeed(float value)
    {
        snake.SetMovementSpeed((int)value);
        SaveCurrentSettings();
    }
    private void SaveCurrentSettings()
    {
        PlayerPrefs.SetInt("SnakeSpeed", (int)snakeSpeedSlider.value);
        PlayerPrefs.Save();
    }
    private void LoadSettingFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("SnakeSpeed"))
        {
            snakeSpeedSlider.value = initialSnakeSpeed;
            OnChangeSnakeSpeed(initialSnakeSpeed);
            SaveCurrentSettings();
            return;
        }

        snakeSpeedSlider.value = PlayerPrefs.GetInt("SnakeSpeed");
    }

}
