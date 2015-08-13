using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected };
public enum TreeType { Sapling, SmallTree, BigTree, DeadTree };

public class TreeClass : MonoBehaviour
{
    //Global timer values:
    public float DeltaTime = 5;
    public float Level4To1 = 30;
    public float Level1To2 = 30;
    public float Level2To3 = 30;
    public float Level3To4 = 30;

    public TreeState State;
    public TreeType Type;

    private float _nextEventTime = 0f;
    private bool _processStarted = false;

    //Cached components
    private Renderer _thisRenderer;
    private Transform _thisTransform;

    void Awake()
    {
        _thisTransform = transform;
        _thisRenderer = _thisTransform.GetChild(0).GetComponent<Renderer>();
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
                _nextEventTime = Time.time + Random.Range(Level3To4 - DeltaTime, Level3To4 + DeltaTime);
                break;
        }
        _processStarted = true;
    }

    void Update()
    {
        if(_processStarted)
        {
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
                _thisRenderer.material = ResourcesManager.instance.Tree2Material; //Change the current material
                _nextEventTime = Time.time + Random.Range(Level1To2 - DeltaTime, Level1To2 + DeltaTime); //Set the next event time value
                Type = TreeType.SmallTree; //Update the tree type
                break;
            case TreeType.SmallTree:
                _thisRenderer.material = ResourcesManager.instance.Tree3Material; //Change the current material
                _nextEventTime = Time.time + Random.Range(Level2To3 - DeltaTime, Level2To3 + DeltaTime); //Set the next event time value
                Type = TreeType.BigTree; //Update the tree type
                break;
            case TreeType.BigTree:
                _thisRenderer.material = null; //Change the current material
                _nextEventTime = Time.time + Random.Range(Level3To4 - DeltaTime, Level3To4 + DeltaTime); //Set the next event time value
                Type = TreeType.Sapling; //Update the tree type
                break;
            case TreeType.DeadTree:
                _thisRenderer.material = ResourcesManager.instance.Tree1Material; //Change the current material
                _nextEventTime = Time.time + Random.Range(Level4To1 - DeltaTime, Level4To1 + DeltaTime); //Set the next event time value
                Type = TreeType.Sapling; //Update the tree type
                break;
        }
    }
}
