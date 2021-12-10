using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes the camera follow the player

public class CameraController_old : MonoBehaviour {

	public Transform target;	// Target to follow (player)

	public Vector3 offset;			// Offset from the player
	public float zoomSpeed = 10f;	// How quickly we zoom
	public float minZoom = 5f;		// Min zoom amount
	public float maxZoom = 10f;		// Max zoom amount

	public float pitch = 2f;        // Pitch up the camera to look at head

	//public float yawSpeed = 100f;	// How quickly we rotate

	// In these variables we store input from Update
	private float currentZoom;// = 10f;
    //private float currentYaw = 0f;

    private void Start()
    {
		currentZoom = maxZoom;
    }

    void Update ()
	{
		// Adjust our zoom based on the scrollwheel
		currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

		// Adjust our camera's rotation around the player
		//currentYaw -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
	}

	void LateUpdate ()
	{
		// Set our cameras position based on offset and zoom
		transform.position = target.position - offset * currentZoom;
		// Look at the player's head
		transform.LookAt(target.position + Vector3.up * pitch);

		// Rotate around the player
		//transform.RotateAround(target.position, Vector3.up, currentYaw);
	}

}
