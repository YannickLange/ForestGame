using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

	void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "GameOver"))
            PressReset();
    }

    public void PressReset()
    {
        Application.LoadLevel(0);
    }
}
