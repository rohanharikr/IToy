using System.IO;
using UnityEngine;
using UnityEditor;
using System;

public class Context
{
    [MenuItem("Assets/IToy/Flip horizontal")]
    public static void FlipHorizontal()
    {
        string assetGuid = Selection.assetGUIDs[0];
        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetDirPath = Path.GetDirectoryName(assetPath);

        //Set Read/Write access for GetPixel OP
        Utility.ReadWriteAccess(assetPath, true);

        //Grayscale OP
        Texture2D originalImage = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        byte[] originalAsset = originalImage.EncodeToJPG();
        Texture2D grayscaleImageBuffer = new(originalImage.width, originalImage.height);
        Color[] colors = originalImage.GetPixels();
        for (int y = 0; y < originalImage.height; y++)
            for (int x = 0; x < originalImage.width; x++)
            {
                int index = y * originalImage.width + (originalImage.width - 1 - x);
                grayscaleImageBuffer.SetPixel(x, y, colors[index]);
            }
        grayscaleImageBuffer.Apply();

        //OP done - Unset Read/Write access
        Utility.ReadWriteAccess(assetPath, false);

        //Delete original image (we save this in IToyControl SO)
        AssetDatabase.DeleteAsset(assetPath);

        //Write grayscale to .png file
        string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
        byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
        File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
        AssetDatabase.Refresh();
        Texture2D currentAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(grayscaleAssetPath);

        //Create IToyControl SO
        IToyControl control = ScriptableObject.CreateInstance<IToyControl>();
        control.Original = originalAsset;
        control.Current = currentAsset;
        control.FlipHorizontal = true;
        AssetDatabase.CreateAsset(control, Path.Combine(assetDirPath, assetName + ".asset"));
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/IToy/Flip vertical")]
    public static void FlipVertical()
    {
        string assetGuid = Selection.assetGUIDs[0];
        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetDirPath = Path.GetDirectoryName(assetPath);

        //Set Read/Write access for GetPixel OP
        Utility.ReadWriteAccess(assetPath, true);

        //Grayscale OP
        Texture2D originalImage = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        byte[] originalAsset = originalImage.EncodeToJPG();
        Texture2D grayscaleImageBuffer = new(originalImage.width, originalImage.height);
        Color[] colors = originalImage.GetPixels();
        Array.Reverse(colors);
        grayscaleImageBuffer.SetPixels(colors);
        grayscaleImageBuffer.Apply();

        //OP done - Unset Read/Write access
        Utility.ReadWriteAccess(assetPath, false);

        //Delete original image (we save this in IToyControl SO)
        AssetDatabase.DeleteAsset(assetPath);

        //Write grayscale to .png file
        string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
        byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
        File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
        AssetDatabase.Refresh();
        Texture2D currentAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(grayscaleAssetPath);

        //Create IToyControl SO
        IToyControl control = ScriptableObject.CreateInstance<IToyControl>();
        control.Original = originalAsset;
        control.Current = currentAsset;
        control.FlipVertical = true;
        AssetDatabase.CreateAsset(control, Path.Combine(assetDirPath, assetName + ".asset"));
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/IToy/Grayscale")]
    public static void Grayscale()
    {
        string assetGuid = Selection.assetGUIDs[0];
        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetDirPath = Path.GetDirectoryName(assetPath);

        //Set Read/Write access for GetPixel OP
        Utility.ReadWriteAccess(assetPath, true);

        //Grayscale OP
        Texture2D originalImage = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        byte[] originalAsset = originalImage.EncodeToJPG();
        Texture2D grayscaleImageBuffer = new(originalImage.width, originalImage.height);
        for (int i = 0; i < originalImage.width; i++)
            for (int j = 0; j < originalImage.height; j++)
            {
                Color pixel = originalImage.GetPixel(i, j);
                float grayScalePixel = pixel.grayscale;
                Color pixelColor = new(grayScalePixel, grayScalePixel, grayScalePixel);
                grayscaleImageBuffer.SetPixel(i, j, pixelColor);
            }
        grayscaleImageBuffer.Apply();

        //OP done - Unset Read/Write access
        Utility.ReadWriteAccess(assetPath, false);

        //Delete original image (we save this in IToyControl SO)
        AssetDatabase.DeleteAsset(assetPath);
        
        //Write grayscale to .png file
        string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
        byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
        File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
        AssetDatabase.Refresh();
        Texture2D currentAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(grayscaleAssetPath);
        
        //Create IToyControl SO
        IToyControl control = ScriptableObject.CreateInstance<IToyControl>();
        control.Original = originalAsset;
        control.Current = currentAsset;
        AssetDatabase.CreateAsset(control, Path.Combine(assetDirPath, assetName + ".asset"));
        AssetDatabase.SaveAssets();
    }
}

