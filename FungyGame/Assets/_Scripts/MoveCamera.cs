using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

    public float cameraSpeed = 1f;
	// Update is called once per frame
	void Update () {

        if(Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + cameraSpeed * Time.deltaTime, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - cameraSpeed * Time.deltaTime, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y , Camera.main.transform.position.z - cameraSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + cameraSpeed * Time.deltaTime);
        }
	
	}
}
