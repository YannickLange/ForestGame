using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{

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
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, 30 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, -30 * Time.deltaTime);
        }


    }
}
