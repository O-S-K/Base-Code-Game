using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleSpritePackerEditor
{
    public class SPCutAllEditor : EditorWindow
    {
        public List<Texture> m_Textures;
        private Vector2 scrollPos;
        private static bool isReNameSprite = false;
        private SerializedObject serializedObject;
        private static SPAtlasBuilder m_AtlasBuilder;

        private void OnEnable()
        {
            this.titleContent = new GUIContent("SP Cut All Sprite");
            serializedObject = new SerializedObject(this);
            m_AtlasBuilder = new SPAtlasBuilder();
            m_Textures = new List<Texture>();
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Textures"), true);


                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.EndScrollView();
                if (GUILayout.Button("Remove All"))
                {
                    m_Textures = new List<Texture>();
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

            if (m_Textures == null || m_Textures.Count == 0)
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
                    isReNameSprite = GUILayout.Toggle(isReNameSprite, " đổi tên các sprite trùng tên?");

                    GUILayout.Space(6f);
                    GUILayout.Space(6f);
                    GUILayout.EndVertical();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                if (GUILayout.Button("Cut All"))
                {
                    m_AtlasBuilder.RebuildAtlas(m_Textures, isReNameSprite);
                    AssetDatabase.Refresh();
                    Debug.Log("cut success");
                    EditorUtility.DisplayDialog("Reference Replacer", "cut success", "Okay");
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
                            if (!(filtered[i] is Texture)) break;
                        }

                        var tmp = new List<Texture>();
                        for (int i = 0; i < filtered.Length; i++)
                        {
                            if (filtered[i] is Texture)
                            {
                                try
                                {
                                    m_Textures.Remove((Texture) filtered[i]);
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(e);
                                }

                                if (((TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(filtered[i])))
                                    .spriteImportMode == SpriteImportMode.Multiple)
                                    tmp.Add((Texture) filtered[i]);
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

        [MenuItem("Window/Simple Sprite Packer/Cut all sprite")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SPCutAllEditor));
        }
    }
}