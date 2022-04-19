using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SimpleSpritePacker;
using Object = UnityEngine.Object;

namespace SimpleSpritePackerEditor
{
    public class SPReferenceReplacerEditor : EditorWindow
    {
        public List<SPInstance> m_Instances = new List<SPInstance>();
        private ReplaceMode m_ReplaceMode = ReplaceMode.SourceWithAtlas;
        private Thread thread;
        private int instanceCount = 0;
        private Vector2 scrollPos = new Vector2();
        public static string path;
        public static bool isReplace = false;
        public static bool isReplaceSuccess = false;
        private bool isLog = false;

        public static string PrefsKey_Log = "SPRefReplacer_Log";
        public static string replaceContent = "";
        private SerializedObject serializedObject;

        [MenuItem("Tools/Simple Sprite Packer/Reference Replacer Tool")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SPReferenceReplacerEditor));
        }

        protected void OnEnable()
        {
            path = Application.dataPath;
            this.titleContent = new GUIContent("SP Reference Replacer");
            serializedObject = new SerializedObject(this);
            m_Instances = new List<SPInstance>();
        }

        protected void OnGUI()
        {
            try
            {
                EditorGUIUtility.labelWidth = 100f;
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(6f);
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Space(6f);
                GUILayout.ExpandWidth(true);
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none,
                    GUI.skin.verticalScrollbar);
                GUILayout.BeginVertical();
                GUILayout.Space(10f);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Instances"), true);
                if (GUILayout.Button("Remove All"))
                {
                    m_Instances = new List<SPInstance>();
                }

                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(6f);

                DropAreaGUI();
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Space(6f);

                GUILayout.BeginHorizontal();
                GUILayout.Space(6f);

                EditorGUILayout.LabelField("Replace mode", GUILayout.Width(130f));
                this.m_ReplaceMode = (ReplaceMode) EditorGUILayout.EnumPopup(this.m_ReplaceMode);

                GUILayout.Space(6f);
                GUILayout.EndHorizontal();

                GUILayout.Space(6f);
                GUILayout.EndVertical();

                GUILayout.Space(6f);
                GUILayout.BeginVertical();
                GUILayout.Space(6f);

                GUILayout.BeginHorizontal();
                GUILayout.Space(6f);

                EditorGUILayout.HelpBox(
                    "Source with atlas: gán referent từ texture cũ sang texture mới \nAtlas with source: gán referent từ texture mới sang texture cũ",
                    MessageType.Info);

                GUILayout.Space(6f);
                GUILayout.EndHorizontal();
                GUILayout.Space(6f);
                GUILayout.EndVertical();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (m_Instances == null || m_Instances.Count == 0)
            {
                EditorGUILayout.HelpBox("Please set the sprite packer instance reference in order to use this feature.",
                    MessageType.Info);
            }
            else
            {
                try
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(6f);
                    GUI.changed = false; 
                    isLog = GUILayout.Toggle(EditorPrefs.GetBool(PrefsKey_Log), " General Log File ?");
                    if (GUI.changed)
                    {
                        EditorPrefs.SetBool(PrefsKey_Log, isLog);
                    }

                    GUILayout.Space(6f);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(6f);
                    GUILayout.EndVertical();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }


                GUILayout.Toggle(isReplace, "is Replacing");
                if (!isReplace && GUILayout.Button("Replace"))
                {
                    isReplace = true;
                    replaceContent = "";
                    var spriteObjs = new List<SpriteObj>();

                    foreach (var instance in m_Instances)
                    {
                        if (instance == null) continue;
                        var savePath = "";
                        var bundleName = "";
                        List<string> metaEdit = null;
                        foreach (var tar in instance.copyOfSprites)
                        {
                            if (!(tar.source is Sprite)) continue;
                            var targetPath = AssetDatabase.GetAssetPath(tar.targetSprite);
                            var thisPath = AssetDatabase.GetAssetPath(tar.source);
                            if (targetPath == thisPath) continue;
                            var targetMeta = File.ReadAllLines(targetPath + ".meta");
                            var thisMeta = File.ReadAllLines(thisPath + ".meta");
                            if (m_ReplaceMode == ReplaceMode.SourceWithAtlas)
                            {
                                var bundle = thisMeta.ToList().Find(s => s.Contains("assetBundleName:")).Split(':')[1]
                                    .Remove(0, 1);
                                if (bundle != "")
                                {
                                    if (bundleName == "")
                                    {
                                        bundleName = bundle;
                                    }
                                    else if (bundle != bundleName)
                                    {
                                        EditorUtility.DisplayDialog("Replace Failed",
                                            "file: " + instance.name + ",có sprite khác asset bundle name", "Okay");
                                        return;
                                    }

                                    if (metaEdit == null)
                                    {
                                        savePath = targetPath + ".meta";
                                        metaEdit = targetMeta.ToList();
                                        var index = metaEdit.FindIndex(s => s.Contains("assetBundleName:"));
                                        if (metaEdit[index] == "  assetBundleName: " + bundleName)
                                            metaEdit = null;
                                        else
                                            metaEdit[index] = "  assetBundleName: " + bundleName;
                                    }
                                }
                            }

                            var spriteObj = SPTools.GetSpriteObj(tar, targetMeta, thisMeta);

                            if (spriteObj != null)
                            {
                                spriteObjs.Add(spriteObj);
                            }
                        }

                        if (m_ReplaceMode != ReplaceMode.SourceWithAtlas) continue;
                        if (metaEdit != null)
                        {
                            File.SetAttributes(savePath, FileAttributes.Archive);
                            File.WriteAllLines(savePath, metaEdit);
                        }
                    }

                    var paths = AssetDatabase.GetAllAssetPaths();

                    var log = Application.productName + ".txt";
                    thread = new Thread(() => SPTools.ReplaceReferences(spriteObjs, m_ReplaceMode, paths, log, isLog));
                    thread.Start();
                }

                if (isReplaceSuccess)
                {
                    EditorUtility.DisplayDialog("Reference Replacer",
                        replaceContent != "" ? replaceContent : "No Sprite Replacer", "Okay");
                    isReplaceSuccess = false;
                }
            }

            try
            {
                GUILayout.EndVertical();
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.EndVertical();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private void DropAreaGUI()
        {
            var evt = Event.current;
            var drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

            GUI.color = Color.green;
            GUI.Box(drop_area, "Add SpInstance (Drop Here)",
                new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box)
                    {alignment = TextAnchor.MiddleCenter});
            GUI.color = Color.white;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        Object[] filtered = DragAndDrop.objectReferences;

                        for (int i = 0; i < filtered.Length; i++)
                        {
                            if (!(filtered[i] is SPInstance)) break;
                        }

                        var tmp = new List<SPInstance>();
                        for (int i = 0; i < filtered.Length; i++)
                        {
                            if (filtered[i] is SPInstance)
                            {
                                m_Instances.Remove((SPInstance) filtered[i]);
                                tmp.Add((SPInstance) filtered[i]);
                            }
                        }

                        try
                        {
                            m_Instances.AddRange(tmp);
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }

                    break;
                }
            }
        }
    }

    public enum ReplaceMode : int
    {
        SourceWithAtlas,
        AtlasWithSource
    }

    public class SpriteObj
    {
        public string name;
        public string targetId;
        public string thisId;
        public string targetGuild;
        public string thisGuild;
    }
}