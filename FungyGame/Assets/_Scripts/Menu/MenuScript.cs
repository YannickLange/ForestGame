using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour {

    void OnLevelWasLoaded(int level)
    {

        //Highscorescene
        if (level == 1)
        {
            int count = 0;
            GameObject[] highscoreText = GameObject.FindGameObjectsWithTag("HighScoreText");
            Highscore[] scores = HighScores.GetHighScores();
            for (int i = 0; i < 10; i++)
            {
                Text tex = highscoreText[i].GetComponent<Text>();
                if (scores[i].Score != 0)
                    tex.text = scores[i].Name + ": - - : " + scores[i].Score;
                else
                {
                    tex.text = "";
                    count++;
                }
            }
            if (count == 10)
                highscoreText[0].GetComponent<Text>().text = "No highscores yet";
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
