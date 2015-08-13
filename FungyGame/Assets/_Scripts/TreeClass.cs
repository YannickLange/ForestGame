using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected, Dead };
public enum TreeType { Sapling, SmallTree, BigTree, DeadTree };

public class TreeClass : MonoBehaviour
{
    public static Vector3[] Positions;
    public static Vector3[] Scales;
    //Global timer values:
    public float DeltaTime = 20;
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
        switch (Type)
        {
            case TreeType.Sapling:
                _nextEventTime = Time.time + Random.Range(TimerValues[0] - DeltaTime, TimerValues[0] + DeltaTime);
                break;
            case TreeType.SmallTree:
                _nextEventTime = Time.time + Random.Range(TimerValues[1] - DeltaTime, TimerValues[1] + DeltaTime);
                break;
            case TreeType.BigTree:
                _nextEventTime = Time.time + Random.Range(TimerValues[2] - DeltaTime, TimerValues[2] + DeltaTime);
                break;
            case TreeType.DeadTree:
                _nextEventTime = Time.time + Random.Range(TimerValues[3] - DeltaTime, TimerValues[3] + DeltaTime);
                break;
        }
        _processStarted = true;
    }

    void Update()
    {
        if(_processStarted)
        {
            //CheckState();
            if(Time.time >= _nextEventTime && State == TreeState.Alive)
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
                _thisTransform.localScale = Scales[1];
                _thisTransform.localPosition = Positions[1];
                _nextEventTime = Time.time + Random.Range(TimerValues[0] - DeltaTime, TimerValues[0] + DeltaTime); //Set the next event time value
                Type = TreeType.SmallTree; //Update the tree type
                break;
            case TreeType.SmallTree:
                _thisRenderer.material = ResourcesManager.instance.Tree3Mat; //Change the current material
                _thisTransform.localScale = Scales[2];
                _thisTransform.localPosition = Positions[2];
                _nextEventTime = Time.time + Random.Range(TimerValues[1] - DeltaTime, TimerValues[1] + DeltaTime); //Set the next event time value
                Type = TreeType.BigTree; //Update the tree type
                break;
            case TreeType.BigTree:
                _thisRenderer.material = ResourcesManager.instance.Tree4Mat; //Change the current material
                _thisTransform.localScale = Scales[3];
                _thisTransform.localPosition = Positions[3];
                _nextEventTime = Time.time + Random.Range(TimerValues[2] - DeltaTime, TimerValues[2] + DeltaTime); //Set the next event time value
                Type = TreeType.DeadTree; //Update the tree type
                break;
            case TreeType.DeadTree:
                _thisRenderer.material = ResourcesManager.instance.Tree1Mat; //Change the current material
                _thisTransform.localScale = Scales[0];
                _thisTransform.localPosition = Positions[0];
                _nextEventTime = Time.time + Random.Range(TimerValues[3] - DeltaTime, TimerValues[3] + DeltaTime); //Set the next event time value
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
        }
    }
}
