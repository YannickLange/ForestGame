using UnityEngine;
using System.Collections.Generic;
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
                int treetype = Random.Range(0, ResourcesManager.instance.TreeTypes.Length - 2);
                Map.instance.Hexagons[i].PlantTree((TreeType)treetype);
            }
        }
        //TODO: minimum amount of trees
    }

    public static void SpawnSapling(Hexagon hex)
    {
        hex.PlantTree(TreeType.Sapling);
    }


}
