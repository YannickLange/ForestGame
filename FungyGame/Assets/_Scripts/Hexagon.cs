using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void HexagonEventHandler(object sender,EventArgs e,int clickID);

public class Hexagon : MonoBehaviour
{
    public bool infected { get; set; }

    public NGO ngo { get; set; }
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
        set
        {
            if (Fungi != value)
            {
                if (Fungi)
                { //remove from from old
                    Fungi.occupiedHexagon = null;
                }
                Fungi = value;
                if (Fungi)
                { //add to new hexagon
                    Fungi.occupiedHexagon = this;
                }
            }
        }
    }
    
    void Update()
    {
        if (_HexTree != null)
        {
            if (_HexTree._processStarted)
            {
                CheckState();
            }
            if (_HexTree._infection != null)
            {
                var infection = _HexTree._infection;
                if (infection.stage == _HexTree._infection.maxStage)
                {
                    Debug.Log("Tree should be dead");
                    //Does not do anything, just here for completion sake
                    _HexTree.State = TreeState.Dead;
                    _HexTree.Type = TreeType.DeadTree;
                    //End of useless code
                
                    ReplaceTree((int)TreeType.DeadTree);
                    Debug.Log(_HexTree);
                    Debug.Log(infection);
                    Debug.Log(infection.gameObject);
                    Destroy(infection.gameObject);
                }
            }
        }
    }
    
    public void CheckState()
    {
        if (_HexTree != null)
        {
            switch (_HexTree.State)
            {
                case TreeState.Alive:
                    if (Time.time >= _HexTree._nextEventTime)
                    {
                        GrowTree();
                    }
                    break;
            }
        } else
        {
            Debug.Log("should not get here");
        }
    }
    
    /// <summary>
    /// Set the next state of the tree
    /// </summary>
    public void GrowTree()
    {
        if (_HexTree != null)
        {
            int typeValue = (int)_HexTree.Type;
            /*if (typeValue >= (int)TreeType.DeadTree) //TODO: Change this back to deadtree when they exist
            return;*/
        
            int newType = typeValue + 1;
            if (newType >= (int)TreeType.DeadTree)
                return;
            _HexTree._nextEventTime = Time.time + UnityEngine.Random.Range(_HexTree.growTime, _HexTree.growTime + _HexTree.randomGrowTimeRange); //Set the next event time value
            ReplaceTree(newType);
        } else
        {
            Debug.Log("should not get here");
        }
    }
    
    public void InfectTree()
    {
        if (_HexTree != null)
        {
            GameObject treeInfect = Instantiate(_HexTree._treeInfectPrefab, _HexTree.transform.position + new Vector3(0f, 0f, 0.01f), _HexTree.transform.rotation) as GameObject;
            treeInfect.transform.parent = _HexTree.transform;
            _HexTree.occupiedHexagon.Fungi.stage = 0;
            _HexTree._infection = treeInfect.GetComponent<Fungi>();
            _HexTree.State = TreeState.Infected;
        
            GridManager.instance.UserInteraction.updateView();
        } else
        {
            Debug.Log("should not get here");
        }
    }
    
    public void ReplaceTree(int newType)
    {
        if (_HexTree != null)
        {
            //create the new tree
            GameObject tree = Instantiate(ResourcesManager.instance.TreeTypes [newType], _HexTree.gameObject.transform.position, _HexTree.gameObject.transform.rotation) as GameObject;
            TreeClass newTreeClassScript = tree.GetComponent<TreeClass>();
            //Make the forest the parent
            tree.transform.parent = GameObject.Find("Forest").transform;
            //Make sure the hexagon and the tree now know their significant other
            newTreeClassScript.occupiedHexagon = this;
            var oldTree = _HexTree;
            _HexTree = newTreeClassScript;
            if (newTreeClassScript.Type == TreeType.DeadTree)
                GridManager.instance.Meter.Fungus(5);
            //destroy the original
            GameObject.Destroy(oldTree.gameObject);
        
            GridManager.instance.UserInteraction.updateView();
        } else
        {
            Debug.Log("should not get here");
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
    
    public bool adjacentAccessibleHexagonExists()
    {
        foreach (var hexagon in SurroundingHexagons)
        {
            if (hexagon.isAccessible())
                return true;
        }
        return false;
    }
    
    public bool isAbleToMoveAwayFrom()
    {
        return infected && Fungi != null && Fungi.stage == Fungi.maxStage && adjacentAccessibleHexagonExists();
    }

    public void updateMaterial()
    {
        var adjacentSelectedHexagon = getAdjacentSelectedHexagon();
        if (CurrentSelectionState == SelectionState.IsSelected)
        {
            _renderer.material = ResourcesManager.instance.HexSelectedMaterial;
        } else if (adjacentSelectedHexagon != null)
        {
            if (adjacentSelectedHexagon.isAbleToMoveAwayFrom() && isAccessible())
            {
                _renderer.material = ResourcesManager.instance.HexValidSurroundingMaterial;
            } else
            {
                _renderer.material = ResourcesManager.instance.HexInvalidSurroundingMaterial;
            }
        } else
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
            } else
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
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        foreach (Touch t in Input.touches)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(t.fingerId))
                return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnHexagonClick(new EventArgs(), 0);
        }
        if (Input.GetMouseButtonUp(1))
        {
            OnHexagonClick(new EventArgs(), 1);
        }

    }
    #endregion
    
    private TreeClass _HexTree;

    public TreeClass HexTree
    {
        get{ return _HexTree;}
        set
        {
            if (_HexTree != value)
            {
                if (_HexTree)
                { //remove from from old
                    _HexTree.occupiedHexagon = null;
                }
                _HexTree = value;
                if (_HexTree)
                { //add to new hexagon
                    _HexTree.occupiedHexagon = this;
                }
            }
        }
    }

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
