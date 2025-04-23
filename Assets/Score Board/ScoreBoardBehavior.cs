using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreBoardBehavior : MonoBehaviour
{
    private const string SCORE_PREFS_KEY = "ScoreList";
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private LootLockerManager lootLocker;
    private List<ScoreEntry> scoreList = new List<ScoreEntry>();

    /// <summary>
    /// Return true if the score was added to the list and false if it was not high enough to dethrone another.
    /// </summary>
    public bool AddNewScore(string name, int score)
    {
        ScoreEntry newEntry = new ScoreEntry(name, score);

        if (CanAddScore(newEntry))
        {
            InsertScore(newEntry);
            lootLocker.SaveScoreToLootLocker(name, score);
            return true;
        }

        return false;
    }

    private bool CanAddScore(ScoreEntry newEntry)
    {
        if (scoreList.Count < 20) return true;

        int lowestScore = scoreList.Last().score;
        return newEntry.score > lowestScore;
    }

    private void InsertScore(ScoreEntry newEntry)
    {
        scoreList.Add(newEntry);
        SortScoreList();

        if (scoreList.Count > 20)
            RemoveLowestScore();

        int scoreIndex = scoreList.IndexOf(newEntry);
        AddScoreToUI(newEntry, scoreIndex);
    }

    private void SortScoreList()
    {
        scoreList = scoreList.OrderByDescending(entry => entry.score).ToList();
    }

    private void RemoveLowestScore()
    {
        scoreList.RemoveAt(scoreList.Count - 1);
        RemoveLastScoreFromUI();
    }

    private void AddScoreToUI(ScoreEntry entry, int scoreIndex = -1)
    {
        ScoreBoardEntryBehavior scoreEntry = Instantiate(scoreEntryPrefab, scoreContainer.transform).GetComponent<ScoreBoardEntryBehavior>();
        scoreEntry.SetInfo(entry.name, entry.score);

        if (scoreIndex != -1)
        {
            scoreEntry.transform.SetSiblingIndex(scoreIndex);
            scoreEntry.AnimateOnEnter();
        }
    }

    private void RemoveLastScoreFromUI()
    {
        Transform lastChild = scoreContainer.transform.GetChild(scoreContainer.transform.childCount - 1);
        Destroy(lastChild.gameObject);
    }

    private void Start()
    {
        StartCoroutine(LoadScoresFromLootLOcker());
    }

    private IEnumerator LoadScoresFromLootLOcker()
    {
        yield return new WaitUntil(() => lootLocker.IsSessionStarted());
        lootLocker.LoadScoresFromLootLocker(LoadScoreListIntoUICallback);
    }

    private void LoadScoreListIntoUICallback(List<ScoreEntry> scoreList)
    {
        foreach (ScoreEntry entry in scoreList)
            AddScoreToUI(entry);
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