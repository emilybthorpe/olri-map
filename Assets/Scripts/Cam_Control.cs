using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Control : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float panSpeed = 10f;
    public float rotateSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 200f;
    public float doubleTapTime = 0.3f;

    private Vector3 initialPosition;
    private float initialZoom;
    private float lastTapTime;
    public GameObject mapHolder;
    void Start()
    {
        initialPosition = transform.position;
        initialZoom = Camera.main.orthographicSize;
         if (mapHolder != null)
        {
            // Get the bounds of the mapHolder object
            Vector3 center = mapHolder.transform.position;

            // Calculate the distance from the camera to the mapHolder object
            float distance = CalculateDistance(mapHolder.transform.position);

            // Set the position of the camera to look at the center of the mapHolder object
            transform.position = center - transform.forward * distance;

            // Orient the camera to look at the center of the mapHolder object
            transform.LookAt(center);
            Debug.Log("Kamikaze");
        }
    }

    void Update()
    {
        if (Input.touchCount == 1 && Time.time - lastTapTime > doubleTapTime)
        {
            // Handle panning with one finger touch
            Touch touch = Input.GetTouch(0);
            Vector2 touchDeltaPosition = touch.deltaPosition;
            transform.Translate(-touchDeltaPosition.x * panSpeed * Time.deltaTime, -touchDeltaPosition.y * panSpeed * Time.deltaTime, 0);
        }
        else if (Input.touchCount == 2)
        {
            // Handle pinch-to-zoom with two finger touch
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPosition = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPosition = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMagnitude = (touchZeroPrevPosition - touchOnePrevPosition).magnitude;
            float touchDeltaMagnitude = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMagnitude - touchDeltaMagnitude;
            Camera.main.orthographicSize += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }

        // Handle double tap to reset camera position and zoom
        if (Input.touchCount == 1 && Input.GetTouch(0).tapCount == 2)
        {
            transform.position = initialPosition;
            Camera.main.orthographicSize = initialZoom;
            lastTapTime = Time.time;
        }

        // panning with mouse
        if (Input.GetMouseButton(0) && Input.touchCount == 0)
        {
            float mouseX = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
            transform.Translate(-mouseX, -mouseY, 0);
        }

        // Handle rotation with right mouse button click and drag
        if (Input.GetMouseButton(1) && Input.touchCount == 0)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            transform.Rotate(0, mouseX, 0, Space.World);
        }
    }
    private float CalculateDistance(Vector3 position)
    {
        float distance = (position - transform.position).magnitude;
        return distance;
    }
}
