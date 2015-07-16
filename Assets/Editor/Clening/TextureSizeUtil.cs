using UnityEngine;
using System.Collections;
using UnityEditor;

// Feel free to pick your own class name:
using System.Reflection;


public class TextureSizeUtil 
{
    public static bool GetNativeImageSize(Texture2D asset, out int width, out int height) {
        if (asset != null) {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            
            if (importer != null) {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
                
                width = (int)args[0];
                height = (int)args[1];
                
                return true;
            }
        }
        
        height = width = 0;
        return false;
    }

    public static bool GetNativeImageSize(string assetPath, out int width, out int height) {
        //if (asset != null) 
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            
            if (importer != null) {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
                
                width = (int)args[0];
                height = (int)args[1];
                
                return true;
            }
        }
        
        height = width = 0;
        return false;
    }

    public static bool GetCompressedImageSize(string assetPath, out int width, out int height) 
    {
        Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
        width = t.width;
        height = t.height;

        return false;
    }
}
