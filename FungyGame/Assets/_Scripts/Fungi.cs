using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fungi : MonoBehaviour {
    public string tileSheetName = "fungusTest";

    public float timer = 0f;
    public float timerSpeedMultiplier = 1f;
    public float maxTimer = 10f;

    public int stage = 0;
    public int maxStage = 6;

    private SpriteRenderer spriteRenderer;
    private Sprite[] stageSprites;
    private Hexagon startHexagon, endHexagon;
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
	
	// Update is called once per frame
	void Update () {
        //Only works for fungi on the ground Hacky, need to work around
        if (occupiedHexagon != null)
        {
            Debug.Assert(occupiedHexagon.HexTree != null);
            //Should adjust it for multiple types, not extendable code! TODO
            if (occupiedHexagon.Type == TreeType.SmallTree)
            {
                timerSpeedMultiplier = 1 + 0.3f + (Random.value / 4);
            }
            else if (occupiedHexagon.Type == TreeType.BigTree)
            {
                timerSpeedMultiplier = 1 + 0.5f + (Random.value / 2);
            }
        }
        if (stage < maxStage)
        {
            timer += 1f * Time.deltaTime * timerSpeedMultiplier;

            if (timer >= maxTimer)
            {
                stage++;
                timer = 0f;

                UpdateSprite();
            }
        }
	}

    public void UpdateSprite()
    {
        //Update the stage sprite
        spriteRenderer.sprite = stageSprites[stage];
        GridManager.instance.UserInteraction.updateView();
    }
}
