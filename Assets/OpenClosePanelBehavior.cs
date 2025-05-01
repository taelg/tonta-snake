using UnityEngine;
using UnityEngine.UI;

public class OpenClosePanelBehavior : MonoBehaviour
{
    [SerializeField] private Button[] openCloseButton;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        InitializeButtonsBehavior();
        panel.SetActive(true);
    }

    private void Start()
    {
        panel.SetActive(false);
    }

    private void InitializeButtonsBehavior()
    {
        foreach (var button in openCloseButton)
            button.onClick.AddListener(OnOpenCloseButtonClicked);
    }

    private void OnOpenCloseButtonClicked()
    {
        panel.SetActive(!panel.activeSelf);
    }

}
