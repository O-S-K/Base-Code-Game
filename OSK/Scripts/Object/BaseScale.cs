using UnityEngine;
using DG.Tweening;

namespace OSK
{
    public class BaseScale : MonoBehaviour
    {
        public enum TypeScale
        {
            Single,
            Loop
        };
        public Transform pivotScale;
        public TypeScale typeScale;
        public Vector3 valueScale;
        public Ease ease = Ease.Linear;
        public float timeScale;

        protected virtual void Start()
        {
            if (pivotScale == null) 
                pivotScale = this.transform;

            switch(typeScale)
            {
                case TypeScale.Single:
                    ScaleBodySingle(valueScale, timeScale, ease);
                    break;
                case TypeScale.Loop:
                    ScaleBodyLoop(valueScale, timeScale, ease);
                    break;
            }
        }

        protected void ScaleBodySingle(Vector3 _value, float _timeScale, Ease _ease)
        {
            pivotScale.DOScale(_value, _timeScale).SetEase(_ease);
        }
        protected void ScaleBodyLoop(Vector3 _value, float _timeScale, Ease _ease)
        {
            pivotScale.DOScale(_value, _timeScale).SetEase(_ease).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
