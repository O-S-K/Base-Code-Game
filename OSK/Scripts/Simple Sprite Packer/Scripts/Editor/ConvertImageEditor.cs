using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleSpritePackerEditor

{
    public class ConvertImageEditor : EditorWindow
    {
        public List<Object> m_Objs;
        public ConvertState m_ConvertMode;
        public EnCodeType m_EnCodeType;
        private Vector2 scrollPos;
        private static bool isReNameSprite = false;
        private SerializedObject serializedObject;

        private void OnEnable()
        {
            m_Objs = new List<Object>();
            this.titleContent = new GUIContent("SP ConvertImage");
            serializedObject = new SerializedObject(this);
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Objs"), true);

                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.EndScrollView();
                if (GUILayout.Button("Remove All"))
                {
                    m_Objs = new List<Object>();
                }

                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
                GUILayout.Space(6f);
                GUILayout.BeginHorizontal();
                GUILayout.EndHorizontal();
                DropAreaGUI();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }


            EditorGUILayout.LabelField("Replace mode", GUILayout.Width(130f));
            m_ConvertMode = (ConvertState) EditorGUILayout.EnumPopup(m_ConvertMode);
            m_EnCodeType = (EnCodeType) EditorGUILayout.EnumPopup(m_EnCodeType);
            if (m_Objs == null || m_Objs.Count == 0)
            {
                EditorGUILayout.HelpBox("kéo file vào list", MessageType.Info);
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

                if (GUILayout.Button("Convert"))
                {
                    for (var i = 0; i < m_Objs.Count; i++)
                    {
                        if (m_ConvertMode == ConvertState.ByteToImage)
                        {
                            try
                            {
                                var path = AssetDatabase.GetAssetPath(m_Objs[i]);
                                var bytes = File.ReadAllBytes(path);
                                var tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
                                tex.LoadImage(bytes);
                                if (m_EnCodeType == EnCodeType.Png)
                                {
                                    var savePath = path.Split('.')[0] + ".png";
                                    File.WriteAllBytes(savePath, tex.EncodeToPNG());
                                }
                                else
                                {
                                    var savePath = path.Split('.')[0] + ".jpg";
                                    File.WriteAllBytes(savePath, tex.EncodeToJPG());
                                }
//                        AssetDatabase.CreateAsset(tex, savePath);
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (m_Objs[i] is Texture)
                                {
                                    var path = AssetDatabase.GetAssetPath(m_Objs[i]);
                                    var bytes = File.ReadAllBytes(path);
                                    var savePath = path.Split('.')[0] + ".bytes";
//                                    File.Delete(path);
                                    File.WriteAllBytes(savePath, bytes);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                            }
                        }
                    }

                    Debug.Log("ConvertImage");
                    EditorUtility.DisplayDialog("ConvertImage", "ConvertImage succes", "Okay");
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
                            if (m_ConvertMode == ConvertState.ByteToImage)
                            {
                                if (!(filtered[i] is TextAsset)) break;
                            }
                            else
                            {
                                if (!(filtered[i] is Texture)) break;
                            }
                        }

                        var tmp = new List<Object>();
                        for (int i = 0; i < filtered.Length; i++)
                        {
                            try
                            {
                                m_Objs.Remove(filtered[i]);
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                            }

                            tmp.Add(filtered[i]);
                        }

                        try
                        {
                            m_Objs.AddRange(tmp);
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

        public enum ConvertState
        {
            ByteToImage,
            ImageToByte
        }

        public enum EnCodeType
        {
            Jpg,
            Png
        }

        [MenuItem("Window/Simple Sprite Packer/ConvertImage")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ConvertImageEditor));
        }
    }
}