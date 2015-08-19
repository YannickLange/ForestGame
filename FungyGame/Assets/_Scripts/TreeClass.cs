using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected, Dead };
public enum TreeType { Sapling = 0, SmallTree = 1, BigTree = 2, DeadTree = 3, CutTree = 4 }; //DEADTREE must be last!

public class TreeClass : MonoBehaviour
{
    public float growTime = 10f;
    public float randomGrowTimeRange = 5f;

    public TreeState State;
    public TreeType Type;

	public float _nextEventTime = 0f;
    public bool _processStarted = false;
    public Hexagon occupiedHexagon {get; set;}
    public GameObject _treeInfectPrefab;

    //Cached components
    private SpriteRenderer _spriteRenderer;
    private TreeClass _treeClassScript;
    public Fungi _infection;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _treeClassScript = GetComponent<TreeClass>();
        _treeInfectPrefab = (GameObject)Resources.Load("InfectLoadingBar");
        _nextEventTime = Time.time + Random.Range(growTime, growTime + randomGrowTimeRange);
        _processStarted = true;
    }

    public void ReplaceTree(int newType)
    {
        //create the new tree
        GameObject tree = Instantiate(ResourcesManager.instance.TreeTypes[newType], gameObject.transform.position, gameObject.transform.rotation) as GameObject;
        TreeClass newTreeClassScript = tree.GetComponent<TreeClass>();
        //Make the forest the parent
        tree.transform.parent = GameObject.Find("Forest").transform;
        //Make sure the hexagon and the tree now know their significant other
        newTreeClassScript.occupiedHexagon = _treeClassScript.occupiedHexagon;
        newTreeClassScript.occupiedHexagon.HexTree = newTreeClassScript;
        if (newTreeClassScript.Type == TreeType.DeadTree)
            GridManager.instance.Meter.Fungus(5);
        //destroy the original
        GameObject.Destroy(this.gameObject);

        GridManager.instance.UserInteraction.updateView();
    }
}
