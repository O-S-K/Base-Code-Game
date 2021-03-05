#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Reflection;

public enum OPTIONS
{
    all = 0,
    spriteRenders = 1,
    cameras = 2,
    materials = 3,
    fog = 4
}

public class ToolColors : EditorWindow
{
    #region Variable
    public OPTIONS opiton;

    /// Object setup ///
    public List<SpriteRenderer> spriteRenders = new List<SpriteRenderer>();
    public List<Camera> cameras = new List<Camera>();
    public List<Material> materials = new List<Material>();

    //// Color ///
    public Color cameraColor = Color.black;
    public Color spriteRenderColor = Color.black;
    public Color materialColor = Color.black;
    public Color fogColor = Color.black;

    /// Fog ///
    public FogMode fogMode = FogMode.Linear;
    public float fogStartDis;
    public float fogEndDis;


    /// Scroll view ///
    Vector2 scrollViewAll;
    Vector2 scrollChildSpriteRender;
    Vector2 scrollChildCamera;
    Vector2 scrollChildMaterial;

    /// bool reatime ///
    bool boolSprite, boolCamera;
    bool boolMaterial, boolFog;

    SerializedObject serializedObject;
    #endregion

    [MenuItem("Tools/Tool Edit Color Object")]
    public static void ShowWindows()
    {
        GetWindow<ToolColors>("Tool Color");
    }
    void OnEnable()
    {
        titleContent = new GUIContent("Edit Color In Object");
        serializedObject = new SerializedObject(this);
    }

    void OnSelectionChange() => Repaint();

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 100F;
        GUILayout.Space(10);
        opiton = (OPTIONS)EditorGUILayout.EnumPopup("OPTION ", opiton);
        GUILayout.Space(20);
        scrollViewAll = GUILayout.BeginScrollView(
            scrollViewAll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
        ChooseOption(opiton);
        GUILayout.EndScrollView();
    }

    void ChooseOption(OPTIONS option)
    {
        switch (option)
        {
            case OPTIONS.all:
                SetupCameras();
                SetupMaterials();
                SetupFog();
                SetupRender();
                break;
            case OPTIONS.cameras:
                SetupCameras();
                break;
            case OPTIONS.materials:
                SetupMaterials();
                break;
            case OPTIONS.spriteRenders:
                SetupRender();
                break;
            case OPTIONS.fog:
                SetupFog();
                break;
            default:
                Debug.Log("Null Option");
                break;
        }
    }
    public void BaseSetupObject(string nameLable, string nameFindProperty = null, Action action = null)
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        GUILayout.Label(nameLable, style, GUILayout.ExpandWidth(true));
        scrollChildCamera = GUILayout.BeginScrollView(scrollChildCamera, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameFindProperty), true);
        GUILayout.EndScrollView();

        if (action != null)
        {
            action();
        }

        GUILayout.BeginVertical(); GUILayout.Space(10f);
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical(); GUILayout.Space(6f);
    }
    public void BaseSetupObject(string nameLable, Action action = null)
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        GUILayout.Label(nameLable, style, GUILayout.ExpandWidth(true));
        scrollChildCamera = GUILayout.BeginScrollView(scrollChildCamera, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
        GUILayout.EndScrollView();
        action?.Invoke();
        GUILayout.BeginVertical(); GUILayout.Space(10f);
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical(); GUILayout.Space(6f);
    }

    void SetupCameras()
    {
        BaseSetupObject("CAMERA", "cameras", (() =>
        {
            boolCamera = EditorGUILayout.Toggle("Edit Reatime", boolCamera);
            cameraColor = EditorGUILayout.ColorField(cameraColor);

            if (GUILayout.Button("Apply") || boolCamera)
            {
                if (cameras.Count > 0)
                {
                    for (int i = 0; i < cameras.Count; i++)
                    {
                        cameras[i].clearFlags = CameraClearFlags.SolidColor;
                        Undo.RecordObject(cameras[i], "Camera Colors");
                        cameras[i].backgroundColor = cameraColor;
                        EditorUtility.SetDirty(cameras[i]);
                    }
                }
            }
            if (GUILayout.Button("Undo"))
            {
                Undo.PerformUndo();
            }
            if (GUILayout.Button("Delete"))
            {
                if (cameras != null)
                {
                    cameraColor = Color.black;
                    cameras.Clear();
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }
        }));
    }
    void SetupMaterials()
    {
        BaseSetupObject("MATERIALS", "materials", (() =>
        {
            boolMaterial = EditorGUILayout.Toggle("Edit Reatime", boolMaterial);
            materialColor = EditorGUILayout.ColorField(materialColor);

            if (GUILayout.Button("Apply") || boolMaterial)
            {
                if (materials.Count > 0)
                {
                    for (int i = 0; i < materials.Count; i++)
                    {
                        Undo.RecordObject(materials[i], "materials Colors");
                        materials[i].color = materialColor;
                        EditorUtility.SetDirty(materials[i]);
                    }
                }
            }
            if (GUILayout.Button("Undo"))
            {
                Undo.PerformUndo();
            }
            if (GUILayout.Button("Delete"))
            {
                if (materials != null)
                {
                    materialColor = Color.black;
                    materials.Clear();
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }
        }));
    }
    void SetupFog()
    {
        BaseSetupObject("FOG", (() =>
        {
            fogStartDis = EditorGUILayout.FloatField("FogStart ", fogStartDis);
            fogEndDis = EditorGUILayout.FloatField("FogEnd ", fogEndDis);

            boolFog = EditorGUILayout.Toggle("Edit Reatime", boolFog);
            fogColor = EditorGUILayout.ColorField(fogColor);

            if (GUILayout.Button("Apply") || boolFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = fogStartDis;
                RenderSettings.fogEndDistance = fogEndDis;
                RenderSettings.fogColor = fogColor;
            }
            if (GUILayout.Button("Undo"))
            {
                Undo.PerformUndo();
            }
            if (GUILayout.Button("Delete"))
            {
                fogColor = Color.black;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }));
    }
    void SetupRender()
    {
        BaseSetupObject("SPRITE RENDER", "spriteRenders", (() =>
        {
            boolSprite = EditorGUILayout.Toggle("Edit Reatime", boolSprite);
            spriteRenderColor = EditorGUILayout.ColorField(spriteRenderColor);

            if (GUILayout.Button("Apply") || boolSprite)
            {
                if (spriteRenders.Count > 0)
                {
                    for (int i = 0; i < spriteRenders.Count; i++)
                    {
                        Undo.RecordObject(spriteRenders[i], "spriteRenders Colors");
                        spriteRenders[i].color = spriteRenderColor;
                        EditorUtility.SetDirty(spriteRenders[i]);
                    }
                }
            }
            if (GUILayout.Button("Undo"))
            {
                Undo.PerformUndo();
            }
            if (GUILayout.Button("Delete"))
            {
                if (spriteRenders != null)
                {
                    spriteRenderColor = Color.black;
                    spriteRenders.Clear();
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }
        }));
    }

    // void SettingEnvoirment()
    // {
    //     RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
    //     RenderSettings.skybox = Color.white;
    //     RenderSettings.ambientEquatorColor,
    //     RenderSettings.ambientGroundColor,
    //     RenderSettings.ambientIntensity 
    // }
}
#endif




