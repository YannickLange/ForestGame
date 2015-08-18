using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGO : MonoBehaviour
{
    public static bool isNGOWaiting = false;
    public Sprite PickupWithFungiSprite;
    public Sprite PickupSprite;
    public Sprite NormalSprite;
    public float MoveTime = 0.6f;
    public float ProtectActionTime = 5.0f;
    public float ProtectionTime = 10.0f;

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
        isNGOWaiting = true;
        List<Hexagon> ngoHex = new List<Hexagon>();
        //1:Looking for a spot:
        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
            if (Map.instance.Hexagons[i].Fungi == null && Map.instance.Hexagons[i].HexTree != null)
                ngoHex.Add(Map.instance.Hexagons[i]);
        }
        if (ngoHex.Count == 0)
            return;

        _targetHex = ngoHex[Random.Range(0, ngoHex.Count)];
        _targetHex.isTarget = true;
        _targetHex.ngo = this;
        _targetTr = _targetHex.transform;
        //Highlight the hexgon
        //StartCoroutine(_targetHex.FlashHexagon(ResourcesManager.instance.HexNGOTargetMat));

        //2:Looking for the spawn hexagon
        _spawnHex = Map.instance.HexBorders[Random.Range(0, Map.instance.HexBorders.Length)];

        //4:Enable the movement
        transform.position = _spawnHex.transform.position;

        StartCoroutine(MoveNGO());
    }

    public IEnumerator StartProtecting()
    {
        StartCoroutine(_targetHex.FlashHexagon(ResourcesManager.instance.HexNGOTargetMat));

        #region 1:ProtectHexagon
        GetComponent<SpriteRenderer>().sprite = PickupSprite;
        yield return new WaitForSeconds(ProtectActionTime / 2);
        GetComponent<SpriteRenderer>().sprite = PickupWithFungiSprite;
        yield return new WaitForSeconds(ProtectActionTime / 2);
        GetComponent<SpriteRenderer>().sprite = NormalSprite;
        #endregion

        #region 2:Moving out the map
        float dist = 500f;
        Hexagon _exitHex = null;
        float sqrRemainingDistance;
        Vector3 newPosition;
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

        #region 3:HideNGO
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(ProtectionTime);
        #endregion


        #region 4:Destroy the NGO
        _targetHex.isTarget = false;
        _targetHex.ngo = null;
        _targetHex.HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
        Destroy(gameObject);
        #endregion

    }

    private IEnumerator MoveNGO()
    {
        #region 1:Moving
        float sqrRemainingDistance = (_thisTransform.position - _targetTr.position).sqrMagnitude;
        Vector3 newPosition;
        while (sqrRemainingDistance > 1e-6)
        {
            newPosition = Vector3.MoveTowards(_rb.position, _targetTr.position, MoveTime * Time.deltaTime);
            _rb.MovePosition(newPosition);


            sqrRemainingDistance = (_thisTransform.position - _targetTr.position).sqrMagnitude;
            yield return null;
        }
        #endregion

        /*
        #region 2:ProtectHexagon
        GetComponent<SpriteRenderer>().sprite = PickupSprite;
        yield return new WaitForSeconds(ProtectActionTime / 2);
        GetComponent<SpriteRenderer>().sprite = PickupWithFungiSprite;
        yield return new WaitForSeconds(ProtectActionTime / 2);
        GetComponent<SpriteRenderer>().sprite = NormalSprite;
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

        #region HideNGO
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(ProtectionTime);
        #endregion


        #region 5:Destroy the NGO
        _targetHex.isTarget = false;
        _targetHex.HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
        Destroy(gameObject);
        #endregion
         */
    }
}
