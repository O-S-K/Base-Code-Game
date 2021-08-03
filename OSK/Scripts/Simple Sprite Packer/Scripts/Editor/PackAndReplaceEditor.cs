using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SimpleSpritePacker;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleSpritePackerEditor
{
    public class PackAndReplaceEditor : EditorWindow
    {
        public SPInstance m_PackInstance;
        public List<SPInstance> m_ListInstancePack;
        private Vector2 scrollPos;
        private SerializedObject serializedObject;
        private static SPAtlasBuilder m_AtlasBuilder;
        public static bool isReplaceSuccess;
        private bool isReplace;
        public static bool isReplaceFaild;

        private void OnEnable()
        {
            this.titleContent = new GUIContent("SP Pack And Replace Sprite");
            serializedObject = new SerializedObject(this);
            m_AtlasBuilder = new SPAtlasBuilder();
            m_ListInstancePack = new List<SPInstance>();
        }

        private void OnGUI()
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
                GUILayout.Space(6f);
                GUILayout.BeginVertical();
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none,
                    GUI.skin.verticalScrollbar);
                GUILayout.BeginVertical();
                GUILayout.Space(10f);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ListInstancePack"), true);
                if (GUILayout.Button("Remove All"))
                {
                    m_ListInstancePack = new List<SPInstance>();
                }

                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();

                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.EndScrollView();
                GUILayout.Space(6f);
                GUILayout.EndVertical();
                DropAreaGUI();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (m_ListInstancePack == null || m_ListInstancePack.Count == 0)
            {
                EditorGUILayout.HelpBox("kéo texture vào list", MessageType.Info);
            }
            else
            {
                try
                {
                    GUILayout.BeginVertical();
                    GUILayout.Space(6f);
                    GUI.changed = false;
                    GUILayout.Space(6f);
                    GUILayout.EndVertical();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                if (GUILayout.Button("Pack And Replace") && !isReplace)
                {
                    isReplace = true;
                    isReplaceSuccess = false;
                    var sources = new List<Sprite>();
                    m_PackInstance = SPInstanceEditor.CreateInstance(GetNewPath());
                    for (int i = 0; i < m_ListInstancePack.Count; i++)
                    {
                        var copyOfSprites = m_ListInstancePack[i].copyOfSprites;
                        if (copyOfSprites == null || copyOfSprites.Count == 0)
                            continue;
                        sources.Add((Sprite) copyOfSprites[0].source);
                        m_PackInstance.QueueAction_AddSprite(copyOfSprites[0].targetSprite);
                    }

                    m_AtlasBuilder.PackAndReplaceAtlas(m_PackInstance, sources);
                }
            }

            if (isReplaceFaild)
            {
                if (EditorUtility.DisplayDialog("Replace Failed", "có sprite khác asset bundle name", "Okay"))
                {
                    AssetDatabase.Refresh();
                }

                isReplaceFaild = false;
                isReplaceSuccess = false;
                isReplace = false;
            }

            if (isReplaceSuccess)
            {
                EditorUtility.DisplayDialog("Reference Replacer", SPReferenceReplacerEditor.replaceContent != ""
                    ? SPReferenceReplacerEditor.replaceContent
                    : "No Sprite Replacer", "Okay");
                isReplaceSuccess = false;
                isReplace = false;
                Debug.Log("Pack success");
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

        private string GetNewPath()
        {
            var packName = "";
            var nameCount = 0;
            var thisPath = "";

            for (var i = 0; i < m_ListInstancePack.Count; i++)
            {
                {
                    var copyOfSprites = m_ListInstancePack[i].copyOfSprites;
                    if (copyOfSprites == null || copyOfSprites.Count == 0)
                        continue;
                    if (packName == "")
                    {
                        packName = ((Sprite) copyOfSprites[0].source).texture.name;
                        thisPath = AssetDatabase.GetAssetPath(((Sprite) copyOfSprites[0].source).texture);
                        for (var j = 0; j < m_ListInstancePack.Count; j++)
                        {
                            var copyOfSprites2 = m_ListInstancePack[j].copyOfSprites;
                            if (copyOfSprites2 == null || copyOfSprites2.Count == 0)
                                continue;
                            if (((Sprite) copyOfSprites2[0].source).texture.name == packName)
                                nameCount++;
                        }
                    }
                }
                var newName = "";
                var c = 0;
                for (var j = 0; j < m_ListInstancePack.Count; j++)
                {
                    var copyOfSprites = m_ListInstancePack[j].copyOfSprites;
                    if (copyOfSprites == null || copyOfSprites.Count == 0)
                        continue;
                    if (packName == "")
                    {
                        packName = ((Sprite) copyOfSprites[0].source).texture.name;
                        thisPath = AssetDatabase.GetAssetPath(((Sprite) copyOfSprites[0].source).texture);
                        for (var k = 0; k < m_ListInstancePack.Count; k++)
                        {
                            var copyOfSprites2 = m_ListInstancePack[k].copyOfSprites;
                            if (copyOfSprites2 == null || copyOfSprites2.Count == 0)
                                continue;
                            if (((Sprite) copyOfSprites2[0].source).texture.name == packName)
                                nameCount++;
                        }
                    }

                    if (((Sprite) copyOfSprites[0].source).texture.name == packName) continue;
                    if (newName != "" && ((Sprite) copyOfSprites[0].source).texture.name != newName) continue;
                    newName = ((Sprite) copyOfSprites[0].source).texture.name;
                    c++;
                    if (c <= nameCount) continue;
                    thisPath = AssetDatabase.GetAssetPath(((Sprite) copyOfSprites[0].source).texture);
                    break;
                }

                if (c <= nameCount) continue;
                nameCount = c;
                packName = newName;
            }

            if (!File.Exists(thisPath.Split('.')[0] + "/image"))
                Directory.CreateDirectory(thisPath.Split('.')[0] + @"/image");
            var path = thisPath.Split('.')[0] + "/image/" + packName + ".asset";
            if (File.Exists(path))
            {
                File.Delete(path);
                File.Delete(path.Replace(".asset", ".png"));
            }

            return path;
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

                        var filtered = DragAndDrop.objectReferences;
                        for (int i = 0; i < filtered.Length; i++)
                        {
                            if (!(filtered[i] is SPInstance)) break;
                        }

                        var tmp = new List<SPInstance>();
                        for (int i = 0; i < filtered.Length; i++)
                        {
                            if (filtered[i] is SPInstance)
                            {
                                try
                                {
                                    m_ListInstancePack.Remove((SPInstance) filtered[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(e);
                                }

                                tmp.Add((SPInstance) filtered[i]);
                            }
                        }

                        try
                        {
                            m_ListInstancePack.AddRange(tmp);
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

        [MenuItem("Window/Simple Sprite Packer/Pack And Replace")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PackAndReplaceEditor));
        }
    }
}