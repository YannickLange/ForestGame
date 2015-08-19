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

    public float Plantertimer = 0f;
    public float Lumberjacktimer = 0f;
    public float NGOtimer = 0f;
    public float timerSpeedMultiplier = 1f;

    //Readonly hexagons
    public Hexagon[] Hexagons
    {
        get
        {
            return _hexagons;
        }
    }

    //Spawner variables
    public float _planterSpawn = 20f;
    public float _planterRndUp = 10f;
    public float _planterRndDown = 30f;

    public float _lumberjackSpawn = 20f;
    public float _lumberjackRndUp = 10f;
    public float _lumberjackRndDown = 30f;

    public float _NGOSpawn = 20f;
    public float _NGORndUp = 10f;
    public float _NGORndDown = 30f;
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
        _planterSpawn = Random.Range(_planterRndDown, _planterRndUp);
        _lumberjack = GameObject.Find("Lumberjacks").transform;
        _lumberjackSpawn = Random.Range(_lumberjackRndDown, _lumberjackRndUp);
        _ngo = GameObject.Find("NGO").transform;
        _NGOSpawn = Random.Range(_NGORndDown, _NGORndUp);
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
            StartCoroutine(hex.ngo.PickupNGO());
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
        #region Timer
        Plantertimer += 1f * Time.deltaTime * timerSpeedMultiplier;
        NGOtimer += 1f * Time.deltaTime * timerSpeedMultiplier;
        Lumberjacktimer += 1f * Time.deltaTime * timerSpeedMultiplier;
        #endregion
        #region Planter
        if (Plantertimer >= _planterSpawn && !Planter.isPlanterWaiting)
        {
            Plantertimer = 0f;
            _planterSpawn = Random.Range(_planterRndDown, _planterRndUp);

            GameObject planter = Instantiate(ResourcesManager.instance.Planter) as GameObject;
            planter.transform.SetParent(_planter);
            planter.GetComponent<Planter>().Spawn();
        }
        else if (Planter.isPlanterWaiting)
            Plantertimer = 0f;

        #endregion
        #region Lumberjack
        if (Lumberjacktimer >= _lumberjackSpawn && !Lumberjack.isLumberjackWaiting)
        {
            _lumberjackSpawn = Random.Range(_lumberjackRndDown, _lumberjackRndUp);

            GameObject lumberjack = Instantiate(ResourcesManager.instance.Lumberjack) as GameObject;
            lumberjack.transform.SetParent(_lumberjack);
            lumberjack.GetComponent<Lumberjack>().Spawn();
        }
        else if (Lumberjack.isLumberjackWaiting)
            Lumberjacktimer = 0f;
        #endregion

        #region NGO
        if (NGOtimer > _NGOSpawn && !NGO.isNGOWaiting)
        {
            _NGOSpawn = Random.Range(_NGORndDown, _NGORndUp);

            GameObject ngo = Instantiate(ResourcesManager.instance.NGO) as GameObject;
            ngo.transform.SetParent(_ngo);
            ngo.GetComponent<NGO>().Spawn();
        }
        else if (NGO.isNGOWaiting)
            NGOtimer= 0f;
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
