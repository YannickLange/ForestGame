﻿using UnityEngine;
using System.Collections;

public class ResourcesManager : MonoBehaviour
{
    public Material HexValidSurroundingMaterial;
    public Material HexInvalidSurroundingMaterial;
    public Material HexNormalMaterial;
    public Material HexSelectedMaterial;
    public Material HexPlanterTargetMat;
    public Material HexLumberjackTargetMat;
    public Material HexNGOTargetMat;
    public Material HexWhiteBorders;

    public Material[] TreeMat;

    public GameObject[] TreeTypes;

    public GameObject Planter;
    public GameObject Lumberjack;
    public GameObject NGO;

    public static ResourcesManager instance;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }
}
