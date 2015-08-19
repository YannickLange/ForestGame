using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fungi : MonoBehaviour
{
    public string tileSheetName = "fungusTest";
    public float timer = 0f;
    public float timerSpeedMultiplier = 1f;
    public float maxTimer = 10f;
    private int lastStage = -1;
    public int stage = 0;
    public int maxStage = 6;
    public Sprite[] stageSprites;

    //Cached components
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        // load all frames in fruitsSprites array
        spriteRenderer = GetComponent<SpriteRenderer>();
        stageSprites = Resources.LoadAll<Sprite>(tileSheetName);
        maxStage = maxStage - 1;
    }

    void Start()
    {
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
}
