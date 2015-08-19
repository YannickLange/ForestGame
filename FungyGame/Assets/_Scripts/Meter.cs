using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour
{
    public int MaxLivingTrees = 5;
    public int MaxScore = 1;
    private float Score = 0;

    private GameObject Indicator;

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
    }

    public void Fungus (float dmg) {
    }

    void Update ()
    {
        float aliveTrees = 0;
        float totalTrees = 0; 
        float totalFungi = 0;

        foreach (var hexagon in Map.instance.Hexagons)
        {
            //Debug.Log(hexagon.HexTree);
            if (hexagon.HexState != HexagonState.Empty)
            {
                ++totalTrees;
                if (hexagon.Type != TreeType.DeadTree && hexagon.Type != TreeType.CutTree)
                {
                    ++aliveTrees;
                }

                if(hexagon.HexState == HexagonState.CurrentlyInfectingTreeAndFungi ||
                   hexagon.HexState == HexagonState.DeadWoodAndFungi ||
                   hexagon.HexState == HexagonState.SaplingAndFungi ||
                   hexagon.HexState == HexagonState.TreeAndFungi)
                {
                    ++totalFungi;
                }
            }
        }
       
        float unitSize = 200f / totalTrees; // divides by 200
        float aliveTreesSize = aliveTrees * unitSize;
        Indicator.transform.localPosition = new Vector2(-100 + aliveTreesSize, 0);

        //calculate scores 
        float aliveTreesPercentage = (aliveTrees) / totalTrees;
        float addedScore = 0.5f - aliveTreesPercentage;
        addedScore = Mathf.Abs(addedScore);
        addedScore = MaxScore - addedScore;
        addedScore *= Time.deltaTime;
        Score += addedScore;

        Debug.Log("Highscore: " + Score + " added score: " + addedScore);

      
        if(aliveTrees <= MaxLivingTrees || totalFungi <= 0) {
            //GameOver();
            Debug.Log("GAMEOVER!!!!!!!!!!!!!!!!!!");
            //HighScores.SaveHighScore("This Is You", Score);
            Application.LoadLevel(1);
        }
    }
}
