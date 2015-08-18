using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected, Dead };
public enum TreeType { Sapling = 0, SmallTree = 1, BigTree = 2, DeadTree = 3 }; //DEADTREE must be last!

public class TreeClass : MonoBehaviour
{
    public float growTime = 10f;
    public float randomGrowTimeRange = 5f;

    public TreeState State;
    public TreeType Type;

    private float _nextEventTime = 0f;
    private bool _processStarted = false;
    public Hexagon occupiedHexagon {get; set;}
    private GameObject _treeInfectPrefab;

    //Cached components
    private SpriteRenderer _spriteRenderer;
    private TreeClass _treeClassScript;
    private Fungi _infection;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _treeClassScript = GetComponent<TreeClass>();
        _treeInfectPrefab = (GameObject)Resources.Load("InfectLoadingBar");
        _nextEventTime = Time.time + Random.Range(growTime, growTime + randomGrowTimeRange);
        _processStarted = true;
    }

    void Update()
    {
        if(_processStarted)
        {
            CheckState();
        }
        if(_infection != null)
        {
            if (_infection.stage == _infection.maxStage)
            {
                Debug.Log("Tree should be dead");
                occupiedHexagon.Fungi.stage = 0;
                //Does not do anything, just here for completion sake
                State = TreeState.Dead;
                Type = TreeType.DeadTree;
                //End of useless code

                ReplaceTree((int)TreeType.DeadTree);
                Destroy(_infection.gameObject);
            }
        }
    }

    /// <summary>
    /// Set the next state of the tree
    /// </summary>
    private void GrowTree()
	{
		int typeValue = (int)Type;
        if (typeValue >= (int)TreeType.DeadTree) //TODO: Change this back to deadtree when they exist
			return;

		int newType = typeValue + 1;
        _nextEventTime = Time.time + Random.Range(growTime, growTime + randomGrowTimeRange); //Set the next event time value
        ReplaceTree(newType);
    }
    
	private void CheckState()
	{
        switch (State)
        {
            case TreeState.Alive:
				if(Time.time >= _nextEventTime)
				{
					GrowTree();
				}
			break;
        }
    }

    public void InfectTree()
    {
        GameObject treeInfect = (GameObject)Instantiate(_treeInfectPrefab, transform.position + new Vector3(0f, 0f, 0.01f), transform.rotation);
        treeInfect.transform.parent = transform;
        _infection = treeInfect.GetComponent<Fungi>();
        State = TreeState.Infected;
    }

    public void ReplaceTree(int newType)
    {
        //create the new tree
        GameObject tree = (GameObject)Instantiate(ResourcesManager.instance.TreeTypes[newType], gameObject.transform.position, gameObject.transform.rotation);
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
    }
}
