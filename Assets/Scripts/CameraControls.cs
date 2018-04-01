using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.5f; // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f; // The rate of change of the orthographic size in orthographic mode.
    public float panSpeed = 0.1F;
    public float orthographicSizeMin = 30F;
    public float orthographicSizeMax = 275F;
    public float fovMin = 30F;
    public float fovMax = 60f;
    private Camera myCamera;

    void Start()
    {
        Debug.Log("This is: " + this.ToString());
        myCamera = Camera.main;
        Debug.Log("Cam is: " + myCamera.ToString());
    }
    void Update()
    {
        if (myCamera.enabled)
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

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                // Get movement of the finger since last frame
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                float adjustedPanSpeed = panSpeed * (fovMax / myCamera.fieldOfView);

                Debug.Log(touchDeltaPosition.ToString() + " | adjustedPanSpeed: " + adjustedPanSpeed.ToString());

                // Move object across XY plane
                transform.Translate(-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0);
            }
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                // If the camera is orthographic...
                if (myCamera.orthographic)
                {
                    // ... change the orthographic size based on the change in distance between the touches.
                    myCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                    // Make sure the orthographic size never drops below zero.
                    myCamera.orthographicSize = Mathf.Max(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
                }
                else
                {
                    // Otherwise change the field of view based on the change in distance between the touches.
                    myCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                    // Clamp the field of view to make sure it's between 0 and 180.
                    myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
                }
            }
        }

    }

    void zoomOut()
    {
        if (myCamera.orthographic)
        {
            Debug.Log("zoom out Orthographic");
            myCamera.orthographicSize += orthoZoomSpeed;
            myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
        }
        else
        {
            Debug.Log("zoom out Perspective");
            myCamera.fieldOfView += perspectiveZoomSpeed;
            myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
        }

    }
    void zoomIn()
    {
        if (myCamera.orthographic)
        {
            Debug.Log("zoom in Orthographic");
            myCamera.orthographicSize += orthoZoomSpeed;
            myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
        }
        else
        {
            Debug.Log("zoom in Perspective");
            myCamera.fieldOfView -= perspectiveZoomSpeed;
            myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
        }
    }
}