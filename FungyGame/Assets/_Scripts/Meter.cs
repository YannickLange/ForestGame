using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour {

	private GameObject Indicator;

	// Use this for initialization
	void Start () {
		Indicator = this.gameObject.transform.GetChild(0).gameObject;
	}
    
    public void Forest () {
        Forest(2f);
    }
	public void Forest (float dmg) {
		Indicator.transform.position = new Vector2(Indicator.transform.position.x - dmg, 
		                                           Indicator.transform.position.y);
	}
    
    public void Fungus () {
        Fungus(2f);
    }
	public void Fungus (float dmg) {
		Indicator.transform.position = new Vector2(Indicator.transform.position.x + dmg,
		                                           Indicator.transform.position.y);
	}

	void Update () {
		if(Indicator.transform.position.x <= -100 && Indicator.transform.position.y >= 100) {
			//GameOver ();
		}
	}
}
