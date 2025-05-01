using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreLabelBehavior : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Must reflect how long the PointFX take to hit the target.")][SerializeField] private float scoreUpdateDelay = 0.5f;
    [Space]
    [Header("Internal")]
    [SerializeField] private TMP_Text scoreLabel;

    private void Start()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        scoreLabel.text = "0";
    }

    public void UpdateScoreDelayed(int score)
    {
        StartCoroutine(UpdateScoreCoroutine(score));
    }

    private IEnumerator UpdateScoreCoroutine(int score)
    {
        // Simulate a score update delay
        yield return new WaitForSeconds(scoreUpdateDelay);
        scoreLabel.text = score.ToString();
    }

}
