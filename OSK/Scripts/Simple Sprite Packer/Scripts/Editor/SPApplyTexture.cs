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
    public class SPApplyTexture : EditorWindow
    {
        [MenuItem("Window/Simple Sprite Packer/Apply All Texture")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SPApplyTexture));
        }

        protected void OnEnable()
        {
            this.titleContent = new GUIContent("SP Apply All Texture");
        }

        protected void OnGUI()
        {
            EditorGUIUtility.labelWidth = 100f;
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply"))
            {
                SPTools.ApplyAllTexture();
            }


            try
            {
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.EndVertical();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}