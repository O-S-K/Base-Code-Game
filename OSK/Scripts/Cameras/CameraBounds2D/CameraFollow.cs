using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    float speed = 10;
    [SerializeField] CameraBounds2D bounds;
    Vector2 maxXPositions, maxYPositions;

    void Awake()
    {
        bounds.Initialize(GetComponent<Camera>());
        maxXPositions = bounds.maxXlimit;
        maxYPositions = bounds.maxYlimit;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;

        var clampX = Mathf.Clamp(player.position.x, maxXPositions.x, maxXPositions.y);
        var clampY = Mathf.Clamp(player.position.y, maxYPositions.x, maxYPositions.y);

        Vector3 targetPosition = new Vector3(clampX, clampY, currentPosition.z);
        transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * speed);
    }
}
