using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected, Dead };
public enum TreeType { Sapling, SmallTree, BigTree, DeadTree };

public class TreeClass : MonoBehaviour
{
    //Global timer values:
    public float DeltaTime = 5;
    public float Level4To1 = 20;
    public float Level1To2 = 20;
    public float Level2To3 = 20;
    public float Level3To4 = 120;
	public float DeadTime = 20;
    //ar

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
		SetTreeState(TreeState.Infected);
    }

    /// <summary>
    /// Start the growth processus
    /// </summary>
    public void StartTreeGrowth()
    {
        switch (Type)
        {
            case TreeType.Sapling:
                _nextEventTime = Time.time + Random.Range(Level4To1 - DeltaTime, Level4To1 + DeltaTime);
                break;
            case TreeType.SmallTree:
                _nextEventTime = Time.time + Random.Range(Level1To2 - DeltaTime, Level1To2 + DeltaTime);
                break;
            case TreeType.BigTree:
                _nextEventTime = Time.time + Random.Range(Level2To3 - DeltaTime, Level2To3 + DeltaTime);
                break;
            case TreeType.DeadTree:
                _nextEventTime = Time.time + Random.Range(Level4To1 - DeltaTime, Level4To1 + DeltaTime);
                break;
        }
        _processStarted = true;
    }

    void Update()
    {
        if(_processStarted)
        {
            CheckState();
            if(Time.time >= _nextEventTime)
            {
                GrowTree();
            }
        }
    }

    /// <summary>
    /// Set the nex state of the tree
    /// </summary>
    private void GrowTree()
    {
        switch (Type)
        {
            case TreeType.Sapling:
                _thisRenderer.material = ResourcesManager.instance.Tree2Mat; //Change the current material
                _thisTransform.localScale = ResourcesManager.instance.Tree2.localScale;
                _thisTransform.position = ResourcesManager.instance.Tree2.position;
                _nextEventTime = Time.time + Random.Range(Level1To2 - DeltaTime, Level1To2 + DeltaTime); //Set the next event time value
                Type = TreeType.SmallTree; //Update the tree type
                break;
            case TreeType.SmallTree:
                _thisRenderer.material = ResourcesManager.instance.Tree3Mat; //Change the current material
                _thisTransform.localScale = ResourcesManager.instance.Tree3.localScale;
                _thisTransform.position = ResourcesManager.instance.Tree3.position;
                _nextEventTime = Time.time + Random.Range(Level2To3 - DeltaTime, Level2To3 + DeltaTime); //Set the next event time value
                Type = TreeType.BigTree; //Update the tree type
                break;
            case TreeType.BigTree:
                _thisRenderer.material = ResourcesManager.instance.Tree4Mat; //Change the current material
                _thisTransform.localScale = ResourcesManager.instance.Tree4.localScale;
                _thisTransform.position = ResourcesManager.instance.Tree4.position;
                _nextEventTime = Time.time + Random.Range(Level3To4 - DeltaTime, Level3To4 + DeltaTime); //Set the next event time value
                Type = TreeType.Sapling; //Update the tree type
                break;
            case TreeType.DeadTree:
                _thisRenderer.material = ResourcesManager.instance.Tree1Mat; //Change the current material
                _thisTransform.localScale = ResourcesManager.instance.Tree1.localScale;
                _thisTransform.position = ResourcesManager.instance.Tree1.position;
                _nextEventTime = Time.time + Random.Range(Level4To1 - DeltaTime, Level4To1 + DeltaTime); //Set the next event time value
                Type = TreeType.Sapling; //Update the tree type
                break;
        }
    }
    
	private void CheckState()
	{
        switch (State)
        {
            case TreeState.Alive:

                break;
			case TreeState.Infected:
				if(Time.time >= _nextEventTime)
				{
					TreeDying();
				}

				break;
			case TreeState.Dead:
				
				break;

        }
    }

	public void SetTreeState(TreeState newState)
	{
		State = newState;
		switch (State)
		{
		case TreeState.Infected:
			_nextEventTime = Time.time + DeadTime;
			break;
		}
	}

	private void TreeDying()
	{
		_thisRenderer.material = ResourcesManager.instance.TreeDeadMat;
		State = TreeState.Dead;
	}
}
