﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planter : MonoBehaviour
{
    public float MoveTime = 0.5f;
    public float PlantActionTime = 5.0f;


    private Hexagon _targetHex = null;
    private Transform _targetTr = null;
    private Hexagon _spawnHex = null;


    //Cached components:
    private Transform _thisTransform;
    private Rigidbody _rb;


    void Awake()
    {
        _thisTransform = transform;
        _rb = GetComponent<Rigidbody>();
    }
    public void Spawn()
    {
        List<Hexagon> emptyHex = new List<Hexagon>();
        //1:Looking for a spot:
        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
            if (Map.instance.Hexagons[i].HexTree == null && !Map.instance.Hexagons[i].isTarget)
                emptyHex.Add(Map.instance.Hexagons[i]);
        }

        _targetHex = emptyHex[Random.Range(0, emptyHex.Count)];
        _targetHex.isTarget = true;
        _targetTr = _targetHex.transform;
        //Highlight the hexgon
        StartCoroutine(_targetHex.FlashHexagon(ResourcesManager.instance.HexPlanterTargetMat));

        //2:Looking for the spawn hexagon
        _spawnHex = Map.instance.HexBorders[Random.Range(0, Map.instance.HexBorders.Length)];

        //4:Enable the movement
        transform.position = _spawnHex.transform.position;

        StartCoroutine(MovePlanter());
    }

    private IEnumerator MovePlanter()
    {
        #region 1:Moving
        float sqrRemainingDistance = (transform.position - _targetTr.position).sqrMagnitude;
        Vector3 newPostion;
        while (sqrRemainingDistance > float.Epsilon)
        {
            newPostion = Vector3.MoveTowards(_rb.position, _targetTr.position, MoveTime * Time.deltaTime);
            _rb.MovePosition(newPostion);


            sqrRemainingDistance = (_thisTransform.position - _targetTr.position).sqrMagnitude;

            yield return null;
        }
        #endregion

        #region 2:PlantSeed
        yield return new WaitForSeconds(PlantActionTime);
        PlantSeed();

        yield return new WaitForSeconds(PlantActionTime);
        #endregion

        #region 3:Moving out the map
        float dist = 9999f;
        Hexagon _exitHex = null;
        for (int i = 0; i < Map.instance.HexBorders.Length; i++)
        {
            if (Vector3.Distance(_thisTransform.position, Map.instance.HexBorders[i].transform.position) < dist)
            {
                _exitHex = Map.instance.HexBorders[i];
                dist = Vector3.Distance(_thisTransform.position, Map.instance.HexBorders[i].transform.position);
            }
        }
        if (_exitHex != null)
        {
            sqrRemainingDistance = (transform.position - _exitHex.transform.position).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                newPostion = Vector3.MoveTowards(_rb.position, _exitHex.transform.position, MoveTime * Time.deltaTime);
                _rb.MovePosition(newPostion);


                sqrRemainingDistance = (_thisTransform.position - _exitHex.transform.position).sqrMagnitude;

                yield return null;
            }
        }
        #endregion

        #region 4:Destroy the planter
        _targetHex.isTarget = false;
        _targetHex.HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
        Destroy(gameObject);
        #endregion
    }

    private void PlantSeed()
    {
        //1:Instantiate a new Tree at the _current position
        TreeGenerator.SpawnSapling(_targetHex);
    }
}
