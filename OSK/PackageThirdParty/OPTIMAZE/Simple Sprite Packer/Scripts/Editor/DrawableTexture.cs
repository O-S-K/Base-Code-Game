using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace SimpleSpritePackerEditor
{
    public class DrawableTexture
    {
        public static void PenTexture(Texture2D mTexture, Direction dir)
        {
            var newTexture = new Texture2D(mTexture.width + dir.left + dir.right, mTexture.height + dir.top + dir.bot);
            var curColors = mTexture.GetPixels();
            var newColors = MarkPixelsToColour(newTexture, curColors, dir);
            ApplyMarkedPixelChanges(newTexture, newColors, AssetDatabase.GetAssetPath(mTexture));
        }

        public static Color[] MarkPixelsToColour(Texture2D newTexture, Color[] curColors, Direction dir)
        {
            var newColors = new Color[newTexture.width * newTexture.height];
            var xMax = newTexture.width - dir.right;
            var yMax = newTexture.height - dir.top;
            var xMin = dir.left;
            var yMin = dir.bot;
            for (var i = 0; i < newColors.Length; i++)
            {
                newColors[i] = Color.clear;
            }

            for (var x = xMin; x < xMax; x++)
            {
                for (var y = yMin; y < yMax; y++)
                {
                    var curArrPos = MarkPixelToChange(xMax - xMin, x - xMin, y - yMin);
                    var newArrPos = MarkPixelToChange(newTexture.width, x, y);
                    newColors[newArrPos] = curColors[curArrPos];
                }
            }

            return newColors;
        }

        public static int MarkPixelToChange(int width, int x, int y)
        {
            int array_pos = y * width + x;
            return array_pos;
        }

        public static void ApplyMarkedPixelChanges(Texture2D newTexture, Color[] newColors, string path)
        {
            newTexture.SetPixels(newColors);
            newTexture.Apply();
            var bytes = newTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            SPTools.TextureSetReadWriteEnabled(newTexture, false, false);
        }
    }
}