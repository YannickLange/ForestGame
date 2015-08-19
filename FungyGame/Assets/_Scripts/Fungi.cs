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
    public SpriteRenderer spriteRenderer;
    public Sprite[] stageSprites;
    public Hexagon startHexagon, endHexagon;

    public Hexagon occupiedHexagon { get; set; }

    void Awake()
    {
        // load all frames in fruitsSprites array
        spriteRenderer = GetComponent<SpriteRenderer>();
        stageSprites = Resources.LoadAll<Sprite>(tileSheetName);
        maxStage = maxStage - 1;
    }

    void Start()
    {
        //Hacky code here, causes bugs with the infectionbar!
        occupiedHexagon = transform.parent.GetComponent<Hexagon>();
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
