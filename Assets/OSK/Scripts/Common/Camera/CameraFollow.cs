using UnityEngine;
using OSK;
public class CameraFollow : BaseFollow
{
    public Transform playerTarget;
    public float offsetCameraLookAtPlayer;

    [Header("X-axis clamp")]
    public float clampXRange;

    protected override void Start()
    {
        base.Start();
    }

    protected void LateUpdate()
    {
        float xOffset = 0F;
        float zOffset = 0F;

        var distance = playerTarget.forward * offsetCameraLookAtPlayer;
        xOffset += distance.x;
        FollowToTarget(xOffset, 0, zOffset);

        if (transform.position.x > clampXRange)  CaculatePostion(clampXRange);
        if (transform.position.x < -clampXRange) CaculatePostion(-clampXRange);
    }

    Vector3 CaculatePostion(float x)
    {
        return transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
