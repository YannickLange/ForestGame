using UnityEngine;
using System.Collections;

public class SelectHex : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        if (Input.GetMouseButton(0))
            SelectHexMethod (hit);
	}

    void SelectHexMethod(RaycastHit hit)
    {
        Debug.Log("You hit: " + hit.collider.gameObject.name);
    }
}
