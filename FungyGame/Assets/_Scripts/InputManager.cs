using UnityEngine;
using System.Collections;

//TODO: Change the name
public class InputManager : MonoBehaviour
{

    public GameObject prefab;
    private Hexagon startHexagon, endHexagon;
    private Fungi startHexChildScript;

    enum DragState
    {
        Idle,
        Dragging
    }

    DragState state;

    Fungi getFungiFromHexagon(Hexagon hexagon)
    {
        var fungiHolder = hexagon.transform.childCount > 0 ? hexagon.transform.GetChild(0) : null;
        var fungi = fungiHolder ? fungiHolder.GetComponent<Fungi>() : null;
        return fungi;
    }

    public void StartDrag(Hexagon startHexagon)
    {
        var fungi = getFungiFromHexagon(startHexagon);
        this.startHexagon = startHexagon;
        startHexChildScript = fungi;
        state = DragState.Dragging;
    }

    public void EndDrag(Hexagon endHexagon)
    {
        GameObject fungiObject = (GameObject)Instantiate(prefab, endHexagon.transform.position + new Vector3(0, 0.001f, 0), Quaternion.LookRotation(Vector3.up * 90));
        endHexagon.infected = true;
        fungiObject.transform.parent = endHexagon.transform;
        startHexChildScript.stage = 0;
        startHexChildScript.UpdateSprite();

        state = DragState.Idle;
    }

    void OnPressingHexagon(Hexagon hexagon)
    {
        if (state == DragState.Idle)
        {
            var fungi = getFungiFromHexagon(hexagon);

            if (hexagon.infected && fungi.stage == fungi.maxStage)
            {
                StartDrag(hexagon);
            }
            else
            {
                state = DragState.Idle;
            }
        }
        else if (state == DragState.Dragging)
        {
            state = DragState.Dragging;
        }
        else
        {
            Debug.Assert(false);
        }
    }

    void OnReleasingHexagon(Hexagon hexagon)
    {
        if (state == DragState.Idle)
        {
            state = DragState.Idle;
        }
        else if (state == DragState.Dragging)
        {
            if (hexagon.isAccessible() && !hexagon.infected)
            {
                EndDrag(hexagon);
                
            }
            else
            {
                state = DragState.Idle;
            }
        }
        else
        {
            Debug.Assert(false);
        }
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //shoot a ray to see where we currently are
        if (Physics.Raycast(ray, out hit))
        {
            hit.collider.tag = "Hexagon";
            var hoveredHexagon = hit.collider.gameObject.GetComponent<Hexagon>();

            //start the drag by pressing the screen
            if (Input.GetMouseButton(0))
            {
                OnPressingHexagon(hoveredHexagon);
            }
            else
            {//check to see if the finger left the screen
                OnReleasingHexagon(hoveredHexagon);
            }
        }
    }
}
