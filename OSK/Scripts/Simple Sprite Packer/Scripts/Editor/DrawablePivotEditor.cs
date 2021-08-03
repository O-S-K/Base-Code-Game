using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleSpritePacker;
using UnityEditor;
using UnityEngine;

namespace SimpleSpritePackerEditor
{
    public class DrawablePivotEditor : EditorWindow
    {
        public List<SPInstance> m_SPInstances;
        private Vector2 scrollPos;
        private SerializedObject serializedObject;

        private void OnEnable()
        {
            this.titleContent = new GUIContent("SP Drawable Pivot");
            serializedObject = new SerializedObject(this);
            m_SPInstances = new List<SPInstance>();
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
                GUILayout.BeginVertical(GUI.skin.box);
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none,
                    GUI.skin.verticalScrollbar);
                GUILayout.BeginVertical();
                GUILayout.Space(10f);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SPInstances"), true);


                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.EndScrollView();
                if (GUILayout.Button("Remove All"))
                {
                    m_SPInstances = new List<SPInstance>();
                }

                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
                GUILayout.Space(6f);
                DropAreaGUI();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (m_SPInstances == null || m_SPInstances.Count == 0)
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

                if (GUILayout.Button("Drawable Pivot"))
                {
                    for (var i = 0; i < m_SPInstances.Count; i++)
                    {
                        for (var j = 0; j < m_SPInstances[i].copyOfSprites.Count; j++)
                        {
                            var path = AssetDatabase.GetAssetPath(m_SPInstances[i].copyOfSprites[j].targetSprite) + ".meta";
                            var meta = File.ReadAllLines(path).ToList();
                            var index = meta.FindIndex(s => s.Contains("    textureFormat: "));
                            var isReadableIndex = meta.FindIndex(s => s.Contains("isReadable:"));
                            if (!meta[index].Contains("    textureFormat: 4") &&
                                !meta[isReadableIndex].Contains("isReadable: 0")) continue;
                            meta[isReadableIndex] = "  isReadable: 1";
                            meta[index] = "    textureFormat: 4";
                            File.SetAttributes(path, FileAttributes.Archive);
                            File.WriteAllLines(path, meta);
                        }
                    }

                    AssetDatabase.Refresh();

                    for (var i = 0; i < m_SPInstances.Count; i++)
                    {
                        for (var j = 0; j < m_SPInstances[i].copyOfSprites.Count; j++)
                        {
                            DrawablePivot.PenPivot(m_SPInstances[i].copyOfSprites[j].targetSprite);
                        }
                    }

                    for (var i = 0; i < m_SPInstances.Count; i++)
                    {
                        for (var j = 0; j < m_SPInstances[i].copyOfSprites.Count; j++)
                        {
                            var path = AssetDatabase.GetAssetPath(m_SPInstances[i].copyOfSprites[j].targetSprite) +
                                       ".meta";
                            var meta = File.ReadAllLines(path).ToList();
                            var index = meta.FindIndex(s => s.Contains("    textureFormat: "));
                            var isReadableIndex = meta.FindIndex(s => s.Contains("isReadable:"));
                            if (!meta[index].Contains("    textureFormat: 4") &&
                                !meta[isReadableIndex].Contains("isReadable: 1")) continue;
                            meta[isReadableIndex] = "  isReadable: 0";
                            meta[index] = "    textureFormat: -1";
                            File.SetAttributes(path, FileAttributes.Archive);
                            File.WriteAllLines(path, meta);
                        }
                    }

                    EditorUtility.DisplayDialog("Drawable Pivot", "Drawable Pivot", "Okay");
                    AssetDatabase.Refresh();
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
                                    m_SPInstances.Remove((SPInstance) filtered[i]);
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
                            m_SPInstances.AddRange(tmp);
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

        [MenuItem("Window/Simple Sprite Packer/Drawable Pivot")]
        public static void ShowWindow()
        {
            GetWindow(typeof(DrawablePivotEditor));
        }
    }
}