using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace SimpleSpritePackerEditor
{
    public class DrawablePivot
    {
        static int Pen_Width = 8;

        public static void PenPivot(Sprite drawable_sprite)
        {
            var cur_colors = drawable_sprite.texture.GetPixels();
            MarkPixelsToColour(drawable_sprite, cur_colors);
            ApplyMarkedPixelChanges(drawable_sprite, cur_colors);
        }

        public static void MarkPixelsToColour(Sprite drawable_sprite, Color[] cur_colors)
        {
            var center_x = (int) drawable_sprite.pivot.x;
            var center_y = (int) drawable_sprite.pivot.y;
            var width = center_x > center_y ? center_y / 50 : center_x / 50;
            if (width < 4)
                width = 4;
            var rect = drawable_sprite.rect;
            center_x += (int) (rect.position.x);
            center_y += (int) (rect.position.y);
            for (var x = center_x - width; x < center_x + width; x++)
            {
                if (x >= (int) (rect.position.x + rect.width) || x < (rect.position.x))
                    continue;

                for (var y = center_y - width; y < center_y + width; y++)
                {
                    if (y >= (int) (rect.position.x + rect.height) || y < rect.position.y)
                        continue;
                    MarkPixelToChange(drawable_sprite, cur_colors, x, y, Color.white);
                }
            }

            width /= 2;
            for (int x = center_x - width; x < center_x + width; x++)
            {
                if (x >= (int) (rect.position.x + rect.width) || x < rect.position.x)
                    continue;

                for (int y = center_y - width; y < center_y + width; y++)
                {
                    if (y >= (int) (rect.position.x + rect.height) || y < rect.position.y)
                        continue;
                    MarkPixelToChange(drawable_sprite, cur_colors, x, y, Color.red);
                }
            }
        }

        public static void MarkPixelToChange(Sprite drawable_sprite, Color[] cur_colors, int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int) (drawable_sprite.rect.width + drawable_sprite.rect.position.x) + x;

            // Check if this is a valid position
            if (array_pos > cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;
        }

        public static void ApplyMarkedPixelChanges(Sprite drawable_sprite, Color[] cur_colors)
        {
            var texture = new Texture2D((int) drawable_sprite.rect.width, (int) drawable_sprite.rect.height, TextureFormat.ARGB32, false);
            texture.SetPixels(cur_colors);
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(AssetDatabase.GetAssetPath(drawable_sprite.texture), bytes);
        }
    }
}