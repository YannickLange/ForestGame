using UnityEngine;
using System.Collections;

public class TreeGenerator {

    public float density = 0.5f;
    public GameObject[] TreeTypes;

    private GameObject[] Hexagons;


    public TreeGenerator(GameObject[] trees)
    {
        TreeTypes = trees;
        Hexagons = GameObject.FindGameObjectsWithTag("Hexagon");
        GenerateTrees();
    }

    void GenerateTrees()
    {
        //Game object which is the parent of all the hex tiles
        GameObject Forest = new GameObject("Forest");

        for (int i = 0; i < Hexagons.Length; i++)
        {
                if(Random.value <= density)
                {
                    
                    GameObject tree = (GameObject)GameObject.Instantiate(TreeTypes[0], Hexagons[i].transform.position , Quaternion.Euler(new Vector3(0, 0, 0)));
                    Transform child = tree.transform.GetChild(0);
                    child.transform.position += new Vector3(0, child.GetComponent<MeshFilter>().mesh.bounds.size.y / (2 / child.localScale.y));
                    tree.transform.parent = Forest.transform;
                }
        }
    }


}
