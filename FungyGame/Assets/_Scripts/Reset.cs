using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    public void PressReset()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
