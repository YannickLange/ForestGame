using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void HexagonEventHandler(object sender,EventArgs e,int clickID);

public class Hexagon : MonoBehaviour
{
    public bool infected { get; set; }

    public enum SelectionState
    {
        NotSelected,
        IsSelected
    }

    private SelectionState _currentSelectionState = SelectionState.NotSelected;

    public SelectionState CurrentSelectionState
    {
        get { return _currentSelectionState; }
        set
        {
            _currentSelectionState = value;
            updateMaterial();
        }
    }
    
    public Fungi Fungi
    {
        get
        {
            var fungiHolder = transform.childCount > 0 ? transform.GetChild(0) : null;
            var fungi = fungiHolder ? fungiHolder.GetComponent<Fungi>() : null;
            return fungi;
        }
    }

    public bool isAdjacentToSelectedHexagon()
    {
        return getAdjacentSelectedHexagon() != null;
    }

    public Hexagon getAdjacentSelectedHexagon()
    {
        foreach (var hexagon in SurroundingHexagons)
        {
            if (hexagon.CurrentSelectionState == SelectionState.IsSelected)
            {
                return hexagon;
            }
        }
        return null;
    }
    
    public bool isAccessible()
    {
        return isAdjacentToSelectedHexagon() && HexTree != null && !infected;
    }

    public bool isAbleToMoveAwayFrom()
    {
        return infected && Fungi.stage == Fungi.maxStage;
    }

    public void updateMaterial()
    {
        var adjacentSelectedHexagon = getAdjacentSelectedHexagon();
        if (CurrentSelectionState == SelectionState.IsSelected)
        {
            _renderer.material = ResourcesManager.instance.HexSelectedMaterial;
        }
        else if (adjacentSelectedHexagon != null )
        {
            if (adjacentSelectedHexagon.isAbleToMoveAwayFrom() && isAccessible())
            {
                _renderer.material = ResourcesManager.instance.HexValidSurroundingMaterial;
            }
            else
            {
                _renderer.material = ResourcesManager.instance.HexInvalidSurroundingMaterial;
            }
        }
        else
        {
            _renderer.material = ResourcesManager.instance.HexNormalMaterial;
        }
    }

    public IEnumerator FlashHexagon(Material flashMat)
    {
        bool tmp = false;
        do
        {
            if (tmp)
            {
                HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
                tmp = false;
            }
            else
            {
                HexagonRenderer.material = flashMat;
                tmp = true;
            }
            yield return new WaitForSeconds(.4f);
        } while (isTarget);

        yield return null;
    }

    #region CLick event
    public event HexagonEventHandler ClickEvent;

    protected virtual void OnHexagonClick(EventArgs e, int clickID)
    {
        if (ClickEvent != null)
            ClickEvent(this, e, clickID);
    }

    void OnMouseOver()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnHexagonClick(new EventArgs(), 0);
            }
            if (Input.GetMouseButtonDown(1))
            {
                OnHexagonClick(new EventArgs(), 1);
            }
        }
    }
    #endregion

    public TreeClass HexTree { get; set; }

    public bool isTarget { get; set; }

    public List<Hexagon> SurroundingHexagons { get; set; }

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        isTarget = false;
    }

    private int _posX = -1;
    /// <summary>
    /// X position of the hexagon in the map grid
    /// </summary>
    public int X
    {
        get { return _posX; }
    }

    private int _posY = -1;
    /// <summary>
    /// Y position of the hexagon in the map grid
    /// </summary>
    public int Y
    {
        get { return _posY; }
    }

    private Renderer _renderer;
    /// <summary>
    /// Readonly hexagon's renderer
    /// </summary>
    public Renderer HexagonRenderer
    {
        get { return _renderer; }
    }

    /// <summary>
    /// Set the (x,y) information about the position of the tile
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        _posX = x;
        _posY = y;
    }
}
