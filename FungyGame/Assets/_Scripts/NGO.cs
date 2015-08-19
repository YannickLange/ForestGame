using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGO : MonoBehaviour
{
    public static bool isNGOWaiting = false;
    public static bool ProtectionSelection = false;

    public static NGO instance = null;
    public Sprite PickupWithFungiSprite;
    public Sprite PickupSprite;
    public Sprite NormalSprite;
    public float MoveTime = 3f;
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
        isNGOWaiting = false;

        if (instance == null)
            instance = this;
    }

    public void Spawn()
    {
        isNGOWaiting = true;
        List<Hexagon> ngoHex = new List<Hexagon>();
        //1:Looking for a spot:
        for (int i = 0; i < Map.instance.Hexagons.Length; i++)
        {
            if (Map.instance.Hexagons[i].TileInfection == null && Map.instance.Hexagons[i].HexTree != null && !Map.instance.Hexagons[i].isTarget)
                ngoHex.Add(Map.instance.Hexagons[i]);
        }
        if (ngoHex.Count == 0)
            return;

        _targetHex = ngoHex[Random.Range(0, ngoHex.Count)];
        _targetHex.ngo = this;
        _targetHex.isTarget = true;
        _targetTr = _targetHex.transform;

        //2:Looking for the spawn hexagon
        _spawnHex = Map.instance.HexBorders[Random.Range(0, Map.instance.HexBorders.Length)];

        //4:Enable the movement
        transform.position = _spawnHex.transform.position;

        _targetHex.StartCoroutine(MoveNGO());
    }

    public IEnumerator PickupNGO()
    {
        ProtectionSelection = true;

        yield return null;
    }

    public IEnumerator PlaceNGO(Hexagon hex)
    {
        _targetHex.isTarget = false;
        _targetHex.ngo = null;
        hex.isTarget = true;
        hex.ngo = this;
        isNGOWaiting = false;
        ProtectionSelection = false;
        _targetHex.StartCoroutine(hex.FlashHexagon(new Color32(100, 116, 0, 255)));

        #region 1:Moving
        Vector3 newPosition;
        float sqrRemainingDistance;
        if (hex != _targetHex)
        {
            sqrRemainingDistance = (_thisTransform.position - hex.transform.position).sqrMagnitude;
            while (sqrRemainingDistance > 1e-6)
            {
                newPosition = Vector3.MoveTowards(_rb.position, hex.transform.position, MoveTime * Time.deltaTime);
                _rb.MovePosition(newPosition);


                sqrRemainingDistance = (_thisTransform.position - hex.transform.position).sqrMagnitude;
                yield return null;
            }
        }
        #endregion


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

        #region 4:HideNGO
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(ProtectionTime);
        #endregion


        #region 4:Destroy the NGO
        hex.isTarget = false;
        hex.ngo = null;
        hex.HexagonRenderer.material = ResourcesManager.instance.HexNormalMaterial;
        isNGOWaiting = false;
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
    }
}
