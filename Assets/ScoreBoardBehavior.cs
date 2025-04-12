using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreBoardBehavior : MonoBehaviour
{
    private const string SCORE_PREFS_KEY = "ScoreList";
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private GameObject scoreEntryPrefab;
    private List<ScoreEntry> scoreList = new List<ScoreEntry>();

    public void AddNewScore(string name, int score)
    {
        ScoreEntry entry = new ScoreEntry(name, score);
        scoreList.Add(entry);
        scoreList = scoreList.OrderByDescending(entry => entry.score).ToList();
        int scoreIndex = scoreList.IndexOf(entry);
        AddScoreToUI(entry, scoreIndex);
    }

    private void AddScoreToUI(ScoreEntry entry, int scoreIndex = -1)
    {
        ScoreBoardEntryBehavior scoreEntry = Instantiate(scoreEntryPrefab, scoreContainer.transform).GetComponent<ScoreBoardEntryBehavior>();
        scoreEntry.SetInfo(entry.name, entry.score);
        scoreEntry.AnimateOnEnter();

        if (scoreIndex != -1)
            scoreEntry.transform.SetSiblingIndex(scoreIndex);
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