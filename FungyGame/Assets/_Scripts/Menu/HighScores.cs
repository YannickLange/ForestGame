using UnityEngine;
using System.Collections;

public static class HighScores
{
    private static string getKey(int i)
    {
        return i + "HScore";
    }

    public static void SaveHighScore(int score)
    {
        var highScores = GetHighScores();
        for (int i = 0; i < 10; i++)
        {
            if (highScores [i] <= score)
            {
                Debug.Log(highScores [i] + " < " + score);
                for (int j = 9; j > i; j--)
                {
                    Debug.Log(j + " <-- " + (j - 1) + " (" + highScores [j] + "<---" + highScores [j - 1] + ")");
                    highScores [j] = highScores [j - 1];
                }
                highScores [i] = score;
                break;
            }
        }
        SetHighScores(highScores);
        PlayerPrefs.SetInt("LastHighscore", score);
    }

    public static int[] GetHighScores()
    {
        int[] scores = new int[10];
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey(getKey(i)))
            {
                scores [i] = PlayerPrefs.GetInt(getKey(i));
                Debug.Log("score=" + scores [i]);
            } else
            {
                scores [i] = 0;
            }
        }
        Debug.Log(scores);
        return scores;
    }
    
    public static void SetHighScores(int[] highscore)
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey(getKey(i));
        }
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt(getKey(i), highscore [i]);
        }
    }
}
