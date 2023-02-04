using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    static class FindClosestMultipleOf4TextureSize
    {
        const string TOOLS = "Tools/";
        const string ASSET_TOOLS = "Assets/Tools/";

        const string PRINT_OPERATION = "Print closest multiple of 4 Texture size";
        const string PRINT_OPERATION_FULL = TOOLS + PRINT_OPERATION;
        const string PRINT_OPERATION_ASSETS_FULL = ASSET_TOOLS + PRINT_OPERATION;

        const string RESIZE_OPERATION = "Resize to closest multiple of 4 Texture size";
        const string RESIZE_OPERATION_FULL = TOOLS + RESIZE_OPERATION;
        const string RESIZE_OPERATION_ASSETS_FULL = ASSET_TOOLS + RESIZE_OPERATION;

        static bool TextureValidation()
        {
            return Selection.objects.Any() && Selection.objects.All(t => t.GetType() == typeof(Texture2D));
        }

        static IEnumerable<Texture2D> GetTextures()
        {
            return Selection.objects.Select(o => (Texture2D)o);
        }

        static Vector2Int GetClosestSize(Texture2D texture)
        {
            int closestWidth = Mathematics.ClosestMultipleOf4(texture.width);
            int closestHeight = Mathematics.ClosestMultipleOf4(texture.height);
            return new Vector2Int(closestWidth, closestHeight);
        }

        [MenuItem(PRINT_OPERATION_FULL, true)]
        [MenuItem(PRINT_OPERATION_ASSETS_FULL, true)]
        static bool PrintClosestSizeValidation()
        {
            return TextureValidation();
        }

        [MenuItem(PRINT_OPERATION_FULL)]
        [MenuItem(PRINT_OPERATION_ASSETS_FULL)]
        static void PrintClosestSize()
        {
            foreach (Texture2D texture in GetTextures())
            {
                Vector2Int closestSize = GetClosestSize(texture);
                Debug.Log($"{texture.name} - Closest Multiple of 4 Size: {closestSize}");
            }
        }

        [MenuItem(RESIZE_OPERATION_FULL, true)]
        [MenuItem(RESIZE_OPERATION_ASSETS_FULL, true)]
        static bool ResizeToClosestSizeValidation()
        {
            return TextureValidation();
        }

        [MenuItem(RESIZE_OPERATION_FULL)]
        [MenuItem(RESIZE_OPERATION_ASSETS_FULL)]
        static void ResizeToClosestSize()
        {
            foreach (Texture2D texture in GetTextures())
            {
                string imagePath = AssetDatabase.GetAssetPath(texture);

                string extention = Path.GetExtension(imagePath);

                if(!texture.IsSupported(extention))
                {
                    texture.ThrowNotSupportedException(extention);
                }

                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(imagePath);

                bool isOriginalyReadable = textureImporter.isReadable;

                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(imagePath);
                AssetDatabase.Refresh();

                Color[,] originColors = new Color[texture.width, texture.height];

                for (int x = 0; x < texture.width; ++x)
                {
                    for (int y = 0; y < texture.height; ++y)
                    {
                        originColors[x, y] = texture.GetPixel(x, y);
                    }
                }

                Vector2Int closestSize = GetClosestSize(texture);

                //IF should change Texture size.
                if (closestSize.x != texture.width || closestSize.y != texture.height)
                {
                    texture.Resize(closestSize.x, closestSize.y, texture.format, texture.mipmapCount > 1);

                    for (int x = 0; x < closestSize.x; ++x)
                    {
                        for (int y = 0; y < closestSize.y; ++y)
                        {
                            if (x < originColors.GetLength(0) && y < originColors.GetLength(1))
                            {
                                texture.SetPixel(x, y, originColors[x, y]);
                            }
                            else
                            {
                                texture.SetPixel(x, y, Color.clear);
                            }
                        }
                    }

                    texture.Apply();

                    File.WriteAllBytes(imagePath, texture.Encode(extention));
                }

                textureImporter.isReadable = isOriginalyReadable;
                AssetDatabase.ImportAsset(imagePath);
                AssetDatabase.Refresh();
            }
        }
    }
}