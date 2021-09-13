using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuItemEditor : MonoBehaviour
{
    [MenuItem("Assets/Open with Photoshop")]
    private static void OpenPhotosshop()
    {
        Process photoViewer = new Process();
        photoViewer.StartInfo.FileName = @"D:\Quyle\Pts\PhotoshopCS6Portable.exe";
        var inf = new FileInfo(AssetDatabase.GetAssetPath(Selection.activeObject));
        photoViewer.StartInfo.Arguments = inf.FullName;
        photoViewer.Start();
    }

    [MenuItem("Tools/Remove Missing Scripts In Obect")]
    public static void RemoveMissingScriptsInObect()
    {
        var allObject = Resources.FindObjectsOfTypeAll<GameObject>();
        int count = allObject.Sum(GameObjectUtility.RemoveMonoBehavioursWithMissingScript);
        foreach (var obj in allObject)
        {
            EditorUtility.SetDirty(obj);
        }

        UnityEngine.Debug.Log($"<b><color=#ffa500ff>Removed {count} missing scripts</color></b>");
    }

    //[MenuItem("Tools/ExportScene Package")]
    //public static void BuildPackage()
    //{
    //    var guids = AssetDatabase.FindAssets("", new string[]{
    //        "Assets/PrefabScene",
    //        "Assets/Gizmos"
    //    });

    //    var assets = new string[guids.Length];
    //    for (int i = 0; i < guids.Length; ++i)
    //        assets[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

    //    var file = EditorUtility.SaveFilePanel("Export Package", "Assets/../..", "PrefabScene", "unitypackage");
    //    if (!string.IsNullOrEmpty(file))
    //        AssetDatabase.ExportPackage(assets, file, ExportPackageOptions.Default);
    //}
}
