using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    public float zoomSpeed;
    public float panSpeed;
    public float orthographicSizeMin;
    public float orthographicSizeMax;
    public float fovMin;
    public float fovMax;
    private Camera myCamera;

    void Start()
    {
        Debug.Log("This is: " + this.ToString());
        myCamera = Camera.main;
        Debug.Log("Cam is: " + myCamera.ToString());
    }
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Debug.Log("Mouse Scroll Wheel Back: " + Input.GetAxis("Mouse ScrollWheel").ToString());
            zoomOut();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Debug.Log("Mouse Scroll Wheel Forward: " + Input.GetAxis("Mouse ScrollWheel").ToString());
            zoomIn();
        }

        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed left click.");

        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed right click.");

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            Debug.Log(touchDeltaPosition.ToString());

            // Move object across XY plane
            transform.Translate(-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0);
        }
    }

    void zoomOut()
    {
        if (myCamera.orthographic)
        {
            Debug.Log("zoom out Orthographic");
            myCamera.orthographicSize += zoomSpeed;
            myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
        }
        else
        {
            Debug.Log("zoom out Perspective");
            myCamera.fieldOfView += zoomSpeed;
            myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
        }

    }
    void zoomIn()
    {
        if (myCamera.orthographic)
        {
            Debug.Log("zoom in Orthographic");
            myCamera.orthographicSize += zoomSpeed;
            myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
        }
        else
        {
            Debug.Log("zoom in Perspective");
            myCamera.fieldOfView -= zoomSpeed;
            myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
        }
    }
}