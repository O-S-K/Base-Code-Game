using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRotateAroundObject : MonoBehaviour
{
    public float yLimitOffset;
    public float yMaxLimit = 60f;
    public float yMinLimit = -60f;

    public float distance;
    public float maxDistance = 15f;
    public float minDistance = 5f;


    public  float xSpeedTouch = 0.05f;
    public  float ySpeedTouch = 0.05f;

    public bool reset;

    private Vector3 position;
    private float xVelocity;
    private float yVelocity;
    private float targetDistance = 10f;
    private float zoomVelocity = 2f;
    private float smoothingZoom = 0.5f;
    private float y;
    private float dampeningX = 0.9f;
    private float dampeningY = 0.9f;


    internal void Init()
    {
        targetDistance = distance;
        y = 0f;
        xVelocity = 0f;
        yVelocity = 0f;
        zoomVelocity = 0f;
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        HandleInput();
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        transform.Rotate(new Vector3(0f, xVelocity, 0f), Space.World);
        if (y + yVelocity < yMinLimit + yLimitOffset)
        {
            yVelocity = yMinLimit + yLimitOffset - y;
        }
        else if (y + yVelocity > yMaxLimit + yLimitOffset)
        {
            yVelocity = yMaxLimit + yLimitOffset - y;
        }
        y += yVelocity;
        transform.Rotate(new Vector3(yVelocity, 0f, 0f), Space.Self);
        if (targetDistance + zoomVelocity < minDistance)
        {
            zoomVelocity = minDistance - targetDistance;
        }
        else if (targetDistance + zoomVelocity > maxDistance)
        {
            zoomVelocity = maxDistance - targetDistance;
        }
        targetDistance += zoomVelocity;
        distance = Mathf.Lerp(distance, targetDistance, smoothingZoom);
        position = transform.rotation * new Vector3(0f, 0f, -distance) + TheManager.rcc_controller.transform.position;
        transform.position = position;
        xVelocity *= dampeningX;
        yVelocity *= dampeningY;
        zoomVelocity = 0f;
    }

    private void HandleInput()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //foreach (Touch touch in Input.touches)
            //{
            //    TouchPhase phase = touch.phase;
            //    if (phase != TouchPhase.Began)
            //    {
            //        if (phase != TouchPhase.Moved)
            //        {
            //            if (phase != TouchPhase.Ended)
            //            {
            //            }
            //        }
            //        else
            //        {
            //            xVelocity += Input.touches[0].deltaPosition.x * 0.05f;
            //            yVelocity -= Input.touches[0].deltaPosition.y * 0.05f; 
            //        }
            //    }
            //}
            if (Input.GetMouseButton(0))
            {
                xVelocity += Input.GetAxis("Mouse X") * xSpeedTouch;
                yVelocity -= Input.GetAxis("Mouse Y") * ySpeedTouch;
            }
        }
    }
}
