using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {

    public float TimeLength = 60 * 4;

    float timeTheGameIsRunning;
    UnityEngine.UI.Text text;

	void Awake () {
        timeTheGameIsRunning = 0;
        text = GetComponent<UnityEngine.UI.Text>();
	}
	
	void Update () {
        timeTheGameIsRunning += Time.deltaTime;
        text.text = (TimeLength - timeTheGameIsRunning).ToString("0") + "s left";
        if (timeTheGameIsRunning > TimeLength)
        {
            HighScores.SaveHighScore("You finished with score: ", (int)GridManager.instance.Meter.Score);
            Application.LoadLevel(1);
        }
	}
}
