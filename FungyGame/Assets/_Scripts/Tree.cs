using UnityEngine;
using System.Collections;

public enum TreeState { Alive, Infected };
public enum TreeType { Sapling, SmallTree, BigTree };

public class Tree : MonoBehaviour
{
    public TreeState State;
    public TreeType Type;
    private float _timeSpawn; //Moment when the tree is spawned

    void Start()
    {
        _timeSpawn = Time.time;
    }
}
