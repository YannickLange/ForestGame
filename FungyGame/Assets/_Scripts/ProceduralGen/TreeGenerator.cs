using UnityEngine;
using System.Collections;

public class TreeGenerator
{
    public float density = 0.5f;

    public TreeGenerator()
    {
        GenerateTrees();
    }

    void GenerateTrees()
    {
        //Game object which is the parent of all the hex tiles
        GameObject Forest = new GameObject("Forest");


        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
            if (Random.value <= density)
            {
                int treetype = Random.Range(0, ResourcesManager.instance.TreeTypes.Length - 1);
                GameObject tree = GameObject.Instantiate(ResourcesManager.instance.TreeTypes[treetype], Map.instance.Hexagons[i].transform.position, Quaternion.identity) as GameObject;
                tree.transform.parent = Forest.transform;
                Map.instance.Hexagons[i].HexTree = tree.GetComponent<TreeClass>();
                tree.GetComponent<TreeClass>().occupiedHexagon = Map.instance.Hexagons[i];
            }
        }
    }

    public static void SpawnSapling(Hexagon hex)
    {
        GameObject tree = GameObject.Instantiate(ResourcesManager.instance.TreeTypes[0], hex.transform.position, Quaternion.identity) as GameObject;
        tree.transform.parent = GameObject.Find("Forest").transform;
        hex.HexTree = tree.GetComponent<TreeClass>();
    }


}
