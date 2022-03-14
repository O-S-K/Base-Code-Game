using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Diagnostics; 

public class MenuItemEditor : MonoBehaviour
{
    [MenuItem("Assets/Open with Photoshop")]
    private static void OpenPhotosshop()
    {
        Process photoViewer = new Process();
        photoViewer.StartInfo.FileName = @"D:\App Setup\Adobe Photoshop CC 2018\Photoshop";
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

        AssetDatabase.Refresh();
        UnityEngine.Debug.Log($"<b><color=#ffa500ff>Removed {count} missing scripts</color></b>");
    }

    [MenuItem("Tools/PushIndex+1")]
    public static void ChangeNamePrefab()
    { 
        foreach (var obj in Selection.objects)
        {
            var Path = AssetDatabase.GetAssetPath(obj);
            string namePrefab = obj.name.Split(' ')[0] + " " + (int.Parse(obj.name.Split(' ')[1]) + 1) + ".prefab";
            var newPath = "Assets/Project/Resources/Levels/LevelSwap/" + namePrefab;
            File.Move(Path, newPath);
        }
        AssetDatabase.Refresh();
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
