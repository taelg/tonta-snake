using UnityEngine;
using UnityEngine.UI;

public class SnakeSpeedControllerBehavior : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField][Range(1, 5)] private int initialSnakeSpeed = 3;
    [SerializeField] private bool initialSnakeAlternativeInput = false;
    [Space]
    [Header("Internal")]
    [SerializeField] private Slider snakeSpeedSlider;
    [SerializeField] private Toggle alternativeInputToggle;
    [SerializeField] private SnakeBehavior snake;

    private void Awake()
    {
        snakeSpeedSlider.value = initialSnakeSpeed;
        alternativeInputToggle.isOn = initialSnakeAlternativeInput;
        snakeSpeedSlider.onValueChanged.AddListener(OnChangeSnakeSpeed);
        alternativeInputToggle.onValueChanged.AddListener(OnChangeAlternativeInput);
    }

    private void Start()
    {
        LoadSnakeSpeedSetting();
        LoadSnakeAlternativeInputSettings();
    }

    private void OnChangeSnakeSpeed(float value)
    {
        snake.SetMovementSpeed((int)value);
        SaveCurrentSnakeSpeed();
    }

    private void OnChangeAlternativeInput(bool value)
    {
        snake.SetAlternativeInput(value);
        SaveCurrentInputMethod();
    }


    private void SaveCurrentSnakeSpeed()
    {
        PlayerPrefs.SetInt("SnakeSpeed", (int)snakeSpeedSlider.value);
        PlayerPrefs.Save();
    }

    private void SaveCurrentInputMethod()
    {
        PlayerPrefs.SetInt("AlternativeInput", alternativeInputToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSnakeSpeedSetting()
    {
        if (!PlayerPrefs.HasKey("SnakeSpeed"))
        {
            snakeSpeedSlider.value = initialSnakeSpeed;
            OnChangeSnakeSpeed(initialSnakeSpeed);
            SaveCurrentSnakeSpeed();
            return;
        }

        snakeSpeedSlider.value = PlayerPrefs.GetInt("SnakeSpeed");
    }
    private void LoadSnakeAlternativeInputSettings()
    {
        if (!PlayerPrefs.HasKey("AlternativeInput"))
        {
            alternativeInputToggle.isOn = initialSnakeAlternativeInput;
            OnChangeAlternativeInput(initialSnakeAlternativeInput);
            SaveCurrentInputMethod();
            return;
        }

        alternativeInputToggle.isOn = PlayerPrefs.GetInt("AlternativeInput") == 1;
    }

}
