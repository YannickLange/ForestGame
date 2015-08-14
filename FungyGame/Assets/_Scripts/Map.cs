using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour
{
    //TODO: CHANGE THIS FUGLY
    public GameObject[] TreeTypes;
    //<<<<<<<<<<<<<<<<<<
    public GameObject fungi;
    private TreeGenerator treeGenerator;
    //private array of hexagons
    private Hexagon[] _hexagons;
    //Readonly hexagons
    public Hexagon[] Hexagons
    {
        get
        {
            return _hexagons;
        }
    }

    private Hexagon _prevHexagon = null;
    private int _arrayOffset = 0;
    //Map singleton
    public static Map instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        BuildMap();
        int rand = UnityEngine.Random.Range(0, _hexagons.Length - 1);
        while (_hexagons[rand].HexTree == null || _hexagons[rand].HexTree.Type == TreeType.Sapling)
        {
            rand = UnityEngine.Random.Range(0, _hexagons.Length - 1);
        }
        _hexagons[rand].infected = true;
        GameObject fungiObject = (GameObject)Instantiate(fungi, _hexagons[rand].transform.position + new Vector3(0, 0.1f, 0), Quaternion.LookRotation(Vector3.up * 90));
        fungiObject.transform.parent = _hexagons[rand].gameObject.transform;
        
        GridManager.instance.MoveButton.ClickEvent += OnMoveClicked;
        GridManager.instance.InfectButton.ClickEvent += OnInfectClicked;
    }

    public void BuildMap()
    {
        if (GridManager.instance.gridWidthInHexes >= GridManager.instance.gridHeightInHexes)
            _arrayOffset = GridManager.instance.gridWidthInHexes;
        else
            _arrayOffset = GridManager.instance.gridHeightInHexes;
        _hexagons = new Hexagon[GridManager.instance.gridWidthInHexes * GridManager.instance.gridHeightInHexes];
        for (int x = 0; x < GridManager.instance.gridWidthInHexes; x++)
            for (int y = 0; y < GridManager.instance.gridHeightInHexes; y++)
            {
                _hexagons[x + y * _arrayOffset] = GridManager.instance.CreateHexagonAt(x, y);
                _hexagons[x + y * _arrayOffset].ClickEvent += new HexagonEventHandler(this.OnHexagonClickedEvent);
            }

        foreach (var hexagon in _hexagons)
        {
            hexagon.SurroundingHexagons = GetSurroundingTiles(hexagon);
        }

        treeGenerator = new TreeGenerator(TreeTypes);
    }

    public List<Hexagon> GetSurroundingTiles(Hexagon hexagon)
    {
        if (hexagon == null)
            return new List<Hexagon>();
        int x = hexagon.X;
        int y = hexagon.Y;
        List<Hexagon> surroundingHexs = new List<Hexagon>();
        #region LEFT SIDE
        //Left upper sider
        if (y % 2 != 0)
        {
            if (!IsOutOfBounds(x, y - 1))
            {
                surroundingHexs.Add(_hexagons[x + (y - 1) * _arrayOffset]);
            }
        }
        else if (!IsOutOfBounds(x - 1, y - 1))
        {
            surroundingHexs.Add(_hexagons[(x - 1) + (y - 1) * _arrayOffset]);
        }


        //Left tile
        if (!IsOutOfBounds(x - 1, y))
        {
            surroundingHexs.Add(_hexagons[(x - 1) + y * _arrayOffset]);
        }
        //Left down tile
        if (y % 2 != 0)
        {
            if (!IsOutOfBounds(x, y + 1))
            {
                surroundingHexs.Add(_hexagons[x + (y + 1) * _arrayOffset]);
            }
        }
        else if (!IsOutOfBounds(x - 1, y + 1))
        {
            surroundingHexs.Add(_hexagons[(x - 1) + (y + 1) * _arrayOffset]);
        }

        #endregion
        #region RIGHT SIDE
        //Right down tile
        if (y % 2 == 0)
        {
            if (!IsOutOfBounds(x, y + 1))
                surroundingHexs.Add(_hexagons[x + (y + 1) * _arrayOffset]);
        }
        else if (!IsOutOfBounds(x + 1, y + 1))
            surroundingHexs.Add(_hexagons[(x + 1) + (y + 1) * _arrayOffset]);
        //Right tile
        if (!IsOutOfBounds(x + 1, y))
            surroundingHexs.Add(_hexagons[(x + 1) + y * _arrayOffset]);

        if (y % 2 == 0)
        {
            if (!IsOutOfBounds(x, y - 1))
                surroundingHexs.Add(_hexagons[x + (y - 1) * _arrayOffset]);
        }
        else if (!IsOutOfBounds(x + 1, y - 1))
            surroundingHexs.Add(_hexagons[(x + 1) + (y - 1) * _arrayOffset]);
        #endregion

        return surroundingHexs;
    }

    private bool IsOutOfBounds(int x, int y)
    {
        return (y < 0 || x < 0 || x >= GridManager.instance.gridWidthInHexes || y >= GridManager.instance.gridWidthInHexes);
    }

    private MoveButton.ButtonEventHandler currentMoveHandler;
    private InfectButton.ButtonEventHandler currentInfectHandler;

    enum UserInteractionState
    {
        Idle,
        HexagonSelected,
        StartedMoving,
    }

    private void updateSelectedHexagon(Hexagon hexagonToSelect)
    {
        List<Hexagon> toBeUpdatedHexagons = new List<Hexagon>();
        if (_prevHexagon != null)
        {
            _prevHexagon.CurrentSelectionState = Hexagon.SelectionState.NotSelected;
            toBeUpdatedHexagons.Add(_prevHexagon);
            toBeUpdatedHexagons.AddRange(_prevHexagon.SurroundingHexagons);
        }
        toBeUpdatedHexagons.Add(hexagonToSelect);
        toBeUpdatedHexagons.AddRange(hexagonToSelect.SurroundingHexagons);

        _prevHexagon = hexagonToSelect;

        hexagonToSelect.CurrentSelectionState = Hexagon.SelectionState.IsSelected;
        foreach (var toBeUpdatedHexagon in toBeUpdatedHexagons)
        {
            toBeUpdatedHexagon.updateMaterial();
        }
    }

    private UserInteractionState userInteractionState = UserInteractionState.Idle;

    private void OnMoveClicked()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                break;
            case UserInteractionState.HexagonSelected:
                break;
            case UserInteractionState.StartedMoving:
                break;
        }
    }

    private void OnInfectClicked()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                break;
            case UserInteractionState.HexagonSelected:
                break;
            case UserInteractionState.StartedMoving:
                break;
        }
    }

    private void updateButtonState()
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                break;
            case UserInteractionState.HexagonSelected:
                break;
            case UserInteractionState.StartedMoving:
                break;
        }
    }

    private void OnHexagonSingleClicked(Hexagon hexagon)
    {
        switch (userInteractionState)
        {
            case UserInteractionState.Idle:
                updateSelectedHexagon(hexagon);
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.HexagonSelected:
                updateSelectedHexagon(hexagon);
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
            case UserInteractionState.StartedMoving:

                updateSelectedHexagon(hexagon);
                userInteractionState = UserInteractionState.HexagonSelected;
                break;
        }
    }

    private void OnHexagonClickedEvent(object sender, EventArgs e, int clickID)
    {

        Hexagon hex = sender as Hexagon;
        switch (clickID)
        {
            case 0:
                OnHexagonSingleClicked(hex);
                break;
            case 1:
            //Checking if the selected hexgon is in the surroundings
                var surroundingTiles = GetSurroundingTiles(hex);
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
}
