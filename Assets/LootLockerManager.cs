using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class LootLockerManager : MonoBehaviour
{
    private string leaderboardId = "highscore_webgl";
    private bool isSessionStarted = false;

    private void Start()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Sessão iniciada com sucesso!");
                isSessionStarted = true;
            }
            else
            {
                Debug.LogError("Erro ao iniciar sessão: " + response.text);
            }
        });

    }

    public bool IsSessionStarted()
    {
        return isSessionStarted;
    }

    public void LoadScoresFromLootLocker(Action<List<ScoreEntry>> callback)
    {
        LootLockerSDKManager.GetScoreList(leaderboardId, 10, (response) =>
        {
            List<ScoreEntry> scoreList = new List<ScoreEntry>();

            if (response.success)
            {
                foreach (var score in response.items)
                {
                    scoreList.Add(new ScoreEntry(score.member_id, score.score));
                }
                callback?.Invoke(scoreList);
            }
            else
            {
                Debug.LogError("Erro ao carregar pontuações: " + response);
                callback?.Invoke(scoreList); // retorna lista vazia em caso de erro
            }
        });
    }

    public void SaveScoreToLootLocker(string playerName, int score)
    {
        LootLockerSDKManager.SubmitScore(playerName, score, leaderboardId, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Pontuação salva com sucesso!");
            }
            else
            {
                Debug.LogError("Erro ao salvar pontuação: " + response.text);
            }
        });
    }
}
