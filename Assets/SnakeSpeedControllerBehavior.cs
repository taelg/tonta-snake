using UnityEngine;
using UnityEngine.UI;

public class SnakeSpeedControllerBehavior : MonoBehaviour
{
    [SerializeField] private Slider snakeSpeedSlider;
    [SerializeField] private SnakeBehavior snake;

    private void Awake()
    {
        snakeSpeedSlider.onValueChanged.AddListener(OnChangeSnakeSpeed);
    }

    private void OnChangeSnakeSpeed(float value)
    {
        snake.SetMovementSpeed((int)value);
    }

}
