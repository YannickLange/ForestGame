using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    public float fixedRotationAngle = 90f;
    public AngleDirection fixedRotationDirection;
    public bool Important = false;
    public enum AngleDirection
    {
        none,
        x,
        y,
        z
    }
    private Camera m_Camera;

    void Awake()
    {
        m_Camera = Camera.main;
    }

    public void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
        switch (fixedRotationDirection)
        {
            case AngleDirection.x: transform.localEulerAngles = new Vector3(fixedRotationAngle, transform.localEulerAngles.y, transform.localEulerAngles.z); break;
            case AngleDirection.y: transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, fixedRotationAngle, transform.localEulerAngles.z); break;
            case AngleDirection.z: transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, fixedRotationAngle); break;
            default: break;
        }
    }

    void LateUpdate()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer parentSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("Can not set sorting layer without the spriteRenderer component");
            return;
        }

        spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint (transform.position).y * -1;
        
        if (parentSpriteRenderer == null)
        {
            //Debug.Log("Can not set sorting layer without the spriteRenderer component: " + spriteRenderer);
            //TODO. find out why this happens
            return;
        }
        if (Important)
            spriteRenderer.sortingOrder = parentSpriteRenderer.sortingOrder + 1;

    }
}