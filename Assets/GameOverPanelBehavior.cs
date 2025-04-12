using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelBehavior : MonoBehaviour
{

    [SerializeField] private TMP_Text sizeLabel;
    [SerializeField] private Button retryButton;
    [SerializeField] private SnakeBehavior snake;
    [SerializeField] private GameGridBehavior grid;

    private void Start()
    {
        retryButton.onClick.AddListener(OnClickRetry);
    }

    public void ShowFinalScore(int snakeSize)
    {
        this.gameObject.SetActive(true);
        sizeLabel.text = snakeSize.ToString();
    }

    private void OnClickRetry()
    {
        grid.ResetGrid();
        snake.ResetSnake();
        this.gameObject.SetActive(false);
    }


}
