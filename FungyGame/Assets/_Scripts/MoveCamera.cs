using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    private bool rotateLeft = false;
    private bool rotateRight = false;
    private Vector2 worldStartPoint;

    // convert screen point to world point
    private Vector2 getWorldPoint(Vector2 screenPoint)
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
        return hit.point;
    }
    
    public void StartRotateLeft()
    {
        rotateLeft = true;
    }

    public void StopRotateLeft()
    {
        rotateLeft = false;
    }

    public void StartRotateRight()
    {
        rotateRight = true;
    }

    public void StopRotateRight()
    {
        rotateRight = false;
    }

    public void RotateLeft()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, 60 * Time.deltaTime);
    }

    public void RotateRight()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, -60 * Time.deltaTime);
    }

    public float cameraSpeed = 1f;
    public float smooth = 2.0F;
    // Update is called once per frame
    void Update()
    {

        float verticalMovement = Input.GetAxis("Vertical") * cameraSpeed;
        float horizontalMovement = Input.GetAxis("Horizontal") * cameraSpeed;
        verticalMovement *= Time.deltaTime;
        horizontalMovement *= Time.deltaTime;
        float tempY = transform.position.y;
        transform.Translate(horizontalMovement, verticalMovement, 0);
        transform.position = new Vector3(transform.position.x, tempY, transform.position.z);



        //TODO: Change for mobile. Maybe add 2 buttons?
        if (Input.GetKey(KeyCode.Q) || rotateLeft)
        {
            RotateLeft();
        }
        if (Input.GetKey(KeyCode.E) || rotateRight)
        {
            RotateRight();
        }
    }
}
