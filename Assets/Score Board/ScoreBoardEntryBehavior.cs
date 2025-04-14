using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardEntryBehavior : MonoBehaviour
{
    [SerializeField] private Image background;

    private Color highlighted = new Color(1, 0, 0, 0.3f);

    public int score = 0;

    [SerializeField] private TMP_Text textLabel;

    public void SetInfo(string name, int score)
    {
        this.score = score;
        textLabel.text = $"{name} - {score}";
    }

    public void AnimateOnEnter()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Color originalColor = background.color;
        float duration = 0.8f;
        float flashDuration = 0.1f;
        int flashCount = Mathf.RoundToInt(duration / (flashDuration * 2));

        for (int i = 0; i < flashCount; i++)
        {
            background.color = highlighted;
            yield return new WaitForSeconds(flashDuration);

            background.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        background.color = originalColor;
    }

}
