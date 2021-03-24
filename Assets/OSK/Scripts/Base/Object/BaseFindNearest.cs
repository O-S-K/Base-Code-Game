using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSK
{
    public abstract class  BaseFindNearest : MonoBehaviour
    {
        public Transform Pivot;

        [Header("Camera In Sight")]
        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();
        public LayerMask targetMask;
        public LayerMask blockMask;
        public Collider[] targetsInViewRadius;
        protected Transform nearestObject;

        protected virtual void FindVisibleTargets(Transform _transOrigin,Transform _rot, float _fovDistanceCheck, float _positionYStartCheckRaycast, float _timeSmooth)
        {
            visibleTargets.Clear();
            float minimumDistance = _fovDistanceCheck;
            targetsInViewRadius = Physics.OverlapSphere(transform.position, minimumDistance, targetMask);

            foreach (var collider in targetsInViewRadius)
            {
                float distance = Vector3.Distance(_transOrigin.position, collider.transform.position);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    nearestObject = collider.transform;
                }

                var dirToTarget = nearestObject.position - _transOrigin.position;
                var posOrigin = new Vector3(_transOrigin.position.x, _transOrigin.position.y + _positionYStartCheckRaycast, _transOrigin.position.z);

                if (!Physics.Raycast(posOrigin, dirToTarget, distance, blockMask))
                {
                    visibleTargets.Add(nearestObject);
                }
            }

            if (nearestObject != null && visibleTargets.Count > 0)
            {
                CallActionWhenObjectInVisible();
                Vector3 target = nearestObject.position - _transOrigin.position;
                Vector3 newDir = Vector3.RotateTowards(_rot.forward, target, _timeSmooth * Time.deltaTime, 0.0F);
                _rot.rotation = Quaternion.LookRotation(newDir);
                Debug.Log("Nearest Object: " + nearestObject + ": Distance: " + minimumDistance);
            }
            else
            {
                Debug.Log("There is no Object in the given radius");
                CallActionWhenObjectOutVisible();
                _transOrigin.localRotation = Quaternion.Euler(0F, 0F, 0F);
            }
        }
        protected abstract void CallActionWhenObjectInVisible();
        protected abstract void CallActionWhenObjectOutVisible();
    }
}
