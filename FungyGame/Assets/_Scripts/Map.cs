using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour
{
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

    private int _arrayOffset = 0;
    //Map singleton
    public static Map instance = null;

    public Hexagon[] HexBorders { get; set; }

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
        PutFungiOn(_hexagons[rand]);
    }

    public void PutFungiOn(Hexagon hex)
    {
        GameObject fungiObject = (GameObject)Instantiate(fungi, hex.transform.position + new Vector3(0, 0.001f, 0), Quaternion.LookRotation(Vector3.up * 90));
        fungiObject.transform.parent = hex.gameObject.transform;
    }

    public void BuildMap()
    {
        List<Hexagon> borders = new List<Hexagon>();

        if (GridManager.instance.gridWidthInHexes >= GridManager.instance.gridHeightInHexes)
            _arrayOffset = GridManager.instance.gridWidthInHexes;
        else
            _arrayOffset = GridManager.instance.gridHeightInHexes;
        _hexagons = new Hexagon[GridManager.instance.gridWidthInHexes * GridManager.instance.gridHeightInHexes];
        for (int x = 0; x < GridManager.instance.gridWidthInHexes; x++)
            for (int y = 0; y < GridManager.instance.gridHeightInHexes; y++)
            {
                _hexagons[x + y * _arrayOffset] = GridManager.instance.CreateHexagonAt(x, y);
                _hexagons[x + y * _arrayOffset].ClickEvent += new HexagonEventHandler(GridManager.instance.UserInteraction.OnHexagonClickedEvent);
                //Making the border array:
                if (x == 0 || y == 0 || x == GridManager.instance.gridWidthInHexes - 1 || y == GridManager.instance.gridHeightInHexes - 1)
                    borders.Add(_hexagons[x + y * _arrayOffset]);
            }

        HexBorders = new Hexagon[borders.Count];
        borders.CopyTo(HexBorders, 0);

        foreach (var hexagon in _hexagons)
        {
            hexagon.SurroundingHexagons = GetSurroundingTiles(hexagon);
        }

        treeGenerator = new TreeGenerator();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //TEMP
            //Spawn planter
            GameObject planter = Instantiate(ResourcesManager.instance.Planter) as GameObject;
            planter.GetComponent<Planter>().Spawn();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            //TEMP
            //Spawn planter
            GameObject lumberjack = Instantiate(ResourcesManager.instance.Lumberjack) as GameObject;
            lumberjack.GetComponent<Lumberjack>().Spawn();
        }
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
}
