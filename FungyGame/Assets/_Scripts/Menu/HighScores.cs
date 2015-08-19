using UnityEngine;
using System.Collections;

public static class HighScores {

    public static void SaveHighScore(string name, int score)
    {
        Highscore newScore, oldScore;
        newScore.Score = score;
        newScore.Name = name;

        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey(i + "HScore"))
            {
                if (PlayerPrefs.GetInt(i + "HScore") < newScore.Score)
                {
                    // new Score is higher than the stored Score
                    oldScore.Score = PlayerPrefs.GetInt(i + "HScore");
                    oldScore.Name = PlayerPrefs.GetString(i + "HScoreName");
                    PlayerPrefs.SetInt(i + "HScore", newScore.Score);
                    PlayerPrefs.SetString(i + "HScoreName", newScore.Name);
                    newScore.Score = oldScore.Score;
                    newScore.Name = oldScore.Name;
                }
            }
            else
            {
                PlayerPrefs.SetInt(i + "HScore", newScore.Score);
                PlayerPrefs.SetString(i + "HScoreName", newScore.Name);
                newScore.Score = 0;
                newScore.Name = "";
            }
        }
    } 

    public static Highscore[] GetHighScores()
    {
        Highscore[] scores = new Highscore[10];
        for (int i = 0; i < 10; i++)
        {
            scores[i].Score = PlayerPrefs.GetInt(i + "HScore");
           scores[i].Name = PlayerPrefs.GetString(i + "HScoreName");
        }
        return scores;
    }
}


public struct Highscore
{
    public int Score;
    public string Name;
}
