using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject resourcesManager;
    public GameObject gridManager;
    public GameObject map;
    public GameObject moveButton;
    public GameObject infectButton;
    public GameObject inputManager;

    void Awake()
    {
        if (GridManager.instance == null)
            Instantiate(gridManager);
        if (Map.instance == null)
            Instantiate(map);
        if (ResourcesManager.instance == null)
            Instantiate(resourcesManager);

        GridManager.instance.GetComponent<GridManager>().InfectButton = infectButton.GetComponent<InfectButton>();
        GridManager.instance.GetComponent<GridManager>().MoveButton = moveButton.GetComponent<MoveButton>();
    }
}
