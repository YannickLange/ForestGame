using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    public float fixedRotationAngle = 90f;
    public AngleDirection fixedRotationDirection;
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

    void Update()
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
}