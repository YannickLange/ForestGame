using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public GameObject prefab;
    private Hexagon startHexagon, endHexagon;
	
	//TODO: Change the fact that Trees are getting infected/checked to checking/infecting Hexagonsw
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
                    if (endHexagon.infected || endHexagon.currentState != Hexagon.State.CanMoveThere)
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
                        Instantiate(prefab, endHexagon.transform.position + new Vector3(0, 0.1f, 0), Quaternion.LookRotation(Vector3.up * 90));
                        endHexagon.infected = true;
                        endHexagon = null;
                        startHexagon = null;
                    }
        }
	}
}
