using UnityEngine;

namespace OSK
{
    public class BaseFollow : MonoBehaviour
    {
        public enum FollowAxis
        {
            X, Y, Z, XY, XZ, YZ, XYZ
        }

        public Transform target;
        public FollowAxis followAxis;

        [Header("Smooth")]
        [Range(0F, 1F)] public float smoothFollowX;
        [Range(0F, 1F)] public float smoothFollowY;
        [Range(0F, 1F)] public float smoothFollowZ;

        protected Vector3 offset;
        protected Vector3 targetPosition;


        protected virtual void Start()
        {
            offset = transform.position - target.position;
        }


        protected virtual void FollowToTarget(float offsetX, float offsetY, float offsetZ)
        {
            if (target != null)
            {
                var transX = transform.position.x + offsetX;
                var transY = transform.position.y + offsetY;
                var transZ = transform.position.z + offsetZ;

                var targetX = target.position.x + offsetX;
                var targetY = target.position.y + offsetY;
                var targetZ = target.position.z + offsetZ;


                switch (followAxis)
                {
                    case FollowAxis.X: FollowInAxis(targetX, transY, transZ); break;
                    case FollowAxis.Y: FollowInAxis(transX, targetY, transZ); break;
                    case FollowAxis.Z: FollowInAxis(transX, transY, targetZ); break;
                    case FollowAxis.XY: FollowInAxis(targetX, targetY, transZ); break;
                    case FollowAxis.XZ: FollowInAxis(targetX, transY, targetZ); break;
                    case FollowAxis.YZ: FollowInAxis(transX, targetY, targetZ); break;
                    case FollowAxis.XYZ: FollowInAxis(targetX, targetY, targetZ); break;
                }

                transform.position = SmoothFollow(targetPosition.x, targetPosition.y, transform.position.z, smoothFollowX);
                transform.position = SmoothFollow(transform.position.x, targetPosition.y, transform.position.z, smoothFollowY);
                transform.position = SmoothFollow(transform.position.x, targetPosition.y, targetPosition.z, smoothFollowZ);
            }
        }

        protected Vector3 SmoothFollow(float _x, float _y, float _z, float _smooth)
        {
            return Vector3.Lerp(transform.position, new Vector3(_x, _y, _z), _smooth);
        }

        protected Vector3 FollowInAxis(float _x, float _y, float _z)
        {
            return targetPosition = new Vector3(_x, _y, _z);
        }
    }
}