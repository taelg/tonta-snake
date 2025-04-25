using UnityEngine;
using UnityEngine.UI;

public class GameConfigBehavior : MonoBehaviour
{
    [SerializeField] private Button openCloseButton;
    [SerializeField] private GameObject configPanel;

    private void Awake()
    {
        openCloseButton.onClick.AddListener(OnOpenCloseButtonClicked);
    }

    private void OnOpenCloseButtonClicked()
    {
        configPanel.SetActive(!configPanel.activeSelf);
    }

}
