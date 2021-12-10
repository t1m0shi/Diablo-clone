using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.04f; //this gets a nice smooth delay, anything after might as well be 1
    [Range(1f, 10f)]
    public float zoomSpeed = 10f;   // How quickly we zoom
    public float minZoom = 0.5f;      // Min zoom amount
    public float maxZoom = 1f;     // Max zoom amount

    //public float pitch = 2f;        // Pitch up the camera to look at head

    // In these variables we store input from Update
    private float currentZoom;// = 10f;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
        currentZoom = maxZoom;
        
    }

    private void Update()
    {
        // Adjust our zoom based on the scrollwheel
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 pos = target.position + offset * currentZoom;
        //transform.position = Vector3.Slerp(transform.position, pos, smoothFactor);
        transform.position = Vector3.Lerp(transform.position, pos, smoothFactor);
        // Set our cameras position based on offset and zoom
        //transform.position = target.position - offset * currentZoom;
        // Look at the player's head
        //transform.LookAt(target.position + Vector3.up * pitch);
    }
}
