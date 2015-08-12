using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gridManager;
    public GameObject map;

    void Awake()
    {
        if (GridManager.instance == null)
            Instantiate(gridManager);
        if (Map.instance == null)
            Instantiate(map);
    }
}
