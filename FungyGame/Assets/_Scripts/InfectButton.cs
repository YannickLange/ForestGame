using UnityEngine;
using System.Collections;

public class InfectButton : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
	
    }
    // Update is called once per frame
    void Update()
    {
	
    }

    public delegate void ButtonEventHandler();

    public event ButtonEventHandler ClickEvent;

    public void OnButtonClicked()
    {
        if (ClickEvent != null)
            ClickEvent();
    }
}
