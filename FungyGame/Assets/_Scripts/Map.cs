using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class Map : MonoBehaviour
{
    //<<<<<<<<<<<<<<<<<<
    public GameObject fungi;
    private TreeGenerator treeGenerator;
    //private array of hexagons
    private Hexagon[] _hexagons;
    public int MinimumSpawnTrees = 1;
    //Readonly hexagons
    public Hexagon[] Hexagons
    {
        get
        {
            return _hexagons;
        }
    }

    //Spawner variables
    public float _planterNextSpawn = 6f;
    public float _planterRndUp = 7f;
    public float _planterRndDown = 10f;

    public float _lumberjackNextSpawn = 4f;
    public float _lumberjackRndUp = 7f;
    public float _lumberjackRndDown = 10f;

    public float _NGONextSpawn = 4f;
    public float _NGORndUp = 7f;
    public float _NGORndDown = 10f;
    //Map singleton
    public static Map instance = null;

    public Hexagon[] HexBorders { get; set; }

    private Transform _planter;
    private Transform _lumberjack;
    private Transform _ngo;
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
        //DontDestroyOnLoad(gameObject);

        _planter = GameObject.Find("Planters").transform;
        _planterNextSpawn = 6f;
        _lumberjack = GameObject.Find("Lumberjacks").transform;
        _planterNextSpawn = 4f;
        _ngo = GameObject.Find("NGO").transform;
        _planterNextSpawn = 4f;
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
        SpawnFungi(_hexagons[rand]);
    }

    public void PutFungiOn(Hexagon hex)
    {
        GameObject fungiObject = Instantiate(fungi, hex.transform.position + new Vector3(0, 0.001f, 0), Quaternion.LookRotation(Vector3.up * 90)) as GameObject;
        fungiObject.transform.parent = hex.gameObject.transform;
        //NGO checking
        if (hex.ngo != null)
            StartCoroutine(hex.ngo.StartProtecting());
    }

    public void SpawnFungi(Hexagon hex)
    {
        //get all the surrounding hexagons that contains a tree
        List<Hexagon> treesTiles = GetAccessibleTiles(hex);
        //If there's one or more tile with a tree on it
        if (treesTiles.Count > MinimumSpawnTrees)
            PutFungiOn(hex);
        else
        {
            //remove the spawned one

            //Browsing all the hexagons to get at least "MinimumSpawnTrees"
            for (int i = 0; i < Hexagons.Length; i++)
            {
                //Get the surroundings tiles for hex
                treesTiles = GetAccessibleTiles(Hexagons[i]);
                if (treesTiles.Count > MinimumSpawnTrees)
                {
                    PutFungiOn(Hexagons[i]);
                    return;
                }
            }
            //Spawn a new tree
            List<Hexagon> surroundingTiles = GetSurroundingTiles(hex);
            PutFungiOn(hex);
            int count = MinimumSpawnTrees - treesTiles.Count;
            for (int i = 0; i < surroundingTiles.Count; i ++ )
            {
                if(surroundingTiles[i].HexTree == null)
                {
                    TreeGenerator.SpawnSapling(surroundingTiles[i]);
                    count--;
                    if (count == 0)
                        return;
                }
            }
        }
    }

    public void BuildMap()
    {
        List<Hexagon> borders = new List<Hexagon>();

        _hexagons = new Hexagon[GridManager.instance.gridHeightInHexes * GridManager.instance.gridWidthInHexes];
        for (int x = 0; x < GridManager.instance.gridWidthInHexes; x++)
            for (int y = 0; y < GridManager.instance.gridHeightInHexes; y++)
            {
            _hexagons[x + y * GridManager.instance.gridWidthInHexes] = GridManager.instance.CreateHexagonAt(x, y);
            _hexagons[x + y * GridManager.instance.gridWidthInHexes].ClickEvent += new HexagonEventHandler(GridManager.instance.UserInteraction.OnHexagonClickedEvent);
                //Making the border array:
                if (x == 0 || y == 0 || x == GridManager.instance.gridWidthInHexes - 1 || y == GridManager.instance.gridHeightInHexes - 1)
                borders.Add(_hexagons[x + y * GridManager.instance.gridWidthInHexes]);
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
        #region Planter
        if(Time.time > _planterNextSpawn && !Planter.isPlanterWaiting)
        {
            _planterNextSpawn = Time.time + Mathf.Exp(Random.Range(_planterRndDown, _planterRndUp));
            _planterRndDown -= 0.2f;
            if (_planterRndDown <= 0)
                _planterRndDown = 2.6f;
            _planterRndUp -= 0.2f;
            if (_planterRndUp <= 0)
                _planterRndUp = 4f;


            GameObject planter = Instantiate(ResourcesManager.instance.Planter) as GameObject;
            planter.transform.SetParent(_planter);
            planter.GetComponent<Planter>().Spawn();
        }
        else if(Planter.isPlanterWaiting)
            _planterNextSpawn = Time.time + Mathf.Exp(Random.Range(_planterRndDown, _planterRndUp));

        #endregion
        #region Lumberjack
        if (Time.time > _lumberjackNextSpawn && !Lumberjack.isLumberjackWaiting)
        {
            _lumberjackNextSpawn = Time.time + Mathf.Exp(Random.Range(_lumberjackRndDown, _lumberjackRndUp));
            _lumberjackRndDown -= 0.2f;
            if (_lumberjackRndDown <= 0)
                _lumberjackRndDown = 2.2f;
            _lumberjackRndUp -= 0.2f;
            if (_lumberjackRndUp <= 0)
                _lumberjackRndUp = 3.8f;


            GameObject lumberjack = Instantiate(ResourcesManager.instance.Lumberjack) as GameObject;
            lumberjack.transform.SetParent(_lumberjack);
            lumberjack.GetComponent<Lumberjack>().Spawn();
        }
        else if(Lumberjack.isLumberjackWaiting)
            _lumberjackNextSpawn = Time.time + Mathf.Exp(Random.Range(_lumberjackRndDown, _lumberjackRndUp));
        #endregion

        #region NGO
        if (Time.time > _NGONextSpawn && !NGO.isNGOWaiting)
        {
            _NGONextSpawn = Time.time + Mathf.Exp(Random.Range(_lumberjackRndDown, _lumberjackRndUp));
            _NGORndDown -= 0.2f;
            if (_NGORndDown <= 0)
                _NGORndDown = 2.2f;
            _NGORndUp -= 0.2f;
            if (_NGORndUp <= 0)
                _NGORndUp = 3.8f;


            GameObject ngo = Instantiate(ResourcesManager.instance.NGO) as GameObject;
            ngo.transform.SetParent(_ngo);
            ngo.GetComponent<NGO>().Spawn();
        }
        else if(NGO.isNGOWaiting)
            _NGONextSpawn = Time.time + Mathf.Exp(Random.Range(_lumberjackRndDown, _lumberjackRndUp));
        #endregion
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
                surroundingHexs.Add(_hexagons[x + (y - 1) * GridManager.instance.gridWidthInHexes]);
            }
        }
        else if (!IsOutOfBounds(x - 1, y - 1))
        {
            surroundingHexs.Add(_hexagons[(x - 1) + (y - 1) * GridManager.instance.gridWidthInHexes]);
        }


        //Left tile
        if (!IsOutOfBounds(x - 1, y))
        {
            surroundingHexs.Add(_hexagons[(x - 1) + y * GridManager.instance.gridWidthInHexes]);
        }
        //Left down tile
        if (y % 2 != 0)
        {
            if (!IsOutOfBounds(x, y + 1))
            {
                surroundingHexs.Add(_hexagons[x + (y + 1) * GridManager.instance.gridWidthInHexes]);
            }
        }
        else if (!IsOutOfBounds(x - 1, y + 1))
        {
            surroundingHexs.Add(_hexagons[(x - 1) + (y + 1) * GridManager.instance.gridWidthInHexes]);
        }

        #endregion
        #region RIGHT SIDE
        //Right down tile
        if (y % 2 == 0)
        {
            if (!IsOutOfBounds(x, y + 1))
                surroundingHexs.Add(_hexagons[x + (y + 1) * GridManager.instance.gridWidthInHexes]);
        }
        else if (!IsOutOfBounds(x + 1, y + 1))
            surroundingHexs.Add(_hexagons[(x + 1) + (y + 1) * GridManager.instance.gridWidthInHexes]);
        //Right tile
        if (!IsOutOfBounds(x + 1, y))
            surroundingHexs.Add(_hexagons[(x + 1) + y * GridManager.instance.gridWidthInHexes]);

        if (y % 2 == 0)
        {
            if (!IsOutOfBounds(x, y - 1))
                surroundingHexs.Add(_hexagons[x + (y - 1) * GridManager.instance.gridWidthInHexes]);
        }
        else if (!IsOutOfBounds(x + 1, y - 1))
            surroundingHexs.Add(_hexagons[(x + 1) + (y - 1) * GridManager.instance.gridWidthInHexes]);
        #endregion

        return surroundingHexs;
    }

    private bool IsOutOfBounds(int x, int y)
    {
        return (y < 0 || x < 0 || x >= GridManager.instance.gridWidthInHexes || y >= GridManager.instance.gridHeightInHexes);
    }

    public List<Hexagon> GetAccessibleTiles(Hexagon hex)
    {
        List<Hexagon> surroundingTrees = new List<Hexagon>();
        foreach(Hexagon h in GetSurroundingTiles(hex))
        {
            if (h.HexTree != null)
                surroundingTrees.Add(h);
        }
        return surroundingTrees;
    }
}
