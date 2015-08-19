using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void HexagonEventHandler(object sender,EventArgs e,int clickID);

public enum TreeState
{
    Alive,
    Infected,
    Dead
}

public enum TreeType
{
    Sapling = 0,
    SmallTree = 1,
    BigTree = 2,
    DeadTree = 3,
    CutTree = 4
}
//DEADTREE must be last!

public enum HexagonState
{
    Empty,
    WithSapling,
    WithTree,
    WithSaplingAndFungi,
    WithTreeAndFungi,
    WithDeadWoodAndFungi
}

public class Hexagon : MonoBehaviour
{
    private HexagonState _HexState = HexagonState.Empty;

    public HexagonState HexState { get { return _HexState; } }
    
    public TreeType Type;
    public Fungi TreeInfection; // tree infection
    public Fungi TileInfection; // hexagon infection

    private TreeState State;
    public float growTime = 10f;
    public float randomGrowTimeRange = 5f;
    public float _nextEventTime = 0f;

    //cached components
    public GameObject _treeInfectPrefab;
    private NGO _ngo;

    public bool canInfectTree()
    {
        return HexagonContainsFungus && State == TreeState.Alive && (Type == TreeType.BigTree || Type == TreeType.DeadTree || Type == TreeType.SmallTree) && HexTree != null && TileInfection.IsAtMaxStage;
    }

    public bool HexagonContainsFungus    { get { return (TileInfection != null); } }

    public NGO ngo
    {
        get
        {
            return _ngo;
        }
        set
        {
            _ngo = value;
            HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
        }
    }
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
    
    void Update()
    {
        if (_HexTree != null)
        {
            CheckState();
            if (TreeInfection != null)
            {
                var infection = TreeInfection;
                if (infection.IsAtMaxStage)
                {
                    Debug.Log("Tree should be dead");
                
                    ReplaceTree(TreeType.DeadTree);
                    //Does not do anything, just here for completion sake
                    State = TreeState.Dead;
                    //End of useless code
                    Debug.Log(_HexTree);
                    Debug.Log(infection);
                    Debug.Log(infection.gameObject);
                    Destroy(infection.gameObject);
                }
            }
        }
        if (HexagonContainsFungus && _HexTree != null)
        {
            updateFungi(TileInfection);
        }
        if (TreeInfection)
        {
            updateFungi(TreeInfection);
        }
    }

    public void updateFungi(Fungi fungi)
    {
        float speed = 1;
        //Should adjust it for multiple types, not extendable code! TODO
        if (Type == TreeType.SmallTree)
        {
            speed = 1 + 0.3f + (UnityEngine.Random.value / 4);
        } else if (Type == TreeType.BigTree)
        {
            speed = 1 + 0.5f + (UnityEngine.Random.value / 2);
        }
        fungi.advanceGrowth(speed);
    }
    
    public void CheckState()
    {
        if (_HexTree != null)
        {
            switch (State)
            {
                case TreeState.Alive:
                    if (Time.time >= _nextEventTime)
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
            int typeValue = (int)Type;
            /*if (typeValue >= (int)TreeType.DeadTree) //TODO: Change this back to deadtree when they exist
            return;*/
        
            int newType = typeValue + 1;
            if (newType >= (int)TreeType.DeadTree)
                return;
            _nextEventTime = Time.time + UnityEngine.Random.Range(growTime, growTime + randomGrowTimeRange); //Set the next event time value
            ReplaceTree((TreeType)newType);
        } else
        {
            Debug.Log("should not get here");
        }
    }
    
    public void InfectTree()
    {
        if (_HexTree != null)
        {
            GameObject treeInfect = Instantiate(_treeInfectPrefab, _HexTree.transform.position + new Vector3(0f, 0f, 0.01f), _HexTree.transform.rotation) as GameObject;
            treeInfect.transform.parent = _HexTree.transform;
            _HexTree.occupiedHexagon.TileInfection.reset();
            TreeInfection = treeInfect.GetComponent<Fungi>();
            State = TreeState.Infected;
        
            GridManager.instance.UserInteraction.updateView();
        } else
        {
            Debug.Log("should not get here");
        }
    }
    
    public void ReplaceTree(TreeType newType)
    {
        if (_HexTree != null)
        {
            //create the new tree
            GameObject tree = Instantiate(ResourcesManager.instance.TreeTypes[(int)newType], _HexTree.gameObject.transform.position, _HexTree.gameObject.transform.rotation) as GameObject;
            TreeClass newTreeClassScript = tree.GetComponent<TreeClass>();
            //Make the forest the parent
            tree.transform.parent = GameObject.Find("Forest").transform;
            //Make sure the hexagon and the tree now know their significant other
            newTreeClassScript.occupiedHexagon = this;
            var oldTree = _HexTree;
            _HexTree = newTreeClassScript;
            State = TreeState.Alive;
            Type = newType;
            if (newType == TreeType.DeadTree)
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
        return isAdjacentToSelectedHexagon() && HexTree != null && !HexagonContainsFungus;
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
        return HexagonContainsFungus && TileInfection.IsAtMaxStage && adjacentAccessibleHexagonExists();
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
            } //else
            /*else
            {
                _renderer.material = ResourcesManager.instance.HexInvalidSurroundingMaterial;
            }*/
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
        _treeInfectPrefab = (GameObject)Resources.Load("InfectLoadingBar");
        _renderer = GetComponent<Renderer>();
        isTarget = false;
        _nextEventTime = Time.time + UnityEngine.Random.Range(growTime, growTime + randomGrowTimeRange);
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
