using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSK
{
    public class BaseFollow : MonoBehaviour
    {
        public enum FollowAxis
        {
            X, Y, Z, XY, XZ, YZ, XYZ
        }

        public FollowAxis followAxis;
        public Transform target;
        [Range(0, .1F)]
        public float smoothFollow;
        protected Vector3 offset;
        protected Vector3 targetPosition;

        protected virtual void Start()
        {
            offset = transform.position - target.position;
        }

        protected virtual void FollowToTarget(float bonusX, float bonusY, float bonusZ)
        {
            if (target != null)
            {
                switch (followAxis)
                {
                    case FollowAxis.X:   FollowInAxis(target.position.x    + bonusX,   transform.position.y + bonusY,   transform.position.z + bonusZ);  break;
                    case FollowAxis.Y:   FollowInAxis(transform.position.x + bonusX,   target.position.y    + bonusY,   transform.position.z + bonusZ);  break;
                    case FollowAxis.Z:   FollowInAxis(transform.position.x + bonusX,   transform.position.y + bonusY,   target.position.z    + bonusZ);  break;
                    case FollowAxis.XY:  FollowInAxis(target.position.x    + bonusX,   target.position.y    + bonusY,   transform.position.z + bonusZ);  break;
                    case FollowAxis.XZ:  FollowInAxis(target.position.x    + bonusX,   transform.position.y + bonusY,   target.position.z    + bonusZ);  break;
                    case FollowAxis.YZ:  FollowInAxis(transform.position.x + bonusX,   target.position.y    + bonusY,   target.position.z    + bonusZ);  break;
                    case FollowAxis.XYZ: FollowInAxis(target.position.x    + bonusX,   target.position.y    + bonusY,   target.position.z    + bonusZ);  break;
                }
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFollow);
            }
        }

        protected void FollowInAxis(float _x, float _y, float _z)
        {
            targetPosition = new Vector3(_x, _y, _z);
        }
    }
}