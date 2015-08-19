using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour
{

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("level was loaded");
        //Highscorescene
        if (level == 1)
        {
            int lastScore = 0;
            if (PlayerPrefs.HasKey("LastHighscore"))
            {
                lastScore = PlayerPrefs.GetInt("LastHighscore");
            }

            int count = 0;
            int[] scores = HighScores.GetHighScores();
            for (int i = 9; i >= 0; i--)
            {
                Text tex = GameObject.Find("Highscore" + (i + 1)).GetComponent<Text>();
                if (scores [i] != 0)
                {
                    if (lastScore == scores [i])
                    {
                        lastScore = 0;
                        tex.text = scores [i].ToString() + " <---";
                    } else
                    {
                        tex.text = scores [i].ToString();
                    }
                } else
                {
                    tex.text = "-";
                    count++;
                }
            }
            if (count == 10)
                GameObject.Find("Highscore1").GetComponent<Text>().text = "No highscores yet";
        }
    }

    public void Startbutton()
    {
        Application.LoadLevel(2);
    }

    public void HighScoresButton()
    {
        Application.LoadLevel(1);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        Application.LoadLevel(0);
    }
}
