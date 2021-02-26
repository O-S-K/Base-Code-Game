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

        public Transform target;
        public FollowAxis followAxis;

        [Header("Smooth")]
        [Range(0F, 1F)]
        public float smoothFollowX;
        [Range(0F, 1F)]
        public float smoothFollowY;
        [Range(0F, 1F)]
        public float smoothFollowZ;
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
                switch (followAxis)
                {
                    case FollowAxis.X: FollowInAxis(target.position.x + offsetX, transform.position.y + offsetY, transform.position.z + offsetZ); break;
                    case FollowAxis.Y: FollowInAxis(transform.position.x + offsetX, target.position.y + offsetY, transform.position.z + offsetZ); break;
                    case FollowAxis.Z: FollowInAxis(transform.position.x + offsetX, transform.position.y + offsetY, target.position.z + offsetZ); break;
                    case FollowAxis.XY: FollowInAxis(target.position.x + offsetX, target.position.y + offsetY, transform.position.z + offsetZ); break;
                    case FollowAxis.XZ: FollowInAxis(target.position.x + offsetX, transform.position.y + offsetY, target.position.z + offsetZ); break;
                    case FollowAxis.YZ: FollowInAxis(transform.position.x + offsetX, target.position.y + offsetY, target.position.z + offsetZ); break;
                    case FollowAxis.XYZ: FollowInAxis(target.position.x + offsetX, target.position.y + offsetY, target.position.z + offsetZ); break;
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