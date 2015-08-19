using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fungi : MonoBehaviour
{
    public string tileSheetName = "fungusTest";
    private float timer = 0f;
    public float maxTimer = 10f;
    private int lastStage = -1;
    public int stage = 0;
    public Sprite[] stageSprites;

    //Cached components
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        // load all frames in fruitsSprites array
        spriteRenderer = GetComponent<SpriteRenderer>();
        stageSprites = Resources.LoadAll<Sprite>(tileSheetName);
    }

    public bool IsAtMaxStage{ get { return stage == stageSprites.Length - 1; } }

    public void Update()
    {
        if (lastStage != stage)
        {
            lastStage = stage;
            //Update the stage sprite
            spriteRenderer.sprite = stageSprites[stage];
            GridManager.instance.UserInteraction.updateView();
        }
    }

    public void advanceGrowth(float speed)
    {
        if (!IsAtMaxStage)
        {
            timer += 1f * Time.deltaTime * speed;
            
            if (timer >= maxTimer)
            {
                stage++;
                timer = 0f;
            }
        }
    }

}
