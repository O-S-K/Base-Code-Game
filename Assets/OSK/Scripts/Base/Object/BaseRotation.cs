using UnityEngine;
using DG.Tweening;

namespace OSK
{
    public class BaseRotation : MonoBehaviour
    {
        public enum SpinAxis
        {
            X, Y, Z, XY, XZ, YZ, XYZ
        };
        public Transform pivotRotate;
        public bool isRotate = false;
        public SpinAxis spinningAxis;
        public float timePerRound;
        public bool clockwise = true;
        Tween tween;

        protected virtual void Start()
        {
            if (!isRotate) return;
            if (pivotRotate == null)
                pivotRotate = transform;

            switch (spinningAxis)
            {
                case SpinAxis.X:    RotateBody(360F, 0F, 0F);       break;
                case SpinAxis.Y:    RotateBody(0F, 360F, 0F);       break;
                case SpinAxis.Z:    RotateBody(0F, 0F, 360F);       break;
                case SpinAxis.XY:   RotateBody(360F, 360F, 0F);     break;
                case SpinAxis.XZ:   RotateBody(360F, 0F, 360F);     break;
                case SpinAxis.YZ:   RotateBody(0F, 360F, 360F);     break;
                case SpinAxis.XYZ:  RotateBody(360F, 360F, 360F);   break;
            }
        }
        public void RotateBody(float axisX, float axisY, float axisZ)
        {
            Vector3 currentRotation = pivotRotate.rotation.eulerAngles;
            var angleDown     = new Vector3(currentRotation.x + axisX, currentRotation.y + axisY, currentRotation.z + axisZ);
            var angleOpposite = new Vector3(currentRotation.x - axisX, currentRotation.y - axisY, currentRotation.z - axisZ);
            tween = pivotRotate.DORotate(clockwise ? angleDown : angleOpposite, timePerRound, RotateMode.FastBeyond360)
                       .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        public void ResumeRotate() => tween.Play();
        public void StopRotate() => tween.Pause();
    }
}
