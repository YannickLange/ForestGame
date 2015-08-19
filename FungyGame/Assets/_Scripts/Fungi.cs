using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fungi : MonoBehaviour
{
    //Configuration
    public string tileSheetName = "fungusTest";
    public float maxTimer = 10f;
    public Sprite[] stageSprites;

    //State
    private float timer = 0f;
    private int stage = 0;

    //Cache
    private int lastStage = -1;
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        // load all frames in fruitsSprites array
        spriteRenderer = GetComponent<SpriteRenderer>();
        stageSprites = Resources.LoadAll<Sprite>(tileSheetName);
    }

    public bool IsAtMaxStage{ get { return stage == stageSprites.Length - 1; } }

    public void reset()
    {
        stage = 0;
        timer = 0f;
    }

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
