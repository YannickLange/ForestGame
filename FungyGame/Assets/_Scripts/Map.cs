using UnityEngine;
using System.Collections;
using System;

public class Map : MonoBehaviour
{
    //TODO: CHANGE THIS FUGLY
    public GameObject[] TreeTypes; //<<<<<<<<<<<<<<<<<<

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

    //Size parameters
    private int _width = 10;
    private int _height = 10;

    public Material NormalMaterial;
    public Material HighlightedMaterial;
    public Material SurroundingValidMaterial;
    public Material SurroundingInvalidMaterial;

    private Hexagon _prevHexagon = null;
    private Hexagon[] _surroundingHexs; //Array for the 6 surrounding tiles of the selected hexagon
    private int _arrayOffset = 0;


    //Map singleton
    public static Map instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        _surroundingHexs = new Hexagon[6];
    }

    void Start()
    {
        BuildMap();
    }


    public void BuildMap(int width = 10, int height = 10)
    {
        _width = width;
        _height = height;
        if (width >= height)
            _arrayOffset = width;
        else
            _arrayOffset = height;
        _hexagons = new Hexagon[width * height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                _hexagons[x + y * _arrayOffset] = GridManager.instance.CreateHexagonAt(x, y);
                _hexagons[x + y * _arrayOffset].ClickEvent += new HexagonEventHandler(this.OnHexagonClickedEvent);
            }

        treeGenerator = new TreeGenerator(TreeTypes);
    }

    private void GetSurroundingTiles(int x, int y)
    {
        ResetSurroundingTilesHighlighted();

        #region LEFT SIDE
        //Left upper sider
        if (y % 2 != 0)
        {
            if (!IsOutOfBounds(x, y - 1))
                _surroundingHexs[0] = _hexagons[x + (y - 1) * _arrayOffset];
            else
                _surroundingHexs[0] = null;
        }
        else if (!IsOutOfBounds(x - 1, y - 1))
            _surroundingHexs[0] = _hexagons[(x - 1) + (y - 1) * _arrayOffset];
        else
            _surroundingHexs[0] = null;


        //Left tile
        if (!IsOutOfBounds(x - 1, y))
            _surroundingHexs[1] = _hexagons[(x - 1) + y * _arrayOffset];
        else
            _surroundingHexs[1] = null;
        //Left down tile
        if (y % 2 != 0)
        {
            if (!IsOutOfBounds(x, y + 1))
                _surroundingHexs[2] = _hexagons[x + (y + 1) * _arrayOffset];
            else
                _surroundingHexs[2] = null;
        }
        else if (!IsOutOfBounds(x - 1, y + 1))
            _surroundingHexs[2] = _hexagons[(x - 1) + (y + 1) * _arrayOffset];
        else
            _surroundingHexs[2] = null;

        #endregion
        #region RIGHT SIDE
        //Right down tile
        if (y % 2 == 0)
        {
            if (!IsOutOfBounds(x, y + 1))
                _surroundingHexs[3] = _hexagons[x + (y + 1) * _arrayOffset];
            else
                _surroundingHexs[3] = null;
        }
        else if (!IsOutOfBounds(x + 1, y + 1))
            _surroundingHexs[3] = _hexagons[(x + 1) + (y + 1) * _arrayOffset];
        else
            _surroundingHexs[3] = null;
        //Right tile
        if (!IsOutOfBounds(x + 1, y))
            _surroundingHexs[4] = _hexagons[(x + 1) + y * _arrayOffset];
        else
            _surroundingHexs[4] = null;

        if (y % 2 == 0)
        {
            if (!IsOutOfBounds(x, y - 1))
                _surroundingHexs[5] = _hexagons[x + (y - 1) * _arrayOffset];
            else
                _surroundingHexs[5] = null;
        }
        else if (!IsOutOfBounds(x + 1, y - 1))
            _surroundingHexs[5] = _hexagons[(x + 1) + (y - 1) * _arrayOffset];
        else
            _surroundingHexs[5] = null;
        #endregion
    }

    private void SetSurroundingTilesHighlighted()
    {
        for (int i = 0; i < 6; i++)
        {
            if (_surroundingHexs[i] != null)
            {
                if (_surroundingHexs[i].HexTree != null)
                    _surroundingHexs[i].HexagonRenderer.material = SurroundingValidMaterial;
                else
                    _surroundingHexs[i].HexagonRenderer.material = SurroundingInvalidMaterial;
            }
        }
    }

    private bool IsOutOfBounds(int x, int y)
    {
        if (x < 0)
            return true;
        if (x >= _width)
            return true;
        if (y < 0)
            return true;
        if (y >= _height)
            return true;
        return false;
    }

    private void ResetSurroundingTilesHighlighted()
    {
        for (int i = 0; i < 6; i++)
        {
            if (_surroundingHexs[i] != null)
                _surroundingHexs[i].HexagonRenderer.material = NormalMaterial;
        }
    }

    private void OnHexagonClickedEvent(object sender, EventArgs e, int clickID)
    {
        Hexagon hex = sender as Hexagon;
        switch (clickID)
        {
            case 0:
                if (_prevHexagon != null)
                    _prevHexagon.HexagonRenderer.material = NormalMaterial;
                _prevHexagon = hex;

                GetSurroundingTiles(hex.X, hex.Y);

                hex.HexagonRenderer.material = HighlightedMaterial;

                SetSurroundingTilesHighlighted();
                break;
            case 1:
                //Checking if the selected hexgon is in the surroundings
                for (int i = 0; i < 6; i++)
                    if (_surroundingHexs[i] == hex)
                    {
                        Debug.Log("Perform action on the selected hexagon...[Infect/Expand/Other?]");
                        break;
                    }
                break;
        }
    }
}
