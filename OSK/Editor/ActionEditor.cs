using System.IO;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class ActionEditor : MonoBehaviour
{
    [MenuItem("Assets/Open with Photoshop")]
    private static void GetDataSkill()
    {
        Process photoViewer = new Process();
        photoViewer.StartInfo.FileName = @"D:\Quyle\Pts\PhotoshopCS6Portable.exe";
        var inf = new FileInfo(AssetDatabase.GetAssetPath(Selection.activeObject));
        photoViewer.StartInfo.Arguments = inf.FullName;
        photoViewer.Start();
    }
}
