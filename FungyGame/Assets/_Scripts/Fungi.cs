﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fungi : MonoBehaviour {

    public GameObject prefab;

    public float timer = 0f;
    public float maxTimer = 10f;

    public int stage = 0;
    public int maxStage = 6;

    private SpriteRenderer spriteRenderer;
    private Sprite[] stageSprites;
    private Hexagon startHexagon, endHexagon;

    void Awake()
    {
        // load all frames in fruitsSprites array
        spriteRenderer = GetComponent<SpriteRenderer>();
        stageSprites = Resources.LoadAll<Sprite>("tileSheet");
    }
	
	// Update is called once per frame
	void Update () {
        if (stage < maxStage)
        {
            timer += 1f * Time.deltaTime;

            if (timer >= maxTimer)
            {
                stage++;
                timer = 0f;

                //Update the stage sprite
                spriteRenderer.sprite = stageSprites[stage];
            }
        }
	}
}
