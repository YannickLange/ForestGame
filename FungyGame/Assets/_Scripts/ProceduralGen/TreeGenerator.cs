using UnityEngine;
using System.Collections;

public class TreeGenerator
{
    public float density = 0.5f;
    public GameObject[] TreeTypes;

    public TreeGenerator(GameObject[] trees)
    {
        TreeTypes = trees;
        GenerateTrees();
    }

    void GenerateTrees()
    {
        //Game object which is the parent of all the hex tiles
        GameObject Forest = new GameObject("Forest");

        TreeClass.Positions = new Vector3[4];
        TreeClass.Scales = new Vector3[4];

        //Get the local pistion of the trees (used for growing function)
        for (int i = 0; i < 4; i++ )
        {
            GameObject tree = (GameObject)GameObject.Instantiate(TreeTypes[i]);
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
                int treetype = Random.Range(0, TreeTypes.Length);
                GameObject tree = (GameObject)GameObject.Instantiate(TreeTypes[treetype], Map.instance.Hexagons[i].transform.position, Quaternion.identity);
                Transform child = tree.transform.GetChild(0);
                child.position += new Vector3(0, child.GetComponent<MeshFilter>().mesh.bounds.size.y / (2 / child.localScale.y));
                tree.transform.parent = Forest.transform;
                Map.instance.Hexagons[i].HexTree = tree.GetComponent<TreeClass>();
                Map.instance.Hexagons[i].HexTree.StartTreeGrowth();
            }
        }
    }


}
