using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void HexagonEventHandler(object sender,EventArgs e,int clickID);

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
    WithCutTree,
    WithSapling,
    WithTree,
    WithDeadWood,
    WithSaplingAndFungi,
    WithTreeAndFungi,
    WithCurrentlyInfectingTreeAndFungi,
    WithDeadWoodAndFungi
}

public class Hexagon : MonoBehaviour
{
    private HexagonState _HexState = HexagonState.Empty;

    public HexagonState HexState { get { return _HexState; } }
    
    public TreeType Type;
    public Fungi TreeInfection; // tree infection
    public Fungi TileInfection; // hexagon infection

    private float growTime = 10f;
    private float randomGrowTimeRange = 5f;
    private float _nextEventTime = 0f;

    //cached components
    private GameObject _tileInfectPrefab;
    private GameObject _treeInfectPrefab;
    private NGO _ngo;
    private SelectionState _currentSelectionState = SelectionState.NotSelected;

    public bool HasInfectableTree { get { return HasTree && (Type == TreeType.BigTree || Type == TreeType.DeadTree || Type == TreeType.SmallTree); } }

    public bool canInfectTree()
    {
        return HexagonContainsFungus && HasInfectableTree && TileInfection != null && TileInfection.IsAtMaxStage;
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

    public SelectionState CurrentSelectionState
    {
        get { return _currentSelectionState; }
        set
        {
            _currentSelectionState = value;
            updateMaterial();
        }
    }

    bool HasTree
    {
        get
        {
            switch (HexState)
            {
                case HexagonState.WithSapling:
                case HexagonState.WithTree:
                case HexagonState.WithSaplingAndFungi:
                case HexagonState.WithTreeAndFungi:
                case HexagonState.WithCurrentlyInfectingTreeAndFungi:
                    return true;
                case HexagonState.WithDeadWood:
                case HexagonState.WithCutTree:
                case HexagonState.WithDeadWoodAndFungi:
                case HexagonState.Empty:
                    return false;
                default:
                    Debug.Assert(false);
                    return false;
            }
        }
    }

    void OnTreeInfectionAtMax()
    {
        switch (HexState)
        {
            case HexagonState.Empty:
            case HexagonState.WithSapling:
            case HexagonState.WithTree:
            case HexagonState.WithSaplingAndFungi:
            case HexagonState.WithDeadWoodAndFungi:
            case HexagonState.WithTreeAndFungi:
            case HexagonState.WithCutTree:
            case HexagonState.WithDeadWood:
                Debug.Assert(false, "unexpected state");
                break;
            case HexagonState.WithCurrentlyInfectingTreeAndFungi:
                ReplaceTree(TreeType.DeadTree);
                break;
        }
    }
    
    private void removeTileInfection()
    {
        if (TileInfection != null)
        {
            Destroy(TileInfection.gameObject);
            TileInfection = null;
        }
    }
    
    private void removeTreeInfection()
    {
        if (TreeInfection != null)
        {
            Destroy(TreeInfection.gameObject);
            TreeInfection = null;
        }
    }
    
    private void removeTree()
    {
        if (_HexTree != null)
        {
            Destroy(HexTree.gameObject);
            _HexTree = null;
        }
    }

    void Update()
    {
        if (HasTree)
        {
            if (Time.time >= _nextEventTime)
            {
                GrowTree();
            }
        }

        if (TreeInfection != null && TreeInfection.IsAtMaxStage)
            OnTreeInfectionAtMax();

        if (HexagonContainsFungus && _HexTree != null)
        {
            updateFungi(TileInfection);
        }
        if (TreeInfection)
        {
            updateFungi(TreeInfection);
        }
        
        switch (HexState)
        {
            case HexagonState.Empty:
                removeTree();
                removeTileInfection();
                removeTreeInfection();
                break;
            case HexagonState.WithSapling:
            case HexagonState.WithTree:
            case HexagonState.WithCutTree:
            case HexagonState.WithDeadWood:
                removeTileInfection();
                removeTreeInfection();
                break;
            case HexagonState.WithSaplingAndFungi:
            case HexagonState.WithDeadWoodAndFungi:
            case HexagonState.WithTreeAndFungi:
                removeTreeInfection();
                break;
            case HexagonState.WithCurrentlyInfectingTreeAndFungi:
                break;
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
        switch (HexState)
        {
            case HexagonState.WithCutTree:
            case HexagonState.Empty:
            case HexagonState.WithSapling:
            case HexagonState.WithTree:
            case HexagonState.WithSaplingAndFungi:
            case HexagonState.WithDeadWoodAndFungi:
            case HexagonState.WithCurrentlyInfectingTreeAndFungi:
            case HexagonState.WithDeadWood:
                Debug.Assert(false, "unexpected state");
                break;
            case HexagonState.WithTreeAndFungi:
                addTreeInfectingFungi();
                TileInfection.reset();


                GridManager.instance.UserInteraction.updateView();
                _HexState = HexagonState.WithCurrentlyInfectingTreeAndFungi;
                break;
        }
    }
    
    public void ReplaceTree(TreeType newType)
    {
        //create the new tree
        TreeClass tree = (Instantiate(ResourcesManager.instance.TreeTypes [(int)newType], transform.position, transform.rotation) as GameObject).GetComponent<TreeClass>();

        //Make the forest the parent
        tree.transform.parent = GameObject.Find("Forest").transform;
        //Make sure the hexagon and the tree now know their significant other
        var oldTree = _HexTree;
        _HexTree = tree;
        Type = newType;

        //destroy the original
        if (oldTree != null)
        {
            Destroy(oldTree.gameObject);
        }

        tree.GetComponent<CameraFacingBillboard>().Update();

        switch (HexState)
        {
            case HexagonState.Empty:
            case HexagonState.WithCutTree:
            case HexagonState.WithSapling:
            case HexagonState.WithTree:
            case HexagonState.WithDeadWood:
                switch (newType)
                {
                    case TreeType.Sapling:
                        _HexState = HexagonState.WithSapling;
                        break;
                    case TreeType.SmallTree:
                    case TreeType.BigTree:
                        _HexState = HexagonState.WithTree;
                        break;
                    case TreeType.DeadTree:
                        _HexState = HexagonState.WithDeadWood;
                        GridManager.instance.Meter.Fungus(5);
                        break;
                    case TreeType.CutTree:
                        _HexState = HexagonState.Empty;
                        break;
                }
                break;
            case HexagonState.WithSaplingAndFungi:
            case HexagonState.WithDeadWoodAndFungi:
            case HexagonState.WithCurrentlyInfectingTreeAndFungi:
            case HexagonState.WithTreeAndFungi:
                switch (newType)
                {
                    case TreeType.Sapling:
                        _HexState = HexagonState.WithSaplingAndFungi;
                        break;
                    case TreeType.SmallTree:
                    case TreeType.BigTree:
                        _HexState = HexagonState.WithTreeAndFungi;
                        break;
                    case TreeType.DeadTree:
                        _HexState = HexagonState.WithDeadWoodAndFungi;
                        GridManager.instance.Meter.Fungus(5);
                        break;
                    case TreeType.CutTree:
                        _HexState = HexagonState.Empty;
                        break;
                }
                break;
        }
        GridManager.instance.UserInteraction.updateView();
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
    }

    private Fungi CreateFungi(GameObject prefab)
    {
        GameObject fungiObject = Instantiate(prefab, transform.position + new Vector3(0, 0.001f, 0), Quaternion.LookRotation(Vector3.up * 90)) as GameObject;
        fungiObject.transform.parent = gameObject.transform;
        //NGO checking
        if (ngo != null)
            StartCoroutine(ngo.PickupNGO());

        fungiObject.GetComponent<CameraFacingBillboard>().Update();

        return fungiObject.GetComponent<Fungi>();
    }

    public void addTreeInfectingFungi()
    {
        TreeInfection = CreateFungi(_treeInfectPrefab);
    }
    
    public void addTileInfectingFungi()
    {
        TileInfection = CreateFungi(_tileInfectPrefab);
        
        switch (HexState)
        {
            case HexagonState.WithCutTree:
            case HexagonState.Empty:
            case HexagonState.WithCurrentlyInfectingTreeAndFungi:
            case HexagonState.WithDeadWood:
            case HexagonState.WithSaplingAndFungi:
            case HexagonState.WithDeadWoodAndFungi:
            case HexagonState.WithTreeAndFungi:
                Debug.Assert(false, "unexpected state: " + HexState);
                break;
            case HexagonState.WithSapling:
                _HexState = HexagonState.WithSaplingAndFungi;
                break;
            case HexagonState.WithTree:
                _HexState = HexagonState.WithTreeAndFungi;
                break;
        }
    }

    public bool isTarget { get; set; }

    public List<Hexagon> SurroundingHexagons { get; set; }

    void Awake()
    {
        _treeInfectPrefab = (GameObject)Resources.Load("InfectLoadingBar");
        _tileInfectPrefab = (GameObject)Resources.Load("Fungi");
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
