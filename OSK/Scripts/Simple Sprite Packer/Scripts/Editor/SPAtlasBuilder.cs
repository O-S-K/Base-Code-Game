using System;
using UnityEngine;
using UnityEditor;
using SimpleSpritePacker;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SimpleSpritePackerEditor
{
    public class SPAtlasBuilder
    {
        public static int CompareBySize(SPSpriteInfo a, SPSpriteInfo b)
        {
            // A is null b is not b is greater so put it at the front of the list
            if (a == null && b != null) return 1;

            // A is not null b is null a is greater so put it at the front of the list
            if (a != null && b == null) return -1;

            // Get the total pixels used for each sprite
            float aPixels = a.sizeForComparison.x * a.sizeForComparison.y;
            float bPixels = b.sizeForComparison.x * b.sizeForComparison.y;

            if (aPixels > bPixels) return -1;
            else if (aPixels < bPixels) return 1;
            return 0;
        }

        public void CutAtlas(SPInstance m_Instance)
        {
            if (m_Instance == null)
            {
                Debug.LogError(
                    "SPAtlasBuilder failed to rebuild the atlas, reason: Sprite Packer Instance reference is null.");
                return;
            }

            if (m_Instance.texture == null)
            {
                Debug.LogWarning(
                    "Sprite Packer failed to rebuild the atlas, please make sure the atlas texture reference is set.");
                return;
            }

            List<SPSpriteInfo> spriteInfoList = m_Instance.GetSpriteListWithAppliedActions();

            string[] sourceTexturePaths = CollectSourceTextureAssetPaths(spriteInfoList);

//            CorrectTexturesFormat(spriteInfoList);
//
//            SPTools.TextureSetReadWriteEnabled(m_Instance.texture, true, false);

            if (m_Instance.packingMethod == SPInstance.PackingMethod.MaxRects)
            {
                spriteInfoList.Sort(CompareBySize);
            }

            Texture2D[] textures = new Texture2D[spriteInfoList.Count];

            SPSpriteImportData[] spritesImportData = new SPSpriteImportData[spriteInfoList.Count];

            int ia = 0;
            foreach (SPSpriteInfo si in spriteInfoList)
            {
                Texture2D texture = null;

                SPSpriteImportData importData = new SPSpriteImportData {name = "Sprite_" + ia};

                if (si.targetSprite != null)
                {
                    importData.name = si.targetSprite.name;
                }
                else if (si.source != null && (si.source is Texture2D || si.source is Sprite))
                {
                    if (si.source is Texture2D texture2D) importData.name = texture2D.name;
                    else importData.name = ((Sprite) si.source).name;
                }

                if (si.source == null && si.targetSprite != null)
                {
                    texture = new Texture2D((int) si.targetSprite.rect.width, (int) si.targetSprite.rect.height,
                        TextureFormat.ARGB32, false);
                    Color[] pixels = si.targetSprite.texture.GetPixels((int) si.targetSprite.rect.x,
                        (int) si.targetSprite.rect.y,
                        (int) si.targetSprite.rect.width,
                        (int) si.targetSprite.rect.height);
                    texture.SetPixels(pixels);
                }
                else if (si.source is Texture2D sourceTex)
                {
                    if (sourceTex != null)
                    {
                        texture = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.ARGB32, false);
                        Color[] pixels = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
                        texture.SetPixels(pixels);

                        importData.border = Vector4.zero;
                        importData.alignment = m_Instance.defaultPivot;
                        importData.pivot = m_Instance.defaultCustomPivot;
                    }
                }
                else if (si.source is Sprite sourceSprite)
                {
                    if (sourceSprite != null)
                    {
                        texture = new Texture2D((int) sourceSprite.rect.width, (int) sourceSprite.rect.height,
                            TextureFormat.ARGB32, false);
                        Color[] pixels = sourceSprite.texture.GetPixels((int) sourceSprite.rect.x,
                            (int) sourceSprite.rect.y,
                            (int) sourceSprite.rect.width,
                            (int) sourceSprite.rect.height);
                        texture.SetPixels(pixels);

                        importData.border = sourceSprite.border;
                        importData.alignment = SpriteAlignment.Custom;
                        importData.pivot = new Vector2(
                            (0f - sourceSprite.bounds.center.x / sourceSprite.bounds.extents.x / 2 + 0.5f),
                            (0f - sourceSprite.bounds.center.y / sourceSprite.bounds.extents.y / 2 + 0.5f));
                    }
                }

                textures[ia] = (texture != null) ? texture : new Texture2D(1, 1);

                spritesImportData[ia] = importData;

                ia++;
            }

            System.Array.Clear(sourceTexturePaths, 0, sourceTexturePaths.Length);
            Rect[] uvs = {new Rect(0, 0, 1, 1)};

            SPTools.ImportAndConfigureAtlasTexture(AssetDatabase.GetAssetPath(m_Instance.texture), textures[0], uvs, spritesImportData);
        }

        public void RebuildAtlas(SPInstance m_Instance)
        {
            if (m_Instance == null)
            {
                Debug.LogError(
                    "SPAtlasBuilder failed to rebuild the atlas, reason: Sprite Packer Instance reference is null.");
                return;
            }

            if (m_Instance.texture == null)
            {
                Debug.LogWarning(
                    "Sprite Packer failed to rebuild the atlas, please make sure the atlas texture reference is set.");
                return;
            }

            // Make the atlas texture readable
            if (SPTools.TextureSetReadWriteEnabled(m_Instance.texture, true, false))
            {
                // Get a list with the current sprites and applied actions
                List<SPSpriteInfo> spriteInfoList = m_Instance.GetSpriteListWithAppliedActions();

                // Get the source textures asset paths
                string[] sourceTexturePaths = this.CollectSourceTextureAssetPaths(spriteInfoList);

                // Make the source textures readable
                if (!this.SetAssetsReadWriteEnabled(sourceTexturePaths, true))
                {
                    Debug.LogError(
                        "Sprite Packer failed to make one or more of the source texture readable, please do it manually.");
                    return;
                }

                // Make sure all the textures have the correct texture format
                this.CorrectTexturesFormat(spriteInfoList);

                // If we are using max rects packing, sort the sprite info list by size
                if (m_Instance.packingMethod == SPInstance.PackingMethod.MaxRects)
                {
                    spriteInfoList.Sort(CompareBySize);
                }

                // Temporary textures array
                Texture2D[] textures = new Texture2D[spriteInfoList.Count];

                // Create an array to contain the sprite import data
                SPSpriteImportData[] spritesImportData = new SPSpriteImportData[spriteInfoList.Count];

                // Populate the textures and names arrays
                int ia = 0;
                foreach (SPSpriteInfo si in spriteInfoList)
                {
                    // Temporary texture
                    Texture2D texture = null;

                    // Prepare the sprite import data
                    SPSpriteImportData importData = new SPSpriteImportData();

                    // Prepare the sprite name
                    importData.name = "Sprite_" + ia.ToString();

                    if (si.targetSprite != null)
                    {
                        importData.name = si.targetSprite.name;
                    }
                    else if (si.source != null && (si.source is Texture2D || si.source is Sprite))
                    {
                        if (si.source is Texture2D) importData.name = (si.source as Texture2D).name;
                        else importData.name = (si.source as Sprite).name;
                    }

                    // Prepare texture
                    // In case the source texture is missing, rebuild from the already existing sprite
                    if (si.source == null && si.targetSprite != null)
                    {
                        // Copy the sprite into the temporary texture
                        texture = new Texture2D((int) si.targetSprite.rect.width, (int) si.targetSprite.rect.height,
                            TextureFormat.ARGB32, false);
                        Color[] pixels = si.targetSprite.texture.GetPixels((int) si.targetSprite.rect.x,
                            (int) si.targetSprite.rect.y,
                            (int) si.targetSprite.rect.width,
                            (int) si.targetSprite.rect.height);
                        texture.SetPixels(pixels);
                        texture.Apply();
                    }
                    // Handle texture source
                    else if (si.source is Texture2D)
                    {
                        // Get as texture
                        Texture2D sourceTex = si.source as Texture2D;

                        // Check if we have as source texture
                        if (sourceTex != null)
                        {
                            // Copy the source texture into the temp one
                            texture = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.ARGB32, false);
                            Color[] pixels = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
                            texture.SetPixels(pixels);
                            texture.Apply();

                            // Transfer the sprite data
                            importData.border = Vector4.zero;
                            importData.alignment = m_Instance.defaultPivot;
                            importData.pivot = m_Instance.defaultCustomPivot;
                        }
                    }
                    // Handle sprite source
                    else if (si.source is Sprite)
                    {
                        // Get as sprite
                        Sprite sourceSprite = si.source as Sprite;

                        // Make sure we have the sprite
                        if (sourceSprite != null)
                        {
                            // Copy the sprite into the temporary texture
                            texture = new Texture2D((int) sourceSprite.rect.width, (int) sourceSprite.rect.height,
                                TextureFormat.ARGB32, false);
                            Color[] pixels = sourceSprite.texture.GetPixels((int) sourceSprite.rect.x,
                                (int) sourceSprite.rect.y,
                                (int) sourceSprite.rect.width,
                                (int) sourceSprite.rect.height);
                            texture.SetPixels(pixels);
                            texture.Apply();

                            // Transfer the sprite data
                            importData.border = sourceSprite.border;
                            importData.alignment = SpriteAlignment.Custom;
                            importData.pivot = new Vector2(
                                (0f - sourceSprite.bounds.center.x / sourceSprite.bounds.extents.x / 2 + 0.5f),
                                (0f - sourceSprite.bounds.center.y / sourceSprite.bounds.extents.y / 2 + 0.5f));
                        }
                    }

                    // Save the new texture into our array
                    textures[ia] = (texture != null) ? texture : new Texture2D(1, 1);

                    // Set the sprite import data
                    spritesImportData[ia] = importData;

                    // Increase the indexer
                    ia++;
                }

                // Make the source textures assets non readable
                if (SPTools.GetEditorPrefBool(SPTools.Settings_DisableReadWriteEnabled))
                    this.SetAssetsReadWriteEnabled(sourceTexturePaths, false);

                // Clear the source textures asset paths
                System.Array.Clear(sourceTexturePaths, 0, sourceTexturePaths.Length);

                // Create a temporary texture for the packing
                Texture2D tempTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

                // UV coords array
                Rect[] uvs;

                // Pack the textures into the temporary
                if (m_Instance.packingMethod == SPInstance.PackingMethod.Unity)
                {
                    uvs = tempTexture.PackTextures(textures, m_Instance.padding, m_Instance.maxSize);
                }
                else
                {
                    uvs = UITexturePacker.PackTextures(tempTexture, textures, m_Instance.padding, m_Instance.maxSize);

                    // Check if packing failed
                    if (uvs == null)
                    {
                        Debug.LogError(
                            "Sprite Packer texture packing failed, the textures might be exceeding the specified maximum size.");
                        return;
                    }
                }

                // Import and Configure the texture atlas (also disables Read/Write)
                SPTools.ImportAndConfigureAtlasTexture(m_Instance.texture, tempTexture, uvs, spritesImportData);

                // Clear the current sprite info list
                m_Instance.ClearSprites();

                // Clear the actions list
                m_Instance.ClearActions();

                // Destroy the textures from the temporary textures array
                for (int ib = 0; ib < textures.Length; ib++)
                    UnityEngine.Object.DestroyImmediate(textures[ib]);

                // Destroy the temporary texture
                UnityEngine.Object.DestroyImmediate(tempTexture);

                // Convert the temporary sprite info into array
                SPSpriteInfo[] spriteInfoArray = spriteInfoList.ToArray();

                // Clear the temporary sprite info list
                spriteInfoList.Clear();

                // Apply the new sprite reff to the sprite info and add the sprite info to the sprites list
                for (int i = 0; i < spriteInfoArray.Length; i++)
                {
                    SPSpriteInfo info = spriteInfoArray[i];

                    if (info.targetSprite == null)
                        info.targetSprite = SPTools.LoadSprite(m_Instance.texture, spritesImportData[i].name);

                    // Add to the instance sprite info list
                    m_Instance.AddSprite(info);
                }

                // Clear the sprites import data array
                System.Array.Clear(spritesImportData, 0, spritesImportData.Length);

                // Set dirty
                EditorUtility.SetDirty(m_Instance);
            }
            else
            {
                Debug.LogError("Sprite Packer failed to make the atlas texture readable, please do it manually.");
            }
        }

        public void PackAndReplaceAtlas(SPInstance m_Instance, List<Sprite> sourceSprites)
        {
            PackAtlas(m_Instance, sourceSprites);
            ReplaceAtlas(m_Instance);
        }

        private void PackAtlas(SPInstance m_Instance, List<Sprite> sourceSprites)
        {
            if (m_Instance == null)
            {
                Debug.LogError(
                    "SPAtlasBuilder failed to rebuild the atlas, reason: Sprite Packer Instance reference is null.");
                return;
            }

            if (m_Instance.texture == null)
            {
                Debug.LogWarning(
                    "Sprite Packer failed to rebuild the atlas, please make sure the atlas texture reference is set.");
                return;
            }

            // Make the atlas texture readable
            if (SPTools.TextureSetReadWriteEnabled(m_Instance.texture, true, false))
            {
                // Get a list with the current sprites and applied actions
                List<SPSpriteInfo> spriteInfoList = m_Instance.GetSpriteListWithAppliedActions();

                // Get the source textures asset paths
                string[] sourceTexturePaths = this.CollectSourceTextureAssetPaths(spriteInfoList);

                // Make the source textures readable
                if (!this.SetAssetsReadWriteEnabled(sourceTexturePaths, true))
                {
                    Debug.LogError(
                        "Sprite Packer failed to make one or more of the source texture readable, please do it manually.");
                    return;
                }

                // Make sure all the textures have the correct texture format
                this.CorrectTexturesFormat(spriteInfoList);

                // If we are using max rects packing, sort the sprite info list by size
                if (m_Instance.packingMethod == SPInstance.PackingMethod.MaxRects)
                {
                    spriteInfoList.Sort(CompareBySize);
                }

                // Temporary textures array
                Texture2D[] textures = new Texture2D[spriteInfoList.Count];

                // Create an array to contain the sprite import data
                SPSpriteImportData[] spritesImportData = new SPSpriteImportData[spriteInfoList.Count];

                // Populate the textures and names arrays
                int ia = 0;
                foreach (SPSpriteInfo si in spriteInfoList)
                {
                    // Temporary texture
                    Texture2D texture = null;

                    // Prepare the sprite import data
                    SPSpriteImportData importData = new SPSpriteImportData();

                    // Prepare the sprite name
                    importData.name = "Sprite_" + ia.ToString();

                    if (si.targetSprite != null)
                    {
                        importData.name = si.targetSprite.name;
                    }
                    else if (si.source != null && (si.source is Texture2D || si.source is Sprite))
                    {
                        if (si.source is Texture2D) importData.name = (si.source as Texture2D).name;
                        else importData.name = (si.source as Sprite).name;
                    }

                    // Prepare texture
                    // In case the source texture is missing, rebuild from the already existing sprite
                    if (si.source == null && si.targetSprite != null)
                    {
                        // Copy the sprite into the temporary texture
                        texture = new Texture2D((int) si.targetSprite.rect.width, (int) si.targetSprite.rect.height,
                            TextureFormat.ARGB32, false);
                        Color[] pixels = si.targetSprite.texture.GetPixels((int) si.targetSprite.rect.x,
                            (int) si.targetSprite.rect.y,
                            (int) si.targetSprite.rect.width,
                            (int) si.targetSprite.rect.height);
                        texture.SetPixels(pixels);
                        texture.Apply();
                    }
                    // Handle texture source
                    else if (si.source is Texture2D)
                    {
                        // Get as texture
                        Texture2D sourceTex = si.source as Texture2D;

                        // Check if we have as source texture
                        if (sourceTex != null)
                        {
                            // Copy the source texture into the temp one
                            texture = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.ARGB32, false);
                            Color[] pixels = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
                            texture.SetPixels(pixels);
                            texture.Apply();

                            // Transfer the sprite data
                            importData.border = Vector4.zero;
                            importData.alignment = m_Instance.defaultPivot;
                            importData.pivot = m_Instance.defaultCustomPivot;
                        }
                    }
                    // Handle sprite source
                    else if (si.source is Sprite)
                    {
                        // Get as sprite
                        Sprite sourceSprite = si.source as Sprite;

                        // Make sure we have the sprite
                        if (sourceSprite != null)
                        {
                            // Copy the sprite into the temporary texture
                            texture = new Texture2D((int) sourceSprite.rect.width, (int) sourceSprite.rect.height,
                                TextureFormat.ARGB32, false);
                            Color[] pixels = sourceSprite.texture.GetPixels((int) sourceSprite.rect.x,
                                (int) sourceSprite.rect.y,
                                (int) sourceSprite.rect.width,
                                (int) sourceSprite.rect.height);
                            texture.SetPixels(pixels);
                            texture.Apply();

                            // Transfer the sprite data
                            importData.border = sourceSprite.border;
                            importData.alignment = SpriteAlignment.Custom;
                            importData.pivot = new Vector2(
                                (0f - sourceSprite.bounds.center.x / sourceSprite.bounds.extents.x / 2 + 0.5f),
                                (0f - sourceSprite.bounds.center.y / sourceSprite.bounds.extents.y / 2 + 0.5f));
                        }
                    }

                    // Save the new texture into our array
                    textures[ia] = (texture != null) ? texture : new Texture2D(1, 1);

                    // Set the sprite import data
                    spritesImportData[ia] = importData;

                    // Increase the indexer
                    ia++;
                }

                // Make the source textures assets non readable
                if (SPTools.GetEditorPrefBool(SPTools.Settings_DisableReadWriteEnabled))
                    this.SetAssetsReadWriteEnabled(sourceTexturePaths, false);

                // Clear the source textures asset paths
                System.Array.Clear(sourceTexturePaths, 0, sourceTexturePaths.Length);

                // Create a temporary texture for the packing
                Texture2D tempTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

                // UV coords array
                Rect[] uvs;

                // Pack the textures into the temporary
                if (m_Instance.packingMethod == SPInstance.PackingMethod.Unity)
                {
                    uvs = tempTexture.PackTextures(textures, m_Instance.padding, m_Instance.maxSize);
                }
                else
                {
                    uvs = UITexturePacker.PackTextures(tempTexture, textures, m_Instance.padding, m_Instance.maxSize);

                    // Check if packing failed
                    if (uvs == null)
                    {
                        Debug.LogError(
                            "Sprite Packer texture packing failed, the textures might be exceeding the specified maximum size.");
                        return;
                    }
                }

                // Import and Configure the texture atlas (also disables Read/Write)
                SPTools.ImportAndConfigureAtlasTexture(m_Instance.texture, tempTexture, uvs, spritesImportData);

                // Clear the current sprite info list
                m_Instance.ClearSprites();

                // Clear the actions list
                m_Instance.ClearActions();

                // Destroy the textures from the temporary textures array
                for (int ib = 0; ib < textures.Length; ib++)
                    UnityEngine.Object.DestroyImmediate(textures[ib]);

                // Destroy the temporary texture
                UnityEngine.Object.DestroyImmediate(tempTexture);

                // Convert the temporary sprite info into array
                SPSpriteInfo[] spriteInfoArray = spriteInfoList.ToArray();

                // Clear the temporary sprite info list
                spriteInfoList.Clear();

                // Apply the new sprite reff to the sprite info and add the sprite info to the sprites list
                for (int i = 0; i < spriteInfoArray.Length; i++)
                {
                    SPSpriteInfo info = spriteInfoArray[i];

                    if (info.targetSprite == null)
                        info.targetSprite = SPTools.LoadSprite(m_Instance.texture, spritesImportData[i].name);

                    // Add to the instance sprite info list
                    m_Instance.AddSprite(info);
                }

                // Clear the sprites import data array
                System.Array.Clear(spritesImportData, 0, spritesImportData.Length);

                // Set dirty
                EditorUtility.SetDirty(m_Instance);
            }
            else
            {
                Debug.LogError("Sprite Packer failed to make the atlas texture readable, please do it manually.");
            }

            for (int i = 0; i < m_Instance.copyOfSprites.Count; i++)
            {
                var sp = sourceSprites.Find(s => s.name == m_Instance.copyOfSprites[i].targetSprite.name);
                m_Instance.copyOfSprites[i].source = sp;
                sourceSprites.Remove(sp);
            }
        }

        private void ReplaceAtlas(SPInstance m_Instance)
        {
            var spriteObjs = new List<SpriteObj>();
            var bundleName = "";
            var isEditAssetBundle = true;
            foreach (var tar in m_Instance.copyOfSprites)
            {
                if (!(tar.source is Sprite)) continue;
                var targetPath = AssetDatabase.GetAssetPath(tar.targetSprite);
                var thisPath = AssetDatabase.GetAssetPath(tar.source);
                if (targetPath == thisPath) continue;
                var targetMeta = File.ReadAllLines(targetPath + ".meta");
                var thisMeta = File.ReadAllLines(thisPath + ".meta");
                var bundle = thisMeta.ToList().Find(s => s.Contains("assetBundleName:")).Split(':')[1].Remove(0, 1);
                if (bundle != "")
                {
                    if (bundleName == "")
                    {
                        bundleName = bundle;
                    }
                    else if (bundle != bundleName)
                    {
                        PackAndReplaceEditor.isReplaceFaild = true;
                        var folder = (Directory.GetCurrentDirectory() + "/" + AssetDatabase.GetAssetPath(m_Instance)
                                          .Replace("/" + m_Instance.name + ".asset", "")).Replace("/", @"\");
                        for (int i = 0; i < Directory.GetFiles(folder).Length; i++)
                        {
                            File.Delete(Directory.GetFiles(folder)[i]);
                        }

                        AssetDatabase.Refresh();
                        Directory.Delete(folder, true);
                        return;
                    }

                    if (isEditAssetBundle)
                    {
                        var metaEdit = targetMeta.ToList();
                        var index = metaEdit.FindIndex(s => s.Contains("assetBundleName:"));
                        if (metaEdit[index] != "  assetBundleName: " + bundleName)
                        {
                            metaEdit[index] = "  assetBundleName: " + bundleName;
                            File.SetAttributes(targetPath + ".meta", FileAttributes.Archive);
                            File.WriteAllLines(targetPath + ".meta", metaEdit);
                        }

                        isEditAssetBundle = false;
                    }
                }


                var spriteObj = SPTools.GetSpriteObj(tar, targetMeta, thisMeta);

                if (spriteObj != null)
                {
                    spriteObjs.Add(spriteObj);
                }
            }

            var productName = Application.productName + ".txt";
            var paths = AssetDatabase.GetAllAssetPaths().ToList();
            paths.Remove(AssetDatabase.GetAssetPath(m_Instance));
            new Thread(() =>
            {
                SPTools.ReplaceReferences(spriteObjs, ReplaceMode.SourceWithAtlas, paths.ToArray(), productName, true);
                PackAndReplaceEditor.isReplaceSuccess = true;
            }).Start();
        }

        /// <summary>
        /// Rebuilds the atlas texture.
        /// </summary>
        public void RebuildAtlas(List<Texture> texture, bool isRename)
        {
            if (texture == null)
            {
                Debug.LogWarning(
                    "Sprite Packer failed to rebuild the atlas, please make sure the atlas texture reference is set.");
                return;
            }

            for (var idx = 0; idx < texture.Count; idx++)
            {
                var thisPath = AssetDatabase.GetAssetPath(texture[idx]);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(thisPath).OfType<Sprite>();
                if (!File.Exists(thisPath.Split('.')[0]))
                    Directory.CreateDirectory(thisPath.Split('.')[0]);

                foreach (var s in sprites)
                {
                    var path = thisPath.Split('.')[0] + @"\" + s.name + ".asset";
                    if (File.Exists(path))
                    {
                        if (isRename)
                        {
                            var i = 1;
                            while (File.Exists(path) || File.Exists(path.Replace(".asset", ".png")))
                            {
                                path = thisPath.Split('.')[0] + @"\" + s.name + "_" + i + ".asset";
                                i++;
                            }
                        }
                        else
                        {
                            File.Delete(path);
                            File.Delete(path.Replace(".asset", ".png"));
                        }
                    }

                    SPInstanceEditor.CreateCutInstance(path);
                }
            }

            AssetDatabase.Refresh();
            for (var idx = 0; idx < texture.Count; idx++)
            {
                var thisPath = AssetDatabase.GetAssetPath(texture[idx]);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(thisPath).OfType<Sprite>();
                foreach (var s in sprites)
                {
                    var path = thisPath.Split('.')[0] + @"\" + s.name + ".png.meta";
                    File.SetAttributes(path, FileAttributes.Archive);
                    File.WriteAllText(path, File.ReadAllText(path).Replace("isReadable: 0", "isReadable: 1"));
                }
            }

            var al = new SPAtlasBuilder();
            for (var idx = 0; idx < texture.Count; idx++)
            {
                var thisPath = AssetDatabase.GetAssetPath(texture[idx]);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(thisPath).OfType<Sprite>();

                foreach (var s in sprites)
                {
                    var path = thisPath.Split('.')[0] + @"\" + s.name + ".asset";
                    var instance = AssetDatabase.LoadAssetAtPath<SPInstance>(path);
                    instance.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path.Replace(".asset", ".png"));
                    instance.QueueAction_AddSprite(s);
                    al.RebuildAtlas(instance);
                    EditorUtility.SetDirty(instance);
                }
            }

//            AssetDatabase.Refresh();
          
            /*for (var idx = 0; idx < texture.Count; idx++)
            {
                var thisPath = AssetDatabase.GetAssetPath(texture[idx]);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(thisPath).OfType<Sprite>();

                foreach (var s in sprites)
                {
                    var path = thisPath.Split('.')[0] + @"\" + s.name + ".asset";
                    var instance = AssetDatabase.LoadAssetAtPath<SPInstance>(path);
                    var info = new SPSpriteInfo {source = s};
                    instance.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path.Replace(".asset", ".png"));
                    if (info.targetSprite == null)
                        info.targetSprite = SPTools.LoadSprite(instance.texture, s.name);

                    instance.AddSprite(info);
                    EditorUtility.SetDirty(instance);
                }
            }*/
        }

        /// <summary>
        /// Collects the source textures asset paths.
        /// </summary>
        /// <returns>The source texture asset paths.</returns>
        /// <param name="spriteInfoList">Sprite info list.</param>
        protected string[] CollectSourceTextureAssetPaths(List<SPSpriteInfo> spriteInfoList)
        {
            List<string> texturePaths = new List<string>();

            // Add the textures from the sprite info list into our textures list
            foreach (SPSpriteInfo spriteInfo in spriteInfoList)
            {
                string path = string.Empty;

                // No source but present target sprite
                if (spriteInfo.source == null && spriteInfo.targetSprite != null)
                {
                    path = SPTools.GetAssetPath(spriteInfo.targetSprite.texture);
                }
                // Texture source
                else if (spriteInfo.source is Texture2D)
                {
                    path = SPTools.GetAssetPath(spriteInfo.source as Texture2D);
                }
                // Sprite source
                else if (spriteInfo.source is Sprite)
                {
                    path = SPTools.GetAssetPath((spriteInfo.source as Sprite).texture);
                }

                if (!string.IsNullOrEmpty(path))
                {
                    if (!texturePaths.Contains(path))
                        texturePaths.Add(path);
                }
            }

            return texturePaths.ToArray();
        }

        /// <summary>
        /// Sets the assets read write enabled.
        /// </summary>
        /// <returns><c>true</c>, if assets read write enabled was set, <c>false</c> otherwise.</returns>
        /// <param name="assetPaths">Asset paths.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        protected bool SetAssetsReadWriteEnabled(string[] assetPaths, bool enabled)
        {
            bool success = true;

            // Make the assets readable
            foreach (string assetPath in assetPaths)
            {
                // Make the texture readable
                if (!SPTools.AssetSetReadWriteEnabled(assetPath, enabled, false))
                {
                    Debug.LogWarning("Sprite Packer failed to set Read/Write state (" + enabled.ToString() +
                                     ") on asset: " + assetPath);
                    success = false;
                }
            }

            // Return the result
            return success;
        }

        /// <summary>
        /// Corrects the textures format.
        /// </summary>
        /// <param name="spriteInfoList">Sprite info list.</param>
        protected void CorrectTexturesFormat(List<SPSpriteInfo> spriteInfoList)
        {
            if (spriteInfoList == null || spriteInfoList.Count == 0)
                return;

            foreach (SPSpriteInfo spriteInfo in spriteInfoList)
            {
                Texture2D texture = null;

                // No source but present target sprite
                if (spriteInfo.source == null && spriteInfo.targetSprite != null)
                {
                    texture = spriteInfo.targetSprite.texture;
                }
                // Texture source
                else if (spriteInfo.source is Texture2D texture2D)
                {
                    texture = texture2D;
                }
                // Sprite source
                else if (spriteInfo.source is Sprite sprite)
                {
                    texture = sprite.texture;
                }

                if (texture != null)
                {
                    // Make sure it's the correct format
                    if (texture.format != TextureFormat.ARGB32 &&
                        texture.format != TextureFormat.RGBA32 &&
                        texture.format != TextureFormat.BGRA32 &&
                        texture.format != TextureFormat.RGB24 &&
                        texture.format != TextureFormat.Alpha8 &&
                        texture.format != TextureFormat.DXT1 &&
                        texture.format != TextureFormat.DXT5)
                    {
                        // Get the texture asset path
                        string assetPath = SPTools.GetAssetPath(texture);

                        // Set new texture format
                        if (!SPTools.AssetSetFormat(assetPath, TextureImporterFormat.ARGB32))
                        {
                            Debug.LogWarning("Sprite Packer failed to set texture format ARGB32 on asset: " +
                                             assetPath);
                        }
                    }
                }
            }
        }

        public void ResizePosition(Texture2D mTexture, bool isAuto, Direction direction)
        {
            var path = Directory.GetCurrentDirectory() + @"\" + AssetDatabase.GetAssetPath(mTexture);
            FileAttributes attributes = File.GetAttributes(path);
            File.SetAttributes(path + ".meta", FileAttributes.Archive);
            var meta = File.ReadAllLines(path + ".meta");
            var isEdit = false;
            var dir = isAuto ? new Direction() : direction;
            if (isAuto)
            {
                Debug.LogError("m" + meta.Length);
                // for (var i = 0; i < meta.Length; i++)
                // {
                //     // if (!meta[i].Contains("width")) continue;
                //     // if (!meta[i - 1].Contains("y:") || !meta[i - 2].Contains("x")) continue;
                //     isEdit = true;

                //     var position = Mathf.RoundToInt(float.Parse(meta[i - 2].Split(':')[1].Trim()));
                //     var width = Mathf.RoundToInt(float.Parse(meta[i].Split(':')[1].Trim()));
                //     if (position < 0)
                //     {
                //         if (-dir.left > position)
                //         {
                //             dir.left = -position;
                //         }
                //     }
                //     else if (position + width > mTexture.width)
                //     {
                //         if (-dir.right > mTexture.width - position - width)
                //         {
                //             dir.right = position + width - mTexture.width;
                //         }
                //     }

                //     if (meta[i - 1].Split(':')[1].Trim() == "" || meta[i + 1].Split(':')[1].Trim() == "") continue;
                //     position = Mathf.RoundToInt(float.Parse(meta[i - 1].Split(':')[1].Trim()));
                //     var height = Mathf.RoundToInt(float.Parse(meta[i + 1].Split(':')[1].Trim()));
                //     Debug.LogError(dir.bot );
                //     if (position < 0)
                //     {
                //         if (-dir.bot > position)
                //         {
                //             dir.bot = -position;
                //         }
                //     }
                //     else if (position + height > mTexture.height)
                //     {
                //         if (-dir.top > mTexture.height - position - height)
                //         {
                //             dir.top = position + height - mTexture.height;
                //         }
                //     }
                // }

                var newWidth = dir.left + dir.right + mTexture.width;
                var newHeight = dir.top + dir.bot + mTexture.height;
                if (newWidth % 4 != 0)
                {
                    dir.right += 4 - newWidth % 4;
                }

                if (newHeight % 4 != 0)
                {
                    dir.top += 4 - newHeight % 4;
                }
            }

            if (dir == Direction.zero) return;
            for (var i = 0; i < meta.Length; i++)
            {
                if (meta[i].Contains("  isReadable: 0"))
                    meta[i] = "  isReadable: 1";
            //     if (!meta[i].Contains("width")) continue;
            //     if (!meta[i - 1].Contains("y:") || !meta[i - 2].Contains("x")) continue;
                 isEdit = true;
            //     var sizeSprite = int.Parse(meta[i - 1].Split(':')[1].Trim());
            //     meta[i - 1] = meta[i - 1].Replace(sizeSprite.ToString(), sizeSprite + dir.bot + "");
            //     sizeSprite = int.Parse(meta[i - 2].Split(':')[1].Trim());
            //     meta[i - 2] = meta[i - 2].Replace(sizeSprite.ToString(), sizeSprite + dir.left + "");
            }

            if (isEdit)
            {
                File.WriteAllLines(path + ".meta", meta);
                File.SetAttributes(path, attributes);
                AssetDatabase.Refresh();
                DrawableTexture.PenTexture(mTexture, dir);
            }
        }
    }

    [Serializable]
    public struct Direction
    {
        public Direction(int left, int right, int top, int bot)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bot = bot;
        }

        public int left, right, top, bot;
        public static Direction zero = new Direction {left = 0, right = 0, top = 0, bot = 0};

        public static bool operator ==(Direction lhs, Direction rhs)
        {
            return lhs.left == rhs.left && lhs.right == rhs.right && lhs.top == rhs.top && lhs.bot == rhs.bot;
        }

        public static bool operator !=(Direction lhs, Direction rhs)
        {
            return !(lhs == rhs);
        }
    }
}