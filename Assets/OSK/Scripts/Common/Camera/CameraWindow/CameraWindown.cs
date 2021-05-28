using UnityEngine;

public class CameraWindown : MonoBehaviour
{
    public enum FollowAxis
    {
        X,
        Y,
        XY
    }
    public FollowAxis followAxis = FollowAxis.X;
    public Transform target; // player

    [Range(0.01F, 10)]
    public float speed; // 5

    [Range(0.01F, 1)]
    public float timeSmoothCamera; // 0.75F

    public float xLeft, xRight = 1; // 1.25F
    public float yDown, yTop = 1; // 1.45F, 2.5F

    float interpVelocity;
    float x, y;

    Vector3 targetPos;
    Vector3 offset;
    Vector3 posFollow;

    private void Start()
    {
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        if (target)
        {
            switch (followAxis)
            {
                case FollowAxis.X:
                    FollowInAxisX();
                    posFollow = new Vector3(x, target.position.y, target.position.z);
                    break;
                case FollowAxis.Y:
                    FollowInAxisY();
                    posFollow = new Vector3(target.position.x, y, target.position.z);
                    break;
                case FollowAxis.XY:
                    FollowInAxisX();
                    FollowInAxisY();
                    posFollow = new Vector3(x, y, target.position.z);
                    break;
            }

            Vector3 posNoZ = transform.position;
            posNoZ.z = posFollow.z;

            Vector3 targetDirection = posFollow - transform.position;
            interpVelocity = targetDirection.magnitude * speed;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
            targetPos.z = transform.position.z;

            //Ex1: var posPlayer = new Vector3(transform.position.x, target.position.y + offset.y, transform.position.z);
            //Ex2: var posPlayer = transform.position;
            transform.position = Vector3.Lerp(transform.position, targetPos, timeSmoothCamera);
        }
    }

    void FollowInAxisX()
    {
        if (target.position.x > x + xRight)
        {
            x = target.position.x - xRight;
        }
        else if (target.position.x < x + xLeft)
        {
            x = target.position.x - xLeft;
        }
    }

    void FollowInAxisY()
    {
        if (target.position.y > y + yTop)
        {
            y = target.position.y - yTop;
        }
        else if (target.position.y < y + yDown)
        {
            y = target.position.y - yDown;
        }
    }


    private void OnDrawGizmos()
    {
        var pointFollow = new Vector3(x, y, target.position.z);
        Gizmos.DrawLine(pointFollow + new Vector3(xRight, yTop, 0), pointFollow + new Vector3(xLeft, yTop, 0));
        Gizmos.DrawLine(pointFollow + new Vector3(xRight, yTop, 0), pointFollow + new Vector3(xRight, yDown, 0));
        Gizmos.DrawLine(pointFollow + new Vector3(xRight, yDown, 0), pointFollow + new Vector3(xLeft, yDown, 0));
        Gizmos.DrawLine(pointFollow + new Vector3(xLeft, yTop, 0), pointFollow + new Vector3(xLeft, yDown, 0));
    }
}
