using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UserInteraction : MonoBehaviour
{
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
    void Start()
    {
        GridManager.instance.MoveButton.ClickEvent += OnMoveClicked;
        GridManager.instance.InfectButton.ClickEvent += OnInfectClicked;
    }
    
    // Update is called once per frame
    void Update()
    {
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
        else
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            foreach (Touch t in Input.touches)
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    return;
            }

            if (Input.GetMouseButton(0))
            {
                OnPressingNowhere();
            }
        }
    }
    
    private void moveButtonTo(MonoBehaviour button, Hexagon hexagon, Vector3 offset)
    {
        var rectTransform = button.GetComponent<RectTransform>();
        if (hexagon != null && !(userInteractionState == UserInteractionState.StartedDragging || userInteractionState == UserInteractionState.StartedMoving))
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
        return selectedHexagon != null && selectedHexagon.isAbleToMoveAwayFrom() && userInteractionState == UserInteractionState.HexagonSelected && selectedHexagon.infected;
    }
    
    private bool isInfectButtonActive(Hexagon selectedHexagon)
    {
        return selectedHexagon != null && userInteractionState == UserInteractionState.HexagonSelected && selectedHexagon.infected && selectedHexagon.HexTree.State == TreeState.Alive && selectedHexagon.HexTree.Type == TreeType.BigTree && selectedHexagon.HexTree && selectedHexagon.Fungi != null && selectedHexagon.Fungi.stage == selectedHexagon.Fungi.maxStage;
    }
    
    UserInteractionState _DEBUG_lastState = UserInteractionState.Idle;
    
    private void updateView()
    {
        updateView(false);
    }

    private void updateView(bool updateAll)
    {
        if (_DEBUG_lastState != userInteractionState)
        {
            Debug.Log(userInteractionState);
            _DEBUG_lastState = userInteractionState;
        }

        List<Hexagon> toBeUpdatedHexagons = new List<Hexagon>();
        if (updateAll)
        {
            toBeUpdatedHexagons.AddRange(Map.instance.Hexagons);
        }
        else
        {
            if (_prevHexagon != null)
            {
                toBeUpdatedHexagons.Add(_prevHexagon);
                toBeUpdatedHexagons.AddRange(_prevHexagon.SurroundingHexagons);
            }
        }
        foreach (var toBeUpdatedHexagon in toBeUpdatedHexagons)
        {
            toBeUpdatedHexagon.updateMaterial();
        }

        moveButtonTo(GridManager.instance.MoveButton, _prevHexagon, new Vector3(50, 60, 0));
        moveButtonTo(GridManager.instance.InfectButton, _prevHexagon, new Vector3(-50, 60, 0));
        
        GridManager.instance.MoveButton.GetComponent<UnityEngine.UI.Button>().interactable = isMoveButtonActive(_prevHexagon);
        GridManager.instance.InfectButton.GetComponent<UnityEngine.UI.Button>().interactable = isInfectButtonActive(_prevHexagon);
    }

    private void selectDifferentHexagon(Hexagon hexagonToSelect)
    {
        if (_prevHexagon)
        {
            _prevHexagon.CurrentSelectionState = Hexagon.SelectionState.NotSelected;
        }
        _prevHexagon = hexagonToSelect;

        if (hexagonToSelect)
        {
            hexagonToSelect.CurrentSelectionState = Hexagon.SelectionState.IsSelected;
        }
        updateView(true);
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
        updateView();
    }
    
    private void OnInfectClicked()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                if (isInfectButtonActive(_prevHexagon))
                {
                    _prevHexagon.HexTree.InfectTree();
                    selectDifferentHexagon(null);
                    userInteractionState = UserInteractionState.Idle;
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
        updateView();
    }
    
    private void OnHexagonSingleClicked(Hexagon hexagon)
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
            case UserInteractionState.HexagonSelected:
                if (hexagon.infected)
                {
                    selectDifferentHexagon(hexagon);
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                else
                {
                    selectDifferentHexagon(null);
                    userInteractionState = UserInteractionState.Idle;
                }
                break;
            case UserInteractionState.StartedMoving:
                if (hexagon.isAccessible() && !hexagon.infected)
                {
                    EndDrag(hexagon);
                    selectDifferentHexagon(null);
                    userInteractionState = UserInteractionState.Idle;
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
        updateView();
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
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                userInteractionState = UserInteractionState.Idle;
                break;
            case UserInteractionState.HexagonSelected:
                if (hexagon.isAbleToMoveAwayFrom() && hexagon == _prevHexagon)
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
        updateView();
    }

    void OnPressingNowhere()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
            case UserInteractionState.HexagonSelected:
            case UserInteractionState.StartedMoving:
            case UserInteractionState.StartedDragging:
                selectDifferentHexagon(null);
                userInteractionState = UserInteractionState.Idle;
                break;
        }
        updateView();
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
                    selectDifferentHexagon(null);
                    userInteractionState = UserInteractionState.Idle;
                }
                else
                {
                    userInteractionState = UserInteractionState.HexagonSelected;
                }
                break;
        }
        updateView();
    }
    
    public void StartDrag(Hexagon startHexagon)
    {
        this.startHexagon = startHexagon;
        startHexChildScript = startHexagon.Fungi;
    }
    
    public void EndDrag(Hexagon endHexagon)
    {
        Map.instance.PutFungiOn(endHexagon);
        endHexagon.infected = true;
        startHexChildScript.stage = 0;
        startHexChildScript.UpdateSprite();
    }
}
