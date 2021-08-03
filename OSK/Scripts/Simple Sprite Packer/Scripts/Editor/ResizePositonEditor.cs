using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleSpritePackerEditor
{
    public class ResizePositonEditor : EditorWindow
    {
        public List<Texture2D> m_Textures;
        private Vector2 scrollPos;
        private static bool isReNameSprite = false;
        private SerializedObject serializedObject;
        private static SPAtlasBuilder m_AtlasBuilder;
        public Direction m_Direction;
        public int left;
        public int right;
        public int top;
        public int bot;
        public bool m_IsAuto = true;

        private void OnEnable()
        {
            m_Textures = new List<Texture2D>();
            titleContent = new GUIContent("SP Reset Position");
            serializedObject = new SerializedObject(this);
            m_AtlasBuilder = new SPAtlasBuilder();
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
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Space(6f);
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none,
                    GUI.skin.verticalScrollbar);
                GUILayout.BeginVertical();
                GUILayout.Space(10f);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Textures"), true);
                if (GUILayout.Button("Remove All"))
                {
                    m_Textures = new List<Texture2D>();
                }

                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                m_IsAuto = GUILayout.Toggle(m_IsAuto, "is Auto Read Size");
                if (!m_IsAuto)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Direction"));
                    left = EditorGUILayout.IntField("left", left);
                    right = EditorGUILayout.IntField("right", right);
                    top = EditorGUILayout.IntField("top", top);
                    bot = EditorGUILayout.IntField("bot", bot);
                    m_Direction = new Direction(left, right, top, bot);
                    if (GUILayout.Button("Reset"))
                    {
                        left = top = right = bot = 0;
                    }
                }

                GUILayout.Space(6f);
                DropAreaGUI();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (m_Textures == null || m_Textures.Count == 0)
            {
                EditorGUILayout.HelpBox("kéo texture vào list", MessageType.Info);
            }
            else
            {
                try
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(6f);
                    GUI.changed = false;

                    GUILayout.Space(6f);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(6f);
                    GUILayout.EndVertical();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                if (GUILayout.Button("Replace"))
                {
                     Debug.Log(m_Textures.Count);
                    for (var i = 0; i < m_Textures.Count; i++)
                    { 
                        m_AtlasBuilder.ResizePosition(m_Textures[i], m_IsAuto, m_Direction);
                    }

                    Debug.Log("Resize Position");
                    EditorUtility.DisplayDialog("Resize Position", "Resize Position succes", "Okay");
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
                            if (!(filtered[i] is Texture2D)) break;
                        }

                        var tmp = new List<Texture2D>();
                        for (int i = 0; i < filtered.Length; i++)
                        {
                            if (filtered[i] is Texture2D)
                            {
                                try
                                {
                                    m_Textures.Remove((Texture2D) filtered[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(e);
                                }
                                tmp.Add((Texture2D) filtered[i]);
                            }
                        }

                        try
                        {
                            m_Textures.AddRange(tmp);
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

        [MenuItem("Window/Simple Sprite Packer/RePositon")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ResizePositonEditor));
        }
    }
}