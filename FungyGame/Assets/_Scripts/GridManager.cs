using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    //following public variable is used to store the hex model prefab;
    //instantiate it by dragging the prefab on this variable using unity editor
    public GameObject Hex;

    //next two variables can also be instantiated using unity editor
    public int gridWidthInHexes = 10;
    public int gridHeightInHexes = 10;

    //Hexagon tile width and height in game world
    private float hexWidth;
    private float hexHeight;

    private Transform hexGridGO; //Parent object of all hex tiles

    public InfectButton InfectButton { get; set; }

    //GridManager singleton
    public static GridManager instance = null;
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
        hexGridGO = new GameObject("HexGrid").transform;
        hexGridGO.SetParent(transform);
        DontDestroyOnLoad(gameObject);


        //Get the size of the hexagon prefab
        setSizes();
    }

    //Method to initialise Hexagon width and height
    void setSizes()
    {
        //renderer component attached to the Hex prefab is used to get the current width and height
        hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<Renderer>().bounds.size.y;
    }

    //Method to calculate the position of the first hexagon tile
    //The center of the hex grid is (0,0,0)
    Vector3 calcInitPos()
    {
        Vector3 initPos;
        //the initial position will be in the left upper corner
        initPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, 0,
                              gridHeightInHexes / 2f * hexHeight - hexHeight / 2);

        return initPos;
    }

    //method used to convert hex grid coordinates to game world coordinates
    public Vector3 calcWorldCoord(Vector2 gridPos)
    {
        //Position of the first hex tile
        Vector3 initPos = calcInitPos();
        //Every second row is offset by half of the tile width
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = initPos.x + offset + gridPos.x * hexWidth;
        //Every new line is offset in z direction by 3/4 of the hexagon height
        float z = initPos.z - gridPos.y * hexHeight * 0.75f;
        return new Vector3(x, 0, z);
    }

    //Finally the method which initialises and positions all the tiles
    void createGrid()
    {
        for (int y = 0; y < gridHeightInHexes; y++)
        {
            for (int x = 0; x < gridWidthInHexes; x++)
            {
                //GameObject assigned to Hex public variable is cloned
                GameObject hex = (GameObject)Instantiate(Hex);
                hex.transform.eulerAngles = new Vector3(0, 0, 0);
                hex.name = "Hex x:" + x + " y:" + y;
                //Current position in grid
                Vector2 gridPos = new Vector2(x, y);
                hex.transform.position = calcWorldCoord(gridPos);
                hex.transform.parent = hexGridGO.transform;
            }
        }
    }

    /// <summary>
    /// Creates an hexagon tile at the selected (x,y) position
    /// </summary>
    /// <param name="x">X position of the desired hexagon</param>
    /// <param name="y">Y position of the desired hexagon</param>
    public Hexagon CreateHexagonAt(int x, int y)
    {
        //GameObject assigned to Hex public variable is cloned
        GameObject hex = (GameObject)Instantiate(Hex);
        Transform hexTr = hex.transform;
        Hexagon hexScript = hex.GetComponent<Hexagon>();
        hexTr.eulerAngles = new Vector3(0, 0, 0);
        //Current position in grid
        Vector2 gridPos = new Vector2(x, y);
        hexTr.position = calcWorldCoord(gridPos);
        hexTr.name = string.Format("_hexTile_{0}_{1}", x, y);
        hex.transform.SetParent(hexGridGO.transform);

        //Set hexagon script values
        hexScript.SetPosition(x, y);
        return hexScript;
    }
}