using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour
{

    private GameObject Indicator;

    public Reset r { get; set; }

    void Awake()
    {
        r = GetComponent<Reset>();
    }
    // Use this for initialization
    void Start ()
    {
        Indicator = this.gameObject.transform.GetChild(0).gameObject;
    }

    public void Forest (float dmg)
    {
        Indicator.transform.position = new Vector2(Indicator.transform.position.x + dmg, 
                                                   Indicator.transform.position.y);
    }

    public void Fungus (float dmg)
    {
        Indicator.transform.position = new Vector2(Indicator.transform.position.x - dmg,
                                                   Indicator.transform.position.y);
    }

    void Update ()
    {
        // GameOver
        if(Indicator.transform.position.x <= -100 && Indicator.transform.position.y >= 100) {
            //GameOver ();
            GridManager.instance.ResetButton.PressReset();
        }

        /*// worst ending
        if(Indicator.transform.position.x > -100 && Indicator.transform.position.y < -80 || 
           Indicator.transform.position.x > 80 && Indicator.transform.position.y < 100) {
            
        }

        // semi ending
        if(Indicator.transform.position.x > -80 && Indicator.transform.position.y < -10 || 
           Indicator.transform.position.x > 10 && Indicator.transform.position.y < 80) {
            
        }

        // good ending
        if(Indicator.transform.position.x > -10 && Indicator.transform.position.y < 10) {
            
        }*/
    }
}
