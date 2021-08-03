using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using SimpleSpritePacker;
using UnityEditor.U2D;
using Object = UnityEngine.Object;

namespace SimpleSpritePackerEditor
{
    public class SPTools
    {
        public static string Settings_UseSpriteThumbsKey = "SPSettings_UseSpriteThumbs";
        public static string Settings_ThumbsHeightKey = "SPSettings_SpriteThumbsHeight";
        public static string Settings_UseScrollViewKey = "SPSettings_SpriteScrollView";
        public static string Settings_ScrollViewHeightKey = "SPSettings_SpriteScrollViewHeight";
        public static string Settings_DisableReadWriteEnabled = "SPSettings_DisableReadWriteEnabled";
        public static string Settings_AllowMuliSpritesOneSource = "SPSettings_AllowMuliSpritesOneSource";
        public static string Settings_ShowSpritesKey = "SP_ShowSprites";
        public static string Settings_SavedInstanceIDKey = "SP_SavedInstanceID";

        /// <summary>
        /// Prepares the default editor preference values.
        /// </summary>
        public static void PrepareDefaultEditorPrefs()
        {
            if (!EditorPrefs.HasKey(SPTools.Settings_UseScrollViewKey))
            {
                EditorPrefs.SetBool(SPTools.Settings_UseScrollViewKey, true);
            }

            if (!EditorPrefs.HasKey(SPTools.Settings_ScrollViewHeightKey))
            {
                EditorPrefs.SetFloat(SPTools.Settings_ScrollViewHeightKey, 216f);
            }

            if (!EditorPrefs.HasKey(SPTools.Settings_ShowSpritesKey))
            {
                EditorPrefs.SetBool(SPTools.Settings_ShowSpritesKey, true);
            }

            if (!EditorPrefs.HasKey(SPTools.Settings_UseSpriteThumbsKey))
            {
                EditorPrefs.SetBool(SPTools.Settings_UseSpriteThumbsKey, true);
            }

            if (!EditorPrefs.HasKey(SPTools.Settings_ThumbsHeightKey))
            {
                EditorPrefs.SetFloat(SPTools.Settings_ThumbsHeightKey, 50f);
            }

            if (!EditorPrefs.HasKey(SPTools.Settings_AllowMuliSpritesOneSource))
            {
                EditorPrefs.SetBool(SPTools.Settings_AllowMuliSpritesOneSource, true);
            }
        }

        /// <summary>
        /// Gets the editor preference bool value with the specified key.
        /// </summary>
        /// <returns><c>true</c>, if editor preference bool was gotten, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public static bool GetEditorPrefBool(string key)
        {
            return EditorPrefs.GetBool(key);
        }

        /// <summary>
        /// Gets the asset path of a texture.
        /// </summary>
        /// <returns>The asset path.</returns>
        /// <param name="texture">Texture.</param>
        public static string GetAssetPath(Texture2D texture)
        {
            if (texture == null)
                return string.Empty;

            return AssetDatabase.GetAssetPath(texture.GetInstanceID());
        }

        /// <summary>
        /// Gets the asset path of a object.
        /// </summary>
        /// <returns>The asset path.</returns>
        /// <param name="obj">Object.</param>
        public static string GetAssetPath(Object obj)
        {
            if (obj == null)
                return string.Empty;

            return AssetDatabase.GetAssetPath(obj);
        }

        /// <summary>
        /// Does asset reimport.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="options">Options.</param>
        public static void DoAssetReimport(string path, ImportAssetOptions options)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(path, options);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        /// <summary>
        /// Removes the read only flag from the asset.
        /// </summary>
        /// <returns><c>true</c>, if read only flag was removed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        public static bool RemoveReadOnlyFlag(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Clear the read-only flag in texture file attributes
            if (System.IO.File.Exists(path))
            {
#if !UNITY_4_1 && !UNITY_4_0 && !UNITY_3_5
                if (!AssetDatabase.IsOpenForEdit(path))
                {
                    Debug.LogError(path + " is not editable. Did you forget to do a check out?");
                    return false;
                }
#endif
                System.IO.FileAttributes texPathAttrs = System.IO.File.GetAttributes(path);
                texPathAttrs &= ~System.IO.FileAttributes.ReadOnly;
                System.IO.File.SetAttributes(path, texPathAttrs);

                return true;
            }

            // Default
            return false;
        }

        /// <summary>
        /// Creates a blank atlas texture.
        /// </summary>
        /// <returns><c>true</c>, if blank texture was created, <c>false</c> otherwise.</returns>
        /// <param name="path">Asset Path.</param>
        /// <param name="alphaTransparency">If set to <c>true</c> alpha transparency.</param>
        public static bool CreateBlankTexture(string path, bool alphaTransparency)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Prepare blank texture
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            try
            {
                // Create the texture asset
                AssetDatabase.CreateAsset(texture, AssetDatabase.GenerateUniqueAssetPath(path));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (!RemoveReadOnlyFlag(path))
                return false;

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            bytes = null;

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter == null)
                return false;

            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);

            settings.spriteMode = 2;
            settings.readable = false;
            settings.maxTextureSize = 4096;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.npotScale = TextureImporterNPOTScale.ToNearest;
            settings.textureFormat = TextureImporterFormat.ARGB32;
            settings.filterMode = FilterMode.Point;
            settings.aniso = 4;
            settings.alphaIsTransparency = alphaTransparency;

            textureImporter.SetTextureSettings(settings);
            textureImporter.textureType = TextureImporterType.Sprite;

            AssetDatabase.SaveAssets();
            SPTools.DoAssetReimport(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            return true;
        }

        /// <summary>
        /// Imports a texture as asset.
        /// </summary>
        /// <returns><c>true</c>, if texture was imported, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="texture">Texture.</param>
        public static bool ImportTexture(string path, Texture2D texture)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Clear the read-only flag in texture file attributes
            if (!SPTools.RemoveReadOnlyFlag(path))
                return false;

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            bytes = null;

            AssetDatabase.SaveAssets();
            SPTools.DoAssetReimport(path, ImportAssetOptions.ForceSynchronousImport);

            return true;
        }

        /// <summary>
        /// Sets the texture asset Read/Write enabled state.
        /// </summary>
        /// <returns><c>true</c>, if set read write enabled was textured, <c>false</c> otherwise.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="force">If set to <c>true</c> force.</param>
        public static bool TextureSetReadWriteEnabled(Texture2D texture, bool enabled, bool force)
        {
            return SPTools.AssetSetReadWriteEnabled(SPTools.GetAssetPath(texture), enabled, force);
        }

        /// <summary>
        /// Sets the asset Read/Write enabled state.
        /// </summary>
        /// <returns><c>true</c>, if set read write enabled was asseted, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="force">If set to <c>true</c> force.</param>
        public static bool AssetSetReadWriteEnabled(string path, bool enabled, bool force)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            if (ti == null)
                return false;

            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);

            if (force || settings.readable != enabled)
            {
                settings.readable = enabled;
                ti.SetTextureSettings(settings);
                DoAssetReimport(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }

            return true;
        }

        /// <summary>
        /// Sets the asset texture format.
        /// </summary>
        /// <returns><c>true</c>, if set format was set, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="format">Format.</param>
        public static bool AssetSetFormat(string path, TextureImporterFormat format)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            if (ti == null)
                return false;

            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);

            settings.textureFormat = format;
            ti.SetTextureSettings(settings);
            SPTools.DoAssetReimport(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            return true;
        }

        public static bool ImportAndConfigureAtlasTexture(Texture2D targetTexture, Texture2D sourceTexture, Rect[] uvs,
            SPSpriteImportData[] spritesImportData)
        {
            // Get the asset path
            string assetPath = GetAssetPath(targetTexture);

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(
                    "Sprite Packer failed to Import and Configure the atlas texture, reason: Could not resolve asset path.");
                return false;
            }

            if (!SPTools.RemoveReadOnlyFlag(assetPath))
            {
                Debug.LogError(
                    "Sprite Packer failed to Import and Configure the atlas texture, reason: Could not remove the readonly flag from the asset.");
                return false;
            }

            byte[] bytes = sourceTexture.EncodeToPNG();
            File.WriteAllBytes(assetPath, bytes);
            bytes = null;

            TextureImporter texImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (texImporter == null)
            {
                Debug.LogError(
                    "Sprite Packer failed to Import and Configure the atlas texture, reason: Could not get the texture importer for the asset.");
                return false;
            }

            TextureImporterSettings texImporterSettings = new TextureImporterSettings();
            texImporter.textureType = TextureImporterType.Sprite;
            texImporter.spriteImportMode = SpriteImportMode.Multiple;
            SpriteMetaData[] spritesheetMeta = new SpriteMetaData[uvs.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                if (HasSpritesheetMeta(texImporter.spritesheet, spritesImportData[i].name))
                {
                    SpriteMetaData currentMeta = GetSpritesheetMeta(texImporter.spritesheet, spritesImportData[i].name);
                    Rect currentRect = uvs[i];
                    currentRect.x *= sourceTexture.width;
                    currentRect.width *= sourceTexture.width;
                    currentRect.y *= sourceTexture.height;
                    currentRect.height *= sourceTexture.height;
                    currentMeta.rect = currentRect;
                    spritesheetMeta[i] = currentMeta;
                }
                else
                {
                    SpriteMetaData currentMeta = new SpriteMetaData();
                    Rect currentRect = uvs[i];
                    currentRect.x *= sourceTexture.width;
                    currentRect.width *= sourceTexture.width;
                    currentRect.y *= sourceTexture.height;
                    currentRect.height *= sourceTexture.height;
                    currentMeta.rect = currentRect;
                    currentMeta.name = spritesImportData[i].name;
                    currentMeta.alignment = (int) spritesImportData[i].alignment;
                    currentMeta.pivot = spritesImportData[i].pivot;
                    currentMeta.border = spritesImportData[i].border;
                    spritesheetMeta[i] = currentMeta;
                }
            }

            texImporter.spritesheet = spritesheetMeta;
            texImporter.ReadTextureSettings(texImporterSettings);
            texImporterSettings.readable = false;
            texImporter.SetTextureSettings(texImporterSettings);
            DoAssetReimport(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            return true;
        }

        /// <summary>
        /// Imports and configures atlas texture.
        /// </summary>
        /// <returns><c>true</c>, if import and configure atlas texture was successful, <c>false</c> otherwise.</returns>
        /// <param name="targetTexture">Target texture.</param>
        /// <param name="sourceTexture">Source texture.</param>
        /// <param name="uvs">Uvs.</param>
        /// <param name="names">Names.</param>
        /// <param name="defaultPivot">Default pivot.</param>
        /// <param name="defaultCustomPivot">Default custom pivot.</param>
        public static bool ImportAndConfigureAtlasTexture(string assetPath, Texture2D targetTexture, Rect[] uvs,
            SPSpriteImportData[] spritesImportData)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(
                    "Sprite Packer failed to Import and Configure the atlas texture, reason: Could not resolve asset path.");
                return false;
            }

            if (!SPTools.RemoveReadOnlyFlag(assetPath))
            {
                Debug.LogError(
                    "Sprite Packer failed to Import and Configure the atlas texture, reason: Could not remove the readonly flag from the asset.");
                return false;
            }

            byte[] bytes = targetTexture.EncodeToPNG();
            File.WriteAllBytes(assetPath, bytes);

            File.SetAttributes(assetPath + ".meta", FileAttributes.Archive);
            var meta = File.ReadAllLines(assetPath + ".meta").ToList();
            var isDelete = false;
            var nameIndex = 0;
            var spriteIndex = 0;
            for (var i = 0; i < meta.Count; i++)
            {
                if (meta[i].Contains("textureType:"))
                    meta[i] = "  textureType: 8";
                if (meta[i].Contains("  spriteMode:"))
                    meta[i] = "  spriteMode: 2";
                if (!meta[i].Contains("  - first:") && !meta[i].Contains("      213:") &&
                    !meta[i].Contains("    second:"))
                    isDelete = false;
                if (meta[i] == "  spritePackingTag: ") break;
                if (isDelete)
                {
                    meta.Remove(meta[i]);
                }

                if (meta[i].Contains("    sprites:"))
                {
                    meta[i] = "    sprites:";
                    spriteIndex = i + 1;
                    isDelete = true;
                }

                if (meta[i].Contains("  internalIDToNameTable:"))
                {
                    nameIndex = i;
                    isDelete = true;
                }
            }

            var sprites = new List<string>();
            for (var i = 0; i < uvs.Length; i++)
            {
//                var name = new List<string>
//                {
//                    "  - first:", 
//                    "    second: {spritesImportData[i].name}"
//                };
//                names.AddRange(name);
                var sprite = File.ReadAllLines("Assets/Simple Sprite Packer/Sprite Meta.txt");
                sprite[1] = $"      name: {spritesImportData[i].name}";
                sprite[4] = $"        x: {uvs[i].x *= targetTexture.width}";
                sprite[5] = $"        y: {uvs[i].x *= targetTexture.height}";
                sprite[6] = $"        width: {uvs[i].width *= targetTexture.width}";
                sprite[7] = $"        height: {uvs[i].height *= targetTexture.height}";
                sprite[8] = $"      alignment: {(int) spritesImportData[i].alignment}";
                sprite[9] = "      pivot: {x: " + spritesImportData[i].pivot.x + ", y: " +
                            spritesImportData[i].pivot.y + "}";
                sprite[10] = "      border: {x: " + spritesImportData[i].border.x + ", y: " +
                             spritesImportData[i].border.y + ", z: " + spritesImportData[i].border.z + ", w: " +
                             spritesImportData[i].border.w + "}";
                sprites.AddRange(sprite.ToList());
            }

            meta.InsertRange(spriteIndex, sprites);
            File.WriteAllLines(assetPath + ".meta", meta);
            return true;
        }

        /// <summary>
        /// Determines if the specified name has spritesheet meta data.
        /// </summary>
        /// <returns><c>true</c> if has spritesheet meta the specified collection name; otherwise, <c>false</c>.</returns>
        /// <param name="collection">Collection.</param>
        /// <param name="name">Name.</param>
        private static bool HasSpritesheetMeta(SpriteMetaData[] collection, string name)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the spritesheet meta data for the specified name.
        /// </summary>
        /// <returns>The spritesheet meta.</returns>
        /// <param name="collection">Collection.</param>
        /// <param name="name">Name.</param>
        private static SpriteMetaData GetSpritesheetMeta(SpriteMetaData[] collection, string name)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].name == name)
                    return collection[i];
            }

            return new SpriteMetaData();
        }

        /// <summary>
        /// Loads a sprite from a texture.
        /// </summary>
        /// <returns>The sprite.</returns>
        /// <param name="mainTexture">Main texture.</param>
        /// <param name="name">Name.</param>
        public static Sprite LoadSprite(Texture2D mainTexture, string name)
        {
            string texturePath = SPTools.GetAssetPath(mainTexture);
            Object[] atlasAssets = AssetDatabase.LoadAllAssetsAtPath(texturePath);

            foreach (Object asset in atlasAssets)
            {
                if (asset.name == name)
                {
                    var sprite = asset as Sprite;
//                    sprite. = new Vector2();
                    if (sprite != null)
                        return sprite;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if the specified object is main asset.
        /// </summary>
        /// <returns><c>true</c> if is main asset the specified obj; otherwise, <c>false</c>.</returns>
        /// <param name="obj">Object.</param>
        public static bool IsMainAsset(Object obj)
        {
            return AssetDatabase.IsMainAsset(obj);
        }

        /// <summary>
        /// Determines if the specified object has sub assets.
        /// </summary>
        /// <returns><c>true</c> if has sub assets the specified obj; otherwise, <c>false</c>.</returns>
        /// <param name="obj">Object.</param>
        public static bool HasSubAssets(Object obj)
        {
            return (AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj)).Length > 1);
        }

        /// <summary>
        /// Gets the sub assets of an object.
        /// </summary>
        /// <returns>The sub assets.</returns>
        /// <param name="obj">Object.</param>
        public static Object[] GetSubAssets(Object obj)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
            List<Object> subAssets = new List<Object>();

            foreach (Object asset in assets)
            {
                if (AssetDatabase.IsSubAsset(asset))
                    subAssets.Add(asset);
            }

            return subAssets.ToArray();
        }

        /// <summary>
        /// Determines if the specified path is directory.
        /// </summary>
        /// <returns><c>true</c> if is directory the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
        public static bool IsDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return System.IO.Directory.Exists(path);
        }

        /// <summary>
        /// Gets the assets in the specified directory.
        /// </summary>
        /// <returns>The directory assets.</returns>
        /// <param name="path">Path.</param>
        public static Object[] GetDirectoryAssets(string path)
        {
            List<Object> assets = new List<Object>();

            // Get the file paths of all the files in the specified directory
            string[] assetPaths = System.IO.Directory.GetFiles(path);

            // Enumerate through the list of files loading the assets they represent
            foreach (string assetPath in assetPaths)
            {
                // Check if it's a meta file
                if (assetPath.Contains(".meta"))
                    continue;

                Object objAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

                if (objAsset != null)
                    assets.Add(objAsset);
            }

            // Return the array of objects
            return assets.ToArray();
        }

        /// <summary>
        /// Filters the resources for atlas import.
        /// </summary>
        /// <returns>The resources for atlas import.</returns>
        /// <param name="resources">Resources.</param>
        public static Object[] FilterResourcesForAtlasImport(Object[] resources)
        {
            List<Object> tempList = new List<Object>();

            foreach (Object resource in resources)
            {
                string resourcePath = SPTools.GetAssetPath(resource);

                // Check if this is a main asset and queue all it's sub assets
                if (SPTools.IsMainAsset(resource) && SPTools.HasSubAssets(resource))
                {
                    Object[] subAssets = SPTools.FilterResourcesForAtlasImport(SPTools.GetSubAssets(resource));

                    foreach (Object a in subAssets) tempList.Add(a);
                }
                else if (resource is Texture2D || resource is Sprite)
                {
                    tempList.Add(resource);
                }
                else if (SPTools.IsDirectory(resourcePath))
                {
                    Object[] subAssets =
                        SPTools.FilterResourcesForAtlasImport(SPTools.GetDirectoryAssets(resourcePath));

                    foreach (Object a in subAssets) tempList.Add(a);
                }
            }

            return tempList.ToArray();
        }

        [CanBeNull]
        public static SpriteObj GetSpriteObj(SPSpriteInfo tar, string[] targetMeta, string[] thisMeta)
        {
            if (!(tar.source is Sprite)) return null;
            var tarGuild = "";
            for (var i = 0; i < targetMeta.Length; i++)
            {
                if (targetMeta[i].Contains("guid:"))
                    tarGuild = targetMeta[i].Replace("guid: ", "").Trim();
                if (!targetMeta[i].Contains(tar.targetSprite.GetSpriteID().ToString())) continue;
                if (!targetMeta[i + 1].Contains("internalID")) return null;
                var obj = new SpriteObj
                {
                    targetGuild = tarGuild,
                    targetId = targetMeta[i + 1].Replace("internalID: ", "").Trim()
                };
                var thisGuild = "";
                for (var j = 0; j < thisMeta.Length; j++)
                {
                    if (thisMeta[j].Contains("guid:"))
                        thisGuild = thisMeta[j].Replace("guid: ", "").Trim();
                    if (!thisMeta[j].Contains(((Sprite) tar.source).GetSpriteID().ToString())) continue;
                    if (!thisMeta[j + 1].Contains("internalID")) return null;
                    obj.thisGuild = thisGuild;
                    obj.thisId = thisMeta[j + 1].Replace("internalID: ", "").Trim();
                    obj.name = tar.targetSprite.name;
                    if (obj.thisId == "" || obj.thisGuild == "" || obj.targetId == "" || obj.targetGuild == "")
                        return null;
                    return obj;
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Replaces all the references in the supplied array (does not work with internal properties).
        /// </summary>
        /// <param name="spriteObjs"></param>
        /// <param name="replaceMode"></param>
        /// <param name="allFile"></param>
        /// <param name="logName"></param>
        /// <param name="isLog"></param>
        /// <returns>The replaced references count.</returns>
        public static void ReplaceReferences(IReadOnlyList<SpriteObj> spriteObjs, ReplaceMode replaceMode,
            string[] paths, string logName, bool isLog)
        {
            var allFile = new List<string>();
            for (var i = 0; i < paths.Length; i++)
            {
                if (paths[i].IndexOf("Assets/") == -1)
                    continue;
                if (paths[i].Contains(".unity") || paths[i].Contains(".prefab") || paths[i].Contains(".anim") ||
                    paths[i].Contains(".controller") || paths[i].Contains(".mat") || paths[i].Contains(".guiskin") ||
                    paths[i].Contains(".asset"))
                    allFile.Add(paths[i]);
            }

            var count = allFile.Count >= 20 ? 20 : allFile.Count;
            var threads = new List<Thread>();
            if (count >= 20)
            {
                for (var i = 1; i <= 20; i++)
                {
                    var idx = i;
                    var t = new Thread(() =>
                    {
                        for (var index = (idx - 1) * (allFile.Count / 20); index < idx * (allFile.Count / 20); index++)
                        {
                            if (index >= allFile.Count) break;
                            Replace(spriteObjs, allFile[index], replaceMode);
                        }

                        Debug.Log("thread: " + idx + "isAlive");
                        Thread.CurrentThread.Abort();
                    });
                    threads.Add(t);
                    t.Start();
                }
            }

            if (allFile.Count % count > 0)
            {
                for (var i = allFile.Count - allFile.Count % count; i < allFile.Count; i++)
                {
                    if (i >= allFile.Count) break;
                    Replace(spriteObjs, allFile[i], replaceMode);
                }
            }

            while (threads.Count > 0)
            {
                var isAlive = false;
                for (var i = 0; i < threads.Count; i++)
                {
                    if (threads[i].IsAlive) isAlive = true;
                }

                if (!isAlive)
                    break;
            }

            SPReferenceReplacerEditor.isReplace = false;
            SPReferenceReplacerEditor.isReplaceSuccess = true;
            Debug.Log("Replace Success");
            if (!isLog) return;
            var fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\log " + logName;
            if (!File.Exists(fileName))
            {
                File.CreateText(fileName);
            }

            var log = File.ReadAllText(fileName) + "game path:" + SPReferenceReplacerEditor.path + "\n" +
                      DateTime.Now.ToString("g") +
                      "\n" +
                      "{" + SPReferenceReplacerEditor.replaceContent + "\n}\n";
            File.WriteAllText(fileName, log);
            Thread.CurrentThread.Abort();
        }

        public static void Replace(IReadOnlyList<SpriteObj> spriteObjs, string filePath, ReplaceMode replaceMode)
        {
            var isReplace = false;
            var metas = File.ReadAllLines(filePath);
            if (filePath.Contains("BombPrefab")) Debug.Log("");
            switch (replaceMode)
            {
                case ReplaceMode.AtlasWithSource:
                    for (var j = 0; j < metas.Length; j++)
                    {
                        if (!metas[j].Contains("fileID:") || !metas[j].Contains("guid:")) continue;
                        foreach (var spriteObj in spriteObjs)
                        {
                            if (!metas[j].Contains(spriteObj.targetGuild) ||
                                !metas[j].Contains(spriteObj.targetId))
                                continue;
                            metas[j] = metas[j].Replace(spriteObj.targetGuild, spriteObj.thisGuild);
                            metas[j] = metas[j].Replace(spriteObj.targetId, spriteObj.thisId);
                            SPReferenceReplacerEditor.replaceContent +=
                                $"\tReplace file {filePath}: Line {j},SpriteName: {spriteObj.name} \n";
                            isReplace = true;
                            break;
                        }
                    }

                    break;
                case ReplaceMode.SourceWithAtlas:
                    for (var j = 0; j < metas.Length; j++)
                    {
                        if (!metas[j].Contains("fileID:") || !metas[j].Contains("guid:")) continue;
                        foreach (var spriteObj in spriteObjs)
                        {
                            if (!metas[j].Contains(spriteObj.thisGuild) || !metas[j].Contains(spriteObj.thisId))
                                continue;
                            metas[j] = metas[j].Replace(spriteObj.thisGuild, spriteObj.targetGuild);
                            metas[j] = metas[j].Replace(spriteObj.thisId, spriteObj.targetId);
                            SPReferenceReplacerEditor.replaceContent +=
                                $"Replace file {filePath}: Line {j},SpriteName: {spriteObj.name} \n";
                            isReplace = true;
                            break;
                        }
                    }

                    break;
            }

            if (isReplace)
            {
                File.WriteAllLines(filePath, metas);
            }
        }

        public static void ApplyAllTexture()
        {
            var paths = AssetDatabase.GetAllAssetPaths();
            for (var i = 0; i < paths.Length; i++)
            {
                if (paths[i].IndexOf("Assets/") == -1)
                    continue;
                if (paths[i].Contains(".png"))
                {
                    AssetSetReadWriteEnabled(paths[i], !AssetDatabase.LoadAssetAtPath<Texture2D>(paths[i]).isReadable,
                        false);
//                    DrawableTexture.PenTexture(AssetDatabase.LoadAssetAtPath<Texture2D>(paths[i]), new Direction(0, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// Gets all scenes names.
        /// </summary>
        /// <returns>The all scenes names.</returns>
        public static string[] GetAllScenesNames()
        {
            List<string> list = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                list.Add(scene.path);
            }

            return list.ToArray();
        }

        public static Component[] GetProjectPrefabComponents()
        {
            List<Component> result = new List<Component>();

            string[] assets = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in assets)
            {
                if (assetPath.IndexOf("Assets/") == -1)
                    continue;

                UnityEngine.Object assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

                if (assetObj == null)
                    continue;

                if (PrefabUtility.GetPrefabType(assetObj) != PrefabType.None)
                {
                    GameObject gameObject = assetObj as GameObject;
                    if (gameObject != null)
                    {
                        Component[] comps = gameObject.GetComponentsInChildren<Component>(true);

                        foreach (Component comp in comps)
                        {
                            result.Add(comp);
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}