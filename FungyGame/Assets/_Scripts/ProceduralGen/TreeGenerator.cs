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

        TreeClass.Positions = new Vector3[4];
        TreeClass.Scales = new Vector3[4];

        //Get the local pistion of the trees (used for growing function)
        for (int i = 0; i < ResourcesManager.instance.TreeTypes.Length; i++ )
        {
            GameObject tree = GameObject.Instantiate(ResourcesManager.instance.TreeTypes[i]) as GameObject;
            Transform child = tree.transform.GetChild(0);
            child.position += new Vector3(0, child.GetComponent<MeshFilter>().mesh.bounds.size.y / (2 / child.localScale.y));
            TreeClass.Positions[i] = child.localPosition;
            TreeClass.Scales[i] = child.transform.localScale;
            GameObject.Destroy(tree);
        }

        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
            if (Random.value <= density)
            {
                int treetype = Random.Range(0, ResourcesManager.instance.TreeTypes.Length - 1);
                GameObject tree = GameObject.Instantiate(ResourcesManager.instance.TreeTypes[treetype], Map.instance.Hexagons[i].transform.position, Quaternion.identity) as GameObject;
                Transform child = tree.transform.GetChild(0);
                child.position += new Vector3(0, child.GetComponent<MeshFilter>().mesh.bounds.size.y / (2 / child.localScale.y));
                tree.transform.parent = Forest.transform;
                Map.instance.Hexagons[i].HexTree = tree.GetComponent<TreeClass>();
                Map.instance.Hexagons[i].HexTree.StartTreeGrowth();
            }
        }
    }

    public static void SpawnSapling(Hexagon hex)
    {
        GameObject tree = GameObject.Instantiate(ResourcesManager.instance.TreeTypes[0], hex.transform.position, Quaternion.identity) as GameObject;
        Transform child = tree.transform.GetChild(0);
        child.position += new Vector3(0, child.GetComponent<MeshFilter>().mesh.bounds.size.y / (2 / child.localScale.y));
        tree.transform.parent = GameObject.Find("Forest").transform;
        hex.HexTree = tree.GetComponent<TreeClass>();
        hex.HexTree.StartTreeGrowth();
    }


}
