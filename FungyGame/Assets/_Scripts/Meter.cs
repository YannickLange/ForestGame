using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour
{

    private GameObject Indicator;

    float fungusCount = 3;
    float forestCount = 3;
    public Reset r { get; set; }

    void Awake()
    {
        r = GetComponent<Reset>();
    }

    // Use this for initialization
    void Start ()
    {
        Indicator = this.gameObject.transform.GetChild(0).gameObject;
    }

    public void Forest (float dmg) {
        ++forestCount;
    }

    public void Fungus (float dmg) {
        ++fungusCount;
    }

    void Update ()
    {
        float aliveTrees = 0;
        float totalTrees = 0; 

        foreach (var hexagon in Map.instance.Hexagons)
        {
            //Debug.Log(hexagon.HexTree);
            if (hexagon.HexTree != null)
            {
                ++totalTrees;
                if (hexagon.HexTree.State == TreeState.Alive)
                {
                    ++aliveTrees;
                }
            }
        }

        float unitSize = 200f / totalTrees; // divides by 200
        float aliveTreesSize = aliveTrees * unitSize;
        
        Indicator.transform.localPosition = new Vector2(-100 + aliveTreesSize, 0);

        if(aliveTrees <= 0) {
            //GameOver();
        }
    }
}
