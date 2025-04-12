using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveScoreBehavior : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Text errorLabel;
    [SerializeField] private ScoreBoardBehavior scoreBoard;
    protected int score = 0;

    private void Start()
    {
        saveButton.onClick.AddListener(OnClickSave);
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    private void OnEnable()
    {
        errorLabel.gameObject.SetActive(false);
    }

    private void OnClickSave()
    {
        if (inputField.text == "")
        {
            errorLabel.text = "the name cannot be empty.";
            errorLabel.gameObject.SetActive(true);
            return;
        }

        if (inputField.text.Length < 3)
        {
            errorLabel.text = "the min name lenght is 3";
            errorLabel.gameObject.SetActive(true);
            return;
        }

        if (inputField.text.Length > 15)
        {
            errorLabel.text = "the max name lenght is 15";
            errorLabel.gameObject.SetActive(true);
            return;
        }

        errorLabel.gameObject.SetActive(false);
        scoreBoard.AddNewScore(inputField.text, score);
        this.gameObject.SetActive(false);
    }

}