using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour {

	private GameObject Indicator;

	// Use this for initialization
	void Start () {
		Indicator = this.gameObject.transform.GetChild(0).gameObject;
	}

	public void Forest (float dmg = 2) {
		Indicator.transform.position = new Vector2(Indicator.transform.position.x - dmg, 
		                                           Indicator.transform.position.y);
	}

	public void Fungus (float dmg = 2) {
		Indicator.transform.position = new Vector2(Indicator.transform.position.x + dmg,
		                                           Indicator.transform.position.y);
	}

	void Update () {
		if(Indicator.transform.position.x <= -100 && Indicator.transform.position.y >= 100) {
			//GameOver ();
		}
	}
}
