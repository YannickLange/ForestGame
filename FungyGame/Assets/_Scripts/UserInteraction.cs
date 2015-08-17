using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UserInteraction : MonoBehaviour {
    
    
    enum UserInteractionState
    {
        Idle,
        HexagonSelected,
        StartedMoving,
        StartedDragging
    }
    private Hexagon startHexagon, endHexagon;
    private Fungi startHexChildScript;
    private Hexagon _prevHexagon = null;
    private UserInteractionState userInteractionState = UserInteractionState.Idle;

	// Use this for initialization
	void Start () {
        GridManager.instance.MoveButton.ClickEvent += OnMoveClicked;
        GridManager.instance.InfectButton.ClickEvent += OnInfectClicked;
   	}
	
	// Update is called once per frame
    void Update () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //shoot a ray to see where we currently are
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Hexagon")
            {
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
    
    private void moveButtonTo(MonoBehaviour button, Hexagon hexagon, Vector3 offset)
    {
        var rectTransform = button.GetComponent<RectTransform>();
        if (hexagon != null)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(hexagon.transform.position);
            
            rectTransform.transform.position = screenPoint + offset;
            rectTransform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            rectTransform.localScale = new Vector3(0, 0, 0);
        }
    }
    
    private bool isMoveButtonActive(Hexagon selectedHexagon)
    {
        return selectedHexagon != null && userInteractionState == UserInteractionState.HexagonSelected && selectedHexagon.infected;
    }
    
    private bool isInfectButtonActive(Hexagon selectedHexagon)
    {
        return selectedHexagon != null && userInteractionState == UserInteractionState.HexagonSelected && selectedHexagon.infected && selectedHexagon.HexTree.State == TreeState.Alive;
    }
    
    private void updateSelectedHexagon(Hexagon hexagonToSelect)
    {
        Debug.Log(userInteractionState);
        List<Hexagon> toBeUpdatedHexagons = new List<Hexagon>();
        if (_prevHexagon != null)
        {
            _prevHexagon.CurrentSelectionState = Hexagon.SelectionState.NotSelected;
            toBeUpdatedHexagons.Add(_prevHexagon);
            toBeUpdatedHexagons.AddRange(_prevHexagon.SurroundingHexagons);
        }
        if (hexagonToSelect)
        {
            toBeUpdatedHexagons.Add(hexagonToSelect);
            toBeUpdatedHexagons.AddRange(hexagonToSelect.SurroundingHexagons);
        }
        
        _prevHexagon = hexagonToSelect;
        
        if (hexagonToSelect)
        {
            hexagonToSelect.CurrentSelectionState = Hexagon.SelectionState.IsSelected;
        }
        foreach (var toBeUpdatedHexagon in toBeUpdatedHexagons)
        {
            toBeUpdatedHexagon.updateMaterial();
        }
        
        moveButtonTo(GridManager.instance.MoveButton, hexagonToSelect, new Vector3(40, 60, 0));
        moveButtonTo(GridManager.instance.InfectButton, hexagonToSelect, new Vector3(-40, 60, 0));
        
        GridManager.instance.MoveButton.GetComponent<UnityEngine.UI.Button>().interactable = isMoveButtonActive(hexagonToSelect);
        GridManager.instance.InfectButton.GetComponent<UnityEngine.UI.Button>().interactable = isInfectButtonActive(hexagonToSelect);
    }
    
    private void OnMoveClicked()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                StartDrag(_prevHexagon);
                userInteractionState = UserInteractionState.StartedMoving;
                break;
            case UserInteractionState.StartedMoving:
                userInteractionState = UserInteractionState.StartedMoving;
                break;
            case UserInteractionState.StartedDragging:
                userInteractionState = UserInteractionState.StartedDragging;
                break;
        }
        updateSelectedHexagon(_prevHexagon);
    }
    
    private void OnInfectClicked()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.StartedMoving:
                userInteractionState = UserInteractionState.StartedMoving;
                break;
            case UserInteractionState.StartedDragging:
                userInteractionState = UserInteractionState.StartedDragging;
                break;
        }
        updateSelectedHexagon(_prevHexagon);
    }
    
    private void updateButtonState()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.StartedMoving:
                userInteractionState = UserInteractionState.StartedMoving;
                break;
            case UserInteractionState.StartedDragging:
                userInteractionState = UserInteractionState.StartedDragging;
                break;
        }
    }
    
    private void OnHexagonSingleClicked(Hexagon hexagon)
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.HexagonSelected:
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.StartedMoving:
                if (hexagon.isAccessible() && !hexagon.infected)
                {
                    EndDrag(hexagon);
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                else
                {
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                break;
            case UserInteractionState.StartedDragging:
                userInteractionState = UserInteractionState.StartedDragging;
                break;
        }
        updateSelectedHexagon(hexagon);
    }
    
    public void OnHexagonClickedEvent(object sender, EventArgs e, int clickID)
    {
        Hexagon hex = sender as Hexagon;
        switch (clickID)
        {
            case 0:
                OnHexagonSingleClicked(hex);
                break;
            case 1:
                //Checking if the selected hexgon is in the surroundings
                var surroundingTiles = Map.instance.GetSurroundingTiles(hex);
                foreach (var surroundingTile in surroundingTiles)
                {
                    if (surroundingTile == hex)
                    {
                        Debug.Log("Perform action on the selected hexagon...[Infect/Expand/Other?]");
                        break;
                    }
                }
                break;
        }
    }
    
    void OnPressingHexagon(Hexagon hexagon)
    {
        var fungi = getFungiFromHexagon(hexagon);
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                if (hexagon.infected && fungi.stage == fungi.maxStage)
                {
                    StartDrag(hexagon);
                    userInteractionState = UserInteractionState.StartedDragging;
                }
                else
                {
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                break;
            case UserInteractionState.StartedMoving:
                userInteractionState = UserInteractionState.StartedMoving;
                break;
            case UserInteractionState.StartedDragging:
                userInteractionState = UserInteractionState.StartedDragging;
                break;
        }
        updateSelectedHexagon(_prevHexagon);
    }
    
    void OnReleasingHexagon(Hexagon hexagon)
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.StartedMoving:
                userInteractionState = UserInteractionState.StartedMoving;
                break;
            case UserInteractionState.StartedDragging:
                if (hexagon.isAccessible() && !hexagon.infected)
                {
                    EndDrag(hexagon);
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                else
                {
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                break;
        }
        updateSelectedHexagon(_prevHexagon);
    }
    
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
    }
    
    public void EndDrag(Hexagon endHexagon)
    {
        Map.instance.PutFungiOn(endHexagon);
        endHexagon.infected = true;
        startHexChildScript.stage = 0;
        startHexChildScript.UpdateSprite();
    }
}
