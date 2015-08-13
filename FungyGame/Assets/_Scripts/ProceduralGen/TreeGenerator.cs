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
        
        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
                if(Random.value <= density)
                {
                    int treetype = Random.Range(0, TreeTypes.Length);
                    GameObject tree = (GameObject)GameObject.Instantiate(TreeTypes[treetype], Map.instance.Hexagons[i].transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                    Transform child = tree.transform.GetChild(0);
                    child.transform.position += new Vector3(0, child.GetComponent<MeshFilter>().mesh.bounds.size.y / (2 / child.localScale.y));
                    tree.transform.parent = Forest.transform;
                    Map.instance.Hexagons[i].HexTree = tree.GetComponent<Tree>();
                }
        }
    }


}
