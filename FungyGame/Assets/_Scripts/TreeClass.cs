using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected, Dead };
public enum TreeType { Sapling = 0, SmallTree = 1, BigTree = 2, DeadTree = 3 }; //DEADTREE must be last!

public class TreeClass : MonoBehaviour
{
    public static Vector3[] Positions;
    public static Vector3[] Scales;

    public float DeltaTime = 1;
	public float[] TimerValues = { 60, 60, 60, 240 }; //[SAPLING],[SMALLTREE],[BIGTREE],[DEADTREE] 

    public TreeState State;
    public TreeType Type;

    private float _nextEventTime = 0f;
    private bool _processStarted = false;

    //Cached components
    private Renderer _thisRenderer;
    private Transform _thisTransform;

    void Awake()
    {
        _thisTransform = transform.GetChild(0);
        _thisRenderer = _thisTransform.GetComponent<Renderer>();
    }

    /// <summary>
    /// Start the growth processus
    /// </summary>
    public void StartTreeGrowth()
    {
		_nextEventTime = Time.time + Random.Range(TimerValues[(int)Type] - DeltaTime, TimerValues[(int)Type] + DeltaTime);
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
    /// Set the nex state of the tree
    /// </summary>
    private void GrowTree()
	{
		int typeValue = (int)Type;
		if (typeValue >= (int)TreeType.DeadTree)
			return;

		int newType = typeValue + 1;
		_nextEventTime = Time.time + Random.Range(TimerValues[typeValue] - DeltaTime, TimerValues[typeValue] + DeltaTime); //Set the next event time value
		SetTreeType(newType);
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

	public void SetTreeState(TreeState newState)
	{
		switch (newState)
		{
			case TreeState.Alive:
				break;
			case TreeState.Infected:
				break;
			case TreeState.Dead:
				SetTreeType(TreeType.DeadTree);
				break;
		}
		State = newState;
	}

	public void SetTreeType(TreeType newType)
	{
		SetTreeType((int)newType);
	}

	public void SetTreeType(int newType)
	{
		TreeType treeType = (TreeType)newType;
		_thisRenderer.material = ResourcesManager.instance.TreeMat[newType];
		_thisTransform.localScale = Scales[newType];
		_thisTransform.localPosition = Positions[newType];
		Type = treeType;
	}
}
