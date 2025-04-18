using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelBehavior : MonoBehaviour
{

    [SerializeField] private TMP_Text sizeLabel;
    [SerializeField] private Button retryButton;
    [SerializeField] private SnakeBehavior snake;
    [SerializeField] private GameGridBehavior grid;
    [SerializeField] private SaveScoreBehavior saveScorePanel;

    private void Start()
    {
        retryButton.onClick.AddListener(OnClickRetry);
    }

    public void ShowFinalScore(int snakeSize)
    {
        this.gameObject.SetActive(true);
        sizeLabel.text = snakeSize.ToString();
        saveScorePanel.gameObject.SetActive(true);
        saveScorePanel.SetScore(snakeSize);
    }

    private void OnClickRetry()
    {
        grid.ClearGrid();
        snake.ResetSnake();
        this.gameObject.SetActive(false);
    }


}
