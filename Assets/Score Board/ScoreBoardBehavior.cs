using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreBoardBehavior : MonoBehaviour
{
    private const string SCORE_PREFS_KEY = "ScoreList";
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private GameObject scoreEntryPrefab;
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
        scoreList = LoadScores();
        LoadScoreListIntoUI();
    }

    private void OnDestroy()
    {
        SaveScores();
    }

    private List<ScoreEntry> LoadScores()
    {
        if (!PlayerPrefs.HasKey(SCORE_PREFS_KEY)) return new List<ScoreEntry>();

        string json = PlayerPrefs.GetString(SCORE_PREFS_KEY);
        ScoreWrapper wrapper = JsonUtility.FromJson<ScoreWrapper>(json);
        return wrapper.scoreList ?? new List<ScoreEntry>();
    }

    private void LoadScoreListIntoUI()
    {
        foreach (ScoreEntry entry in scoreList)
            AddScoreToUI(entry);
    }

    private void SaveScores()
    {
        ScoreWrapper wrapper = new ScoreWrapper(this.scoreList);
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(SCORE_PREFS_KEY, json);
        PlayerPrefs.Save();
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