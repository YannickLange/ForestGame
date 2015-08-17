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

    //Cached components
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _nextEventTime = Time.time + Random.Range(growTime, growTime + randomGrowTimeRange);
        _processStarted = true;
    }

    void Update()
    {
        if(_processStarted)
        {
            CheckState();
        }
    }

    /// <summary>
    /// Set the next state of the tree
    /// </summary>
    private void GrowTree()
	{
		int typeValue = (int)Type;
        if (typeValue >= (int)TreeType.BigTree) //TODO: Change this back to deadtree when they exist
			return;

		int newType = typeValue + 1;
        _nextEventTime = Time.time + Random.Range(growTime, growTime + randomGrowTimeRange); //Set the next event time value
        GameObject tree = (GameObject)Instantiate(ResourcesManager.instance.TreeTypes[newType], gameObject.transform.position, gameObject.transform.rotation);
        tree.transform.parent = GameObject.Find("Forest").transform;
        GameObject.Destroy(this.gameObject);
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
}
