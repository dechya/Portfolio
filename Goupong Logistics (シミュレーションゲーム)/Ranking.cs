using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Ranking : MonoBehaviour
{
    public InputField playerNameInput; // 플레이어 이름 입력 필드
    public Text moneyText; // 점수(돈)를 표시할 텍스트
    public Text[] rankingTexts; // 1, 2, 3등을 표시할 텍스트 배열

    void FixedUpdate()
    {               
        ShowRanking();
    }

    private void UpdateMoneyText(int money)
    {
        moneyText.text = $"Money: {money}";
    }

    public void SaveScore()
    {
        string playerName = playerNameInput.text;
        int playerScore = GameManager.Instance.money;

        List<PlayerScore> rankingList = LoadRankingList();

        rankingList.Add(new PlayerScore(playerName, playerScore));

        rankingList = rankingList.OrderByDescending(ps => ps.Score).Take(10).ToList();

        SaveRankingList(rankingList);
    }

    private List<PlayerScore> LoadRankingList()
    {
        string jsonString = PlayerPrefs.GetString("rankingList", "[]");
        return JsonUtility.FromJson<PlayerScoreList>(jsonString).PlayerScores;
    }

    private void SaveRankingList(List<PlayerScore> rankingList)
    {
        PlayerScoreList playerScoreList = new PlayerScoreList { PlayerScores = rankingList };
        string jsonString = JsonUtility.ToJson(playerScoreList);
        PlayerPrefs.SetString("rankingList", jsonString);
    }

    void ShowRanking()
    {
        List<PlayerScore> rankingList = LoadRankingList().Take(3).ToList(); 

        for (int i = 0; i < rankingTexts.Length; i++) 
        {
            rankingTexts[i].text = "";
        }

        for (int i = 0; i < rankingList.Count; i++) 
        {
            var score = rankingList[i];
            rankingTexts[i].text = $"{i + 1}. {score.PlayerName} - {score.Score}"; 
        }
    }
}

[System.Serializable]
public class PlayerScore
{
    public string PlayerName;
    public int Score;

    public PlayerScore(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}

[System.Serializable]
public class PlayerScoreList
{
    public List<PlayerScore> PlayerScores = new List<PlayerScore>();
}
