using UnityEngine;
using System.Collections;

//TODO: Change the name
public class InputManager : MonoBehaviour {

    public GameObject prefab;
    private Hexagon startHexagon, endHexagon;
    private Fungi startHexChildScript;
	
	void Update () {


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                hit.collider.tag = "Hexagon";
                startHexagon = hit.collider.gameObject.GetComponent<Hexagon>();
                Debug.Log("hit start hex");
                

                if (!startHexagon.infected)
                {
                    startHexagon = null;
                    Debug.Log("start hex is not infected");
                }
                else if (startHexagon.infected)
                {
                    startHexChildScript = startHexagon.transform.GetChild(0).GetComponent<Fungi>();
                    if (startHexChildScript.stage != startHexChildScript.maxStage)
                    {
                        startHexagon = null;
                        Debug.Log("start hex is not maxstage");
                    }
                }
                
            }
        }

        if (startHexagon != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    hit.collider.tag = "Hexagon";
                    endHexagon = hit.collider.gameObject.GetComponent<Hexagon>();
                    Debug.Log("hit endhex");
                    if (endHexagon.infected || !endHexagon.isAccessible())
                    {
                        endHexagon = null;
                        Debug.Log("endhex is already infected or you cant move there");
                    }
                }
            }

            if (startHexagon != null && endHexagon != null)
                    if (Map.instance.GetSurroundingTiles(startHexagon).Contains(endHexagon))
                    {
                        //TODO: Change this so the hexagon gets infected?
                        GameObject fungiObject = (GameObject)Instantiate(prefab, endHexagon.transform.position + new Vector3(0, 0.1f, 0), Quaternion.LookRotation(Vector3.up * 90));
                        endHexagon.infected = true;
                        fungiObject.transform.parent = endHexagon.transform;
                        startHexChildScript.stage = 0;
                        endHexagon = null;
                        startHexagon = null;
                    }
        }
	}
}
