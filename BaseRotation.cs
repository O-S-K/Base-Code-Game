using UnityEngine;
using DG.Tweening;

namespace OSK
{
    public class BaseRotation : MonoBehaviour
    {
        public enum SpinAxis
        {
            x,y, z,
            xy, xz, yz,
            xyz
        };
        public Transform pivotRotate;
        public bool isRotate = false;
        public SpinAxis spinningAxis;
        public float timePerRound;
        public bool clockwise = true;
        
        protected virtual void Start()
        {
            if (!isRotate) return;
            switch (spinningAxis)
            {
                case SpinAxis.x:    RotateBody(360F, 0F, 0F);       break;
                case SpinAxis.y:    RotateBody(0F, 360F, 0F);       break;
                case SpinAxis.z:    RotateBody(0F, 0F, 360F);       break;
                case SpinAxis.xy:   RotateBody(360F, 360F, 0F);     break;
                case SpinAxis.xz:   RotateBody(360F, 0F, 360F);     break;
                case SpinAxis.yz:   RotateBody(0F, 360F, 360F);     break;
                case SpinAxis.xyz:  RotateBody(360F, 360F, 360F);   break;
            }
        }
        public void RotateBody(float axisX, float axisY, float axisZ)
        {
            Vector3 currentRotation = pivotRotate.rotation.eulerAngles;
            var angleDown     = new Vector3(currentRotation.x + axisX, currentRotation.y + axisY, currentRotation.z + axisZ);
            var angleOpposite = new Vector3(currentRotation.x - axisX, currentRotation.y - axisY, currentRotation.z - axisZ);
            pivotRotate.DORotate(clockwise ? angleDown : angleOpposite, timePerRound, RotateMode.FastBeyond360)
                       .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
    }
}
