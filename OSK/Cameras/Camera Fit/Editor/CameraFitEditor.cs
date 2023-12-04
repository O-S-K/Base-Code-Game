using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraFit), true)]
public class CameraFitEditor : Editor
{
    SerializedProperty _heightProp, _widthProp;
    SerializedProperty _VFOVProp, _HFOVProp, _sizeVProp, _sizeHProp;
    SerializedProperty _modeProp, _cameraProp;

    void OnEnable()
    {
        _heightProp = serializedObject.FindProperty("_mainAreaHeight");
        _widthProp = serializedObject.FindProperty("_mainAreaWidth");
        _VFOVProp = serializedObject.FindProperty("_verticalFOV");
        _HFOVProp = serializedObject.FindProperty("_horizontalFOV");
        _modeProp = serializedObject.FindProperty("_adjustMode");
        _sizeVProp = serializedObject.FindProperty("_verticalSize");
        _sizeHProp = serializedObject.FindProperty("_horizontalSize");
        _cameraProp = serializedObject.FindProperty("_ct");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_modeProp);
        CameraFit.Mode md = (CameraFit.Mode)_modeProp.enumValueIndex;
        CameraFit.CameraType ct = (CameraFit.CameraType)_cameraProp.enumValueIndex;
        switch (ct)
        {
            case CameraFit.CameraType.Perspective:                
                switch (md)
                {
                    case CameraFit.Mode.FixedHeight:
                        EditorGUILayout.PropertyField(_VFOVProp, new GUIContent("Vertical FOV"));
                        break;
                    case CameraFit.Mode.FixedWidth:
                        EditorGUILayout.PropertyField(_HFOVProp, new GUIContent("Horizontal FOV"));
                        break;
                    case CameraFit.Mode.Dynamic:
						AspectRatioMenu();
						EditorGUILayout.PropertyField(_HFOVProp, new GUIContent("Main Area Horiz FOV"));
                        EditorGUILayout.PropertyField(_VFOVProp, new GUIContent("Main Area Vert FOV"));
                        break;
                }                
                break;
            case CameraFit.CameraType.Orthographic:                
                switch (md)
                {
                    case CameraFit.Mode.FixedHeight:
                        EditorGUILayout.PropertyField(_sizeVProp, new GUIContent("Vertical Size"));
                        break;
                    case CameraFit.Mode.FixedWidth:
                        EditorGUILayout.PropertyField(_sizeHProp, new GUIContent("Horizontal Size"));
                        break;
                    case CameraFit.Mode.Dynamic:
						AspectRatioMenu();
						EditorGUILayout.PropertyField(_sizeHProp, new GUIContent("Main Area Horiz Size"));
                        EditorGUILayout.PropertyField(_sizeVProp, new GUIContent("Main Area Vert Size"));
                        break;
                }                
                break;
        }        
        serializedObject.ApplyModifiedProperties();
    }

    void AspectRatioMenu()
    {

        EditorGUILayout.LabelField("Main Area Aspect Ratio");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_widthProp, new GUIContent());
        if (_widthProp.floatValue < 1)
        { 
        _widthProp.floatValue = 1;
        }
        EditorGUILayout.PropertyField(_heightProp, new GUIContent());
        if (_heightProp.floatValue < 1)
        {
            _heightProp.floatValue = 1;
        }
        EditorGUILayout.EndHorizontal();
    }

}