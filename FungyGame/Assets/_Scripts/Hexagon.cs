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

public enum HexagonState
{
    Empty,
    CutTree,
    Sapling,
    Tree,
    DeadWood,
    InfectingSapling,
    InfectingTree,
    InfectingDeadWood,
    InfectedSapling,
    InfectedTree,
    InfectedDeadWood,
    KillingInfectedTree,
    KillingInfectedDeadWood,
    KilledInfectedDeadWood,
}

public class Hexagon : MonoBehaviour
{
    private HexagonState _HexState = HexagonState.Empty;

    public HexagonState HexState { get { return _HexState; } }
    
    public TreeType Type;
    public Fungi TreeInfection; // tree infection
    public Fungi TileInfection; // hexagon infection
    public GameObject OverTile; //Tile with borders

    private float growTime = 10f;
    private float randomGrowTimeRange = 5f;
    private float _nextEventTime = 0f;

    //Updating
    HexagonState lastTreeUpdate;

    //cached components
    private GameObject _tileInfectPrefab;
    private GameObject _treeInfectPrefab;
    private NGO _ngo;
    private SelectionState _currentSelectionState = SelectionState.NotSelected;

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
                case HexagonState.InfectingSapling:
                case HexagonState.InfectingDeadWood:
                case HexagonState.InfectingTree:
                case HexagonState.Sapling:
                case HexagonState.Tree:
                case HexagonState.InfectedSapling:
                case HexagonState.InfectedTree:
                case HexagonState.KillingInfectedTree:
                case HexagonState.KillingInfectedDeadWood:
                case HexagonState.KilledInfectedDeadWood:
                case HexagonState.InfectedDeadWood:
                case HexagonState.DeadWood:
                    return true;
                case HexagonState.CutTree:
                case HexagonState.Empty:
                    return false;
                default:
                    Debug.Assert(false);
                    return false;
            }
        }
    }
    
    public bool TreeIsInfectable
    {
        get
        {
            if (TileInfection != null && !TileInfection.IsAtMaxStage)
                return false;

            switch (HexState)
            {
                case HexagonState.InfectedTree:
                case HexagonState.InfectedDeadWood:
                    return true;
                case HexagonState.KillingInfectedTree:
                case HexagonState.KillingInfectedDeadWood:
                case HexagonState.InfectingSapling:
                case HexagonState.InfectingDeadWood:
                case HexagonState.InfectingTree:
                case HexagonState.InfectedSapling:
                case HexagonState.Tree:
                case HexagonState.DeadWood:
                case HexagonState.Sapling:
                case HexagonState.CutTree:
                case HexagonState.Empty:
                case HexagonState.KilledInfectedDeadWood:
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
            case HexagonState.Sapling:
            case HexagonState.Tree:
            case HexagonState.InfectedSapling:
            case HexagonState.InfectedDeadWood:
            case HexagonState.InfectedTree:
            case HexagonState.CutTree:
            case HexagonState.DeadWood:
            case HexagonState.InfectingSapling:
            case HexagonState.InfectingDeadWood:
            case HexagonState.InfectingTree:
            case HexagonState.KilledInfectedDeadWood:
                break; //nothing to do
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
                _HexState = HexagonState.KilledInfectedDeadWood;
                break;
        }
    }
    
    void OnTileInfectionAtMax()
    {
        switch (HexState)
        {
            case HexagonState.Empty:
            case HexagonState.Sapling:
            case HexagonState.Tree:
            case HexagonState.InfectedSapling:
            case HexagonState.InfectedDeadWood:
            case HexagonState.KilledInfectedDeadWood:
            case HexagonState.InfectedTree:
            case HexagonState.CutTree:
            case HexagonState.DeadWood:
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
                break; //nothing to do
            case HexagonState.InfectingSapling:
                _HexState = HexagonState.InfectedSapling;
                break;
            case HexagonState.InfectingDeadWood:
                _HexState = HexagonState.InfectedDeadWood;
                break;
            case HexagonState.InfectingTree:
                _HexState = HexagonState.InfectedTree;
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

    void RedoIfStateChanged(Action act)
    {
        HexagonState prevState;
        do
        {
            prevState = HexState;
            act.Invoke();
        } while (prevState != HexState);
    }

    void Update()
    {
        RedoIfStateChanged(() => {
            if (HasTree)
            {
                if (Time.time >= _nextEventTime)
                {
                    _nextEventTime = Time.time + UnityEngine.Random.Range(growTime, growTime + randomGrowTimeRange); //Set the next event time value
                    GrowTree();
                }
            }
        });
        
        RedoIfStateChanged(() => {
            if (TreeInfection != null && TreeInfection.IsAtMaxStage)
                OnTreeInfectionAtMax();
        });
        
        RedoIfStateChanged(() => {
            if (TileInfection != null && TileInfection.IsAtMaxStage)
                OnTileInfectionAtMax();
        });
        
        RedoIfStateChanged(() => {
            if (HexagonContainsFungus && _HexTree != null)
            {
                updateFungi(TileInfection);
            }
        });
        
        RedoIfStateChanged(() => {
        if (TreeInfection)
        {
            updateFungi(TreeInfection);
            }
        });
        
        RedoIfStateChanged(() => {
        switch (HexState)
        {
            case HexagonState.Empty:
                removeTree();
                removeTileInfection();
                removeTreeInfection();
                break;
            case HexagonState.Sapling:
            case HexagonState.Tree:
            case HexagonState.CutTree:
            case HexagonState.DeadWood:
                removeTileInfection();
                removeTreeInfection();
                break;
            case HexagonState.InfectingSapling:
            case HexagonState.InfectingDeadWood:
            case HexagonState.InfectingTree:
            case HexagonState.InfectedSapling:
            case HexagonState.InfectedDeadWood:
            case HexagonState.InfectedTree:
                removeTreeInfection();
                break;
            case HexagonState.KilledInfectedDeadWood:
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
                break;
            }
        });

        updateTree();
    }

    private void updateTree()
    {
        if (lastTreeUpdate != HexState)
        {
            lastTreeUpdate = HexState;
            switch (HexState)
            {
                case HexagonState.InfectingSapling:
                case HexagonState.InfectedSapling:
                case HexagonState.Sapling:
                    ReplaceTree(TreeType.Sapling);
                    break;
                case HexagonState.Tree:
                case HexagonState.InfectedTree:
                case HexagonState.InfectingTree:
                case HexagonState.KillingInfectedTree:
                    ReplaceTree(Type);
                    break;
                case HexagonState.CutTree:
                    ReplaceTree(TreeType.CutTree);
                    break;
                case HexagonState.DeadWood:
                case HexagonState.InfectedDeadWood:
                case HexagonState.KilledInfectedDeadWood:
                case HexagonState.InfectingDeadWood:
                case HexagonState.KillingInfectedDeadWood:
                    ReplaceTree(TreeType.DeadTree);
                    break;
            }
        }
    }
    public void ShowOverTile(bool state, Color color)
    {
        OverTile.SetActive(state);
        OverTile.GetComponent<SpriteRenderer>().color = color;
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
        switch (Type)
        {
            case TreeType.CutTree:
                Debug.Assert(false, "unexpected state");
                break;
            case TreeType.Sapling:
                switch (HexState)
                {
                    case HexagonState.CutTree:
                    case HexagonState.Empty:
                    case HexagonState.KillingInfectedDeadWood:
                    case HexagonState.InfectingDeadWood:
                    case HexagonState.DeadWood:
                    case HexagonState.InfectedDeadWood:
                    case HexagonState.KilledInfectedDeadWood:
                    case HexagonState.Tree:
                    case HexagonState.InfectedTree:
                    case HexagonState.InfectingTree:
                    case HexagonState.KillingInfectedTree:
                        Debug.Assert(false, "unexpected state: " + HexState);
                        break;
                    case HexagonState.InfectingSapling:
                        _HexState = HexagonState.InfectingTree;
                        break;
                    case HexagonState.Sapling:
                        _HexState = HexagonState.Tree;
                        break;
                    case HexagonState.InfectedSapling:
                        _HexState = HexagonState.InfectedTree;
                        break;
                }
                ReplaceTree(TreeType.SmallTree);
                break;
            case TreeType.SmallTree:
                ReplaceTree(TreeType.BigTree);
                break;
            case TreeType.BigTree:
                switch (HexState)
                {
                    case HexagonState.CutTree:
                    case HexagonState.Empty:
                    case HexagonState.Sapling:
                    case HexagonState.InfectedSapling:
                    case HexagonState.KillingInfectedDeadWood:
                    case HexagonState.InfectingDeadWood:
                    case HexagonState.InfectingSapling:
                    case HexagonState.DeadWood:
                    case HexagonState.InfectedDeadWood:
                    case HexagonState.KilledInfectedDeadWood:
                        Debug.Assert(false, "unexpected state: " + HexState);
                        break;
                    case HexagonState.Tree:
                        _HexState = HexagonState.DeadWood;
                        break;
                    case HexagonState.InfectedTree:
                        _HexState = HexagonState.InfectedDeadWood;
                        break;
                    case HexagonState.InfectingTree:
                        _HexState = HexagonState.InfectingDeadWood;
                        break;
                    case HexagonState.KillingInfectedTree:
                        _HexState = HexagonState.KillingInfectedDeadWood;
                        break;
                }
                ReplaceTree(TreeType.DeadTree);
                break;
            case TreeType.DeadTree:
                switch (HexState)
                {
                    case HexagonState.CutTree:
                    case HexagonState.Empty:
                    case HexagonState.Sapling:
                    case HexagonState.Tree:
                    case HexagonState.InfectedSapling:
                    case HexagonState.InfectedTree:
                    case HexagonState.InfectingSapling:
                    case HexagonState.InfectingTree:
                        Debug.Assert(false, "unexpected state: " + HexState);
                        break;
                    case HexagonState.DeadWood:
                    case HexagonState.InfectedDeadWood:
                    case HexagonState.KilledInfectedDeadWood:
                    case HexagonState.KillingInfectedTree:
                    case HexagonState.KillingInfectedDeadWood:
                    case HexagonState.InfectingDeadWood:
                        _HexState = HexagonState.Empty;
                        break;
                }
                break;
        }
    }
    
    public void InfectTree()
    {
        switch (HexState)
        {
            case HexagonState.CutTree:
            case HexagonState.Empty:
            case HexagonState.Sapling:
            case HexagonState.Tree:
            case HexagonState.InfectedSapling:
            case HexagonState.DeadWood:
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
            case HexagonState.InfectingSapling:
            case HexagonState.InfectingDeadWood:
            case HexagonState.InfectingTree:
            case HexagonState.KilledInfectedDeadWood:
                Debug.Assert(false, "unexpected state: " + HexState);
                break;
            case HexagonState.InfectedDeadWood:
                addTreeInfectingFungi();
                TileInfection.reset();
                
                GridManager.instance.UserInteraction.updateView();
                _HexState = HexagonState.KillingInfectedDeadWood;
                break;
            case HexagonState.InfectedTree:
                addTreeInfectingFungi();
                TileInfection.reset();

                GridManager.instance.UserInteraction.updateView();
                _HexState = HexagonState.KillingInfectedTree;
                break;
        }
    }
    
    public void TakeCuttedTreeAway()
    {
        switch (HexState)
        {
            case HexagonState.Empty:
            case HexagonState.Sapling:
            case HexagonState.Tree:
            case HexagonState.InfectedSapling:
            case HexagonState.InfectedDeadWood:
            case HexagonState.KilledInfectedDeadWood:
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
            case HexagonState.InfectedTree:
            case HexagonState.DeadWood:
            case HexagonState.InfectingSapling:
            case HexagonState.InfectingDeadWood:
            case HexagonState.InfectingTree:
                Debug.Assert(false, "unexpected state");
                break;
            case HexagonState.CutTree:
                _HexState = HexagonState.Empty;
                break;
        }
    }

    public void ChopTree()
    {
        switch (HexState)
        {
            case HexagonState.CutTree:
            case HexagonState.Empty:
            case HexagonState.DeadWood:
                Debug.Assert(false, "unexpected state");
                break;
            case HexagonState.Sapling:
            case HexagonState.Tree:
            case HexagonState.InfectedSapling:
            case HexagonState.InfectedDeadWood:
            case HexagonState.KilledInfectedDeadWood:
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
            case HexagonState.InfectedTree:
            case HexagonState.InfectingSapling:
            case HexagonState.InfectingDeadWood:
            case HexagonState.InfectingTree:
                _HexState = HexagonState.CutTree;
                break;
        }
    }

    public void PlantTree(TreeType type)
    {
        Debug.Assert(!HexagonContainsFungus, "Trees can only be planted when no fungus is on it");
        Type = type;
        switch (type)
        {
            case TreeType.Sapling:
                _HexState = HexagonState.Sapling;
                break;
            case TreeType.SmallTree:
            case TreeType.BigTree:
                _HexState = HexagonState.Tree;
                break;
            case TreeType.DeadTree:
                _HexState = HexagonState.DeadWood;
                break;
            case TreeType.CutTree:
                _HexState = HexagonState.CutTree;
                break;
        }
    }
    
    private void ReplaceTree(TreeType newType)
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
        return isAdjacentToSelectedHexagon() && HexTree != null && !HexagonContainsFungus && HexState != HexagonState.CutTree;
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
            }
        } else
        {
            _renderer.material = ResourcesManager.instance.HexNormalMaterial;
        }
    }

    public IEnumerator FlashHexagon(Color color)
    {
        bool tmp = false;
        do
        {
            if(tmp)
                ShowOverTile(true, color);
            else
                ShowOverTile(false, color);
            tmp = !tmp;
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
            case HexagonState.CutTree:
            case HexagonState.Empty:
            case HexagonState.KillingInfectedTree:
            case HexagonState.KillingInfectedDeadWood:
            case HexagonState.InfectedSapling:
            case HexagonState.InfectedDeadWood:
            case HexagonState.KilledInfectedDeadWood:
            case HexagonState.InfectedTree:
            case HexagonState.InfectingSapling:
            case HexagonState.InfectingDeadWood:
            case HexagonState.InfectingTree:
                Debug.Assert(false, "unexpected state: " + HexState);
                break;
            case HexagonState.Sapling:
                _HexState = HexagonState.InfectingSapling;
                break;
            case HexagonState.Tree:
                _HexState = HexagonState.InfectingTree;
                break;
            case HexagonState.DeadWood:
                _HexState = HexagonState.InfectingDeadWood;
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
