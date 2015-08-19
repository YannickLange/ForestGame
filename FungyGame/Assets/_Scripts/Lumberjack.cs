using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lumberjack : MonoBehaviour
{
    public Sprite LumberjackWithLog;
    public static bool isLumberjackWaiting = false;
    public float MoveTime = 0.5f;
    public float ChopActionTime = 5.0f;
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
        isLumberjackWaiting = false;
    }

    public void Spawn()
    {
        isLumberjackWaiting = true;
        SelectTarget();   

        //2:Looking for the spawn hexagon
        _spawnHex = Map.instance.HexBorders[Random.Range(0, Map.instance.HexBorders.Length)];

        //4:Enable the movement
        transform.position = _spawnHex.transform.position;

        StartCoroutine(MoveLumberjack());
    }

    private void SelectTarget()
    {
        if (_targetHex != null && _targetHex.ngo == null)
            _targetHex.isTarget = false;
        /*if(_targetHex.ngo != null)
            _targetHex.StopBlink*/
        List<Hexagon> fullHex = new List<Hexagon>();
        //1:Looking for a spot:
        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
            if (Map.instance.Hexagons[i].HexTree != null &&
                !Map.instance.Hexagons[i].isTarget &&
                Map.instance.Hexagons[i].Type != TreeType.Sapling &&
                Map.instance.Hexagons[i].Type != TreeType.DeadTree)
                fullHex.Add(Map.instance.Hexagons[i]);
        }
        if (fullHex.Count == 0)
        {
            Destroy(gameObject);
            _targetHex.ShowOverTile(false, new Color());
        }

        _targetHex = fullHex[Random.Range(0, fullHex.Count)];
        _targetHex.isTarget = true;
        _targetTr = _targetHex.transform;

        StartCoroutine(_targetHex.FlashHexagon(new Color32(99,1,78,255)));
    }

    private IEnumerator MoveLumberjack()
    {
        #region 1:Moving
        float sqrRemainingDistance = (transform.position - _targetTr.position).sqrMagnitude;
        Vector3 newPosition;
        while (sqrRemainingDistance > 1e-6)
        {
            if (_targetHex.Type == TreeType.DeadTree || _targetHex.ngo != null)
            {
                SelectTarget();
            }
            newPosition = Vector3.MoveTowards(_rb.position, _targetTr.position, MoveTime * Time.deltaTime);
            _rb.MovePosition(newPosition);


            sqrRemainingDistance = (_thisTransform.position - _targetTr.position).sqrMagnitude;

            yield return null;
        }
        #endregion

        #region 2:ChopTree
        //Change the sprite of the tree:
        yield return new WaitForSeconds(ChopActionTime / 2f);
        _targetHex.ChopTree();
        yield return new WaitForSeconds(ChopActionTime / 2f);
        _targetHex.TakeCuttedTreeAway();
        GetComponent<SpriteRenderer>().sprite = LumberjackWithLog;
        #endregion

        #region 3:Moving out the map
        float dist = 500f;
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
            while (sqrRemainingDistance > 1e-6)
            {
                newPosition = Vector3.MoveTowards(_rb.position, _exitHex.transform.position, MoveTime * Time.deltaTime);
                _rb.MovePosition(newPosition);


                sqrRemainingDistance = (_thisTransform.position - _exitHex.transform.position).sqrMagnitude;

                yield return null;
            }
        }
        #endregion

        #region 4:Destroy the lumberjack
        _targetHex.isTarget = false;
        _targetHex.HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
        isLumberjackWaiting = false;
        Destroy(gameObject);
        _targetHex.ShowOverTile(false, new Color());
        #endregion
    }
}
