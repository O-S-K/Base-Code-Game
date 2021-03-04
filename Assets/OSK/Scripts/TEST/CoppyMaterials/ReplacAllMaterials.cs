#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Reflection;
using System.Linq;


public class ReplacAllMaterials : EditorWindow
{
    public enum OPTIONS
    {
        TypeMaterialStandard,
        TypeMaterialURP,
        TypeMaterialHDRP,
        TypeMaterialCustom
    }

    public OPTIONS option = OPTIONS.TypeMaterialStandard;
    public List<GameObject> listGameObjects = new List<GameObject>();
    public Material material;
    Vector2 scrollViewAll;

    SerializedObject serializedObject;
    [MenuItem("Tools/Replace Materials")]
    public static void ShowWindows()
    {
        GetWindow<ReplacAllMaterials>("Replace Materials");
    }

    void OnEnable()
    {
        titleContent = new GUIContent("Replace Materials");
        serializedObject = new SerializedObject(this);
    }

    void OnSelectionChange() => Repaint();

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 100F;
        GUILayout.Space(10);
        scrollViewAll = GUILayout.BeginScrollView(scrollViewAll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
        option = (OPTIONS)EditorGUILayout.EnumPopup("OPTION ", option);
        MaterialReplace();
        GUILayout.Space(20);
        AllMaterials();
        GUILayout.EndScrollView();
    }

    void MaterialReplace()
    {
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        GUILayout.Space(10);
        GUILayout.Label("MATERIALS REPLACE", style, GUILayout.ExpandWidth(true));
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("material"), true);
        GUILayout.Space(10);

        if (GUILayout.Button("Delete"))
        {
            if (material != null)
            {
                material = null;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }
    }

    void AllMaterials()
    {
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        GUILayout.Label("DRAG TO MODEL REPACE MATERIALS", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("listGameObjects"), true);
        GUILayout.Space(10);

        if (GUILayout.Button("Apply"))
        {
            
        }
        if (GUILayout.Button("Undo"))
        {
            Undo.PerformUndo();
        }
        if (GUILayout.Button("Delete"))
        {
            if (listGameObjects != null)
            {
                listGameObjects.Clear();
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        GUILayout.BeginVertical(); GUILayout.Space(10f);
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical(); GUILayout.Space(6f);
    }

    //void OnPreprocessModel()
    //{
    //    ModelImporter modelImporter = assetImporter as ModelImporter;
    //    modelImporter.importMaterials = false;
    //}

    //void OnPostprocessModel(GameObject model)
    //{
    //    Renderer[] renders = model.GetComponentsInChildren<Renderer>();
    //    if (renders == null) return;
    //    foreach (Renderer render in renders)
    //    {
    //        render.sharedMaterials = new Material[render.sharedMaterials.Length];
    //    }
    //}
}
#endif
