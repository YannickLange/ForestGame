using UnityEngine;
using System.Collections;

//TODO: Change the name
public class InputManager : MonoBehaviour {

    public GameObject prefab;
    private Hexagon startHexagon, endHexagon;
    private Fungi startHexChildScript;
	
	void Update () {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //shoot a ray to see where we currently are
        if (Physics.Raycast(ray, out hit))
        {
            //start the drag by pressing the screen
            if (Input.GetMouseButtonDown(0))
            {
                //if the startHex is not filled in yet, set a new starthex
                if (startHexagon == null)
                {
                    //set a new starthex
                    hit.collider.tag = "Hexagon";
                    startHexagon = hit.collider.gameObject.GetComponent<Hexagon>();

                    //if the startHex is not infected, stop and set startHex to null
                    if (!startHexagon.infected)
                        startHexagon = null;
                    //if it is infected, check to see if it is in the correct stage
                    else if (startHexagon.infected)
                    {
                        startHexChildScript = startHexagon.transform.GetChild(0).GetComponent<Fungi>();
                        if (startHexChildScript.stage != startHexChildScript.maxStage)
                            startHexagon = null;
                    }
                }
            }
            //check to see if the finger left the screen
            else if (Input.GetMouseButtonUp(0) && startHexagon != null)
            {
                //set the endHexagon
                    hit.collider.tag = "Hexagon";
                    endHexagon = hit.collider.gameObject.GetComponent<Hexagon>();

                //if the endHexagon is already infected or it is not accesible, set it back to null
                    if (endHexagon.infected || !endHexagon.isAccessible())
                        endHexagon = null;
            }
        }

        if (endHexagon != null)
        {
            if (Map.instance.GetSurroundingTiles(startHexagon).Contains(endHexagon))
            {
                //create a new fungi and set everything back
                GameObject fungiObject = (GameObject)Instantiate(prefab, endHexagon.transform.position + new Vector3(0, 0.1f, 0), Quaternion.LookRotation(Vector3.up * 90));
                endHexagon.infected = true;
                fungiObject.transform.parent = endHexagon.transform;
                startHexChildScript.stage = 0;
                endHexagon = null;
                startHexagon = null;
            }
                //this can happen when the player drags to some item that is not next to the one that is selected, set everything back
            else
            {
                endHexagon = null;
                startHexagon = null;
            }

        }
	}
}
