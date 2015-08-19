using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour
{
    public int MaxLivingTrees = 5;
    public int MaxScore = 1;
    public float Score = 0;

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
        float treesKilledByFungi = 0;
        float totalTrees = 0; 
        float totalFungi = 0;
        float aliveTrees = 0;

        foreach (var hexagon in Map.instance.Hexagons)
        {
            //Debug.Log(hexagon.HexTree);
            if (hexagon.HexState != HexagonState.Empty)
            {
                ++totalTrees;
                if (hexagon.IsTreeKilledByFungi)
                {
                    ++treesKilledByFungi;
                }

                if(hexagon.HasFungi)
                {
                    ++totalFungi;
                }

                if(hexagon.IsTreeAlive)
                {
                    ++aliveTrees;
                }
            }
        }
       
        float unitSize = 200f / totalTrees; // divides by 200
        float treesSize = treesKilledByFungi * unitSize;
        Indicator.transform.localPosition = new Vector2(100 - treesSize, 0);

        //calculate scores 
        float deadFungiTreesPercentage = (treesKilledByFungi) / totalTrees;
        float addedScore = 0.5f - deadFungiTreesPercentage;
        addedScore = Mathf.Abs(addedScore);
        addedScore = MaxScore - addedScore;
        addedScore *= Time.deltaTime;
        Score += addedScore;
      
        if(aliveTrees <= MaxLivingTrees || totalFungi <= 0) {
            HighScores.SaveHighScore((int)Score);
            Application.LoadLevel(1);
        }
    }
}
