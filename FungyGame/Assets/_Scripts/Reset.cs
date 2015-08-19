using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

    public void PressReset()
    {
        Application.LoadLevel(0);
    }
}
