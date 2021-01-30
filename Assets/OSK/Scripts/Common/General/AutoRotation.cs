using DG.Tweening;
using UnityEditor;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class AutoRotation : MonoBehaviour
{
    public Transform target;
    public bool autoStart;
    public bool clockwise;
    public Ease ease = Ease.Linear;
    public RotState rotState = RotState.Incremental;
    public RotateMode rotmMode = RotateMode.LocalAxisAdd;
    public float delay;
    public float angle;
    public float duration;
    private float startAngle;
    private Tweener tween;

    private IEnumerator Start()
    {
        if (target == null) target = transform;
        if (autoStart)
        {
            yield return new WaitForSeconds(delay);
            Play();
        }
    }

    public void Play()
    {
        startAngle = transform.eulerAngles.z;
        tween?.Kill();
        RotateByTween();
    }

    public void Pause()
    {
        tween?.Pause();
    }

    public void RotateByTween()
    {
        if (rotState == RotState.Pingpong)
        {
            tween = target.DOLocalRotate(new Vector3(0, 0, startAngle + angle), duration, rotmMode);
            tween.SetLoops(-1, LoopType.Yoyo);
        }
        else if (rotState == RotState.Loop)
        {
            tween = target.DOLocalRotate(new Vector3(0, 0, clockwise ? -360 : 360), duration, rotmMode);
            tween.SetLoops(-1, LoopType.Incremental);
        }
        else
        {
            tween = target.DOLocalRotate(new Vector3(0, 0, startAngle + angle), duration, rotmMode);
        }

        tween.SetEase(ease);
        tween.Play();
    }
}

public enum RotState
{
    Loop,
    Pingpong,
    Incremental
}

#if UNITY_EDITOR
[CustomEditor(typeof(AutoRotation), true), CanEditMultipleObjects]
public class Rotate2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("target"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("clockwise"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ease"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotmMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotState"));

        if (((AutoRotation)target).autoStart)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delay"));
        if (((AutoRotation)target).rotState != RotState.Loop)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("angle"));
            
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif