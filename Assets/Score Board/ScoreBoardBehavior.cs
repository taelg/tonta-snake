using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreBoardBehavior : MonoBehaviour
{
    [SerializeField] private LootLockerManager lootLocker;
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private GameObject scoreLoadingObject;
    private List<ScoreEntry> scoreList = new List<ScoreEntry>();
    private ScoreEntry lastAddedScoreEntry = null;

    private void Start()
    {
        StartCoroutine(LoadScoresFromLootLocker());
    }

    private IEnumerator LoadScoresFromLootLocker()
    {
        scoreLoadingObject.SetActive(true);
        ClearAllScoresFromUI();
        yield return new WaitUntil(() => lootLocker.IsSessionStarted());
        lootLocker.LoadScoresFromLootLocker(LoadScoresIntoUI);
    }

    private void ClearAllScoresFromUI()
    {
        foreach (Transform child in scoreContainer.transform)
        {
            if (child != scoreContainer.transform)
                Destroy(child.gameObject);
            else
                Debug.Log("Viu como precisa desse if?");
        }
    }

    private void LoadScoresIntoUI(List<ScoreEntry> scoreList)
    {
        scoreLoadingObject.SetActive(false);
        foreach (ScoreEntry entry in scoreList)
        {
            bool isTheLastAddedScore = entry.name == lastAddedScoreEntry?.name && entry.score == lastAddedScoreEntry?.score;
            AddScoreToUI(entry, isTheLastAddedScore);
        }
    }

    /// <summary>
    /// Return true if the score was added to the list and false if it was not high enough to dethrone another.
    /// </summary>
    public bool TryAddNewScore(string name, int score)
    {
        ScoreEntry newEntry = new ScoreEntry(name, score);

        if (IsWithinTop20Scores(newEntry))
        {
            lastAddedScoreEntry = newEntry;
            AddNewScore(newEntry);
            return true;
        }

        return false;
    }

    private bool IsWithinTop20Scores(ScoreEntry newEntry)
    {
        if (scoreList.Count < 20) return true;

        int lowestScore = scoreList.Last().score;
        return newEntry.score > lowestScore;
    }

    private void AddNewScore(ScoreEntry newEntry)
    {
        lootLocker.SaveScoreToLootLocker(newEntry.name, newEntry.score, SaveScoreToLootLockerCallback);
    }

    private void SaveScoreToLootLockerCallback(bool success)
    {
        StartCoroutine(LoadScoresFromLootLocker());
    }

    private void AddScoreToUI(ScoreEntry entry, bool animate = false)
    {
        ScoreBoardEntryBehavior scoreEntry = Instantiate(scoreEntryPrefab, scoreContainer.transform).GetComponent<ScoreBoardEntryBehavior>();
        scoreEntry.SetInfo(entry.name, entry.score);

        if (animate)
        {
            scoreEntry.AnimateOnEnter();
            lastAddedScoreEntry = null;
        }
    }

}

[System.Serializable]
public class ScoreWrapper
{
    public List<ScoreEntry> scoreList;

    public ScoreWrapper(List<ScoreEntry> scoreList)
    {
        this.scoreList = scoreList;
    }
}

[System.Serializable]
public class ScoreEntry
{
    public string name;
    public int score;

    public ScoreEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}