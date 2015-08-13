using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected };
public enum TreeType { Sapling, SmallTree, BigTree };

public class TreeClass : MonoBehaviour
{
    public TreeState State;
    public TreeType Type;

    private float _nextEventTime = 0f;

    public void StartTreeGrowth()
    {

    }
}
