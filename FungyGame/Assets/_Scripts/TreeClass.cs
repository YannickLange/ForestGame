using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected, Dead };
public enum TreeType { Sapling = 0, SmallTree = 1, BigTree = 2, DeadTree = 3, CutTree = 4 }; //DEADTREE must be last!

public class TreeClass : MonoBehaviour
{
    public TreeState State;
    public TreeType Type;

    public bool _processStarted = false;
    public Hexagon occupiedHexagon {get; set;}


    void Awake()
    {
        _processStarted = true;
    }
}
