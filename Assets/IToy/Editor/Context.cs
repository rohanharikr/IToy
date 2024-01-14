using System.IO;
using UnityEngine;
using UnityEditor;

public class Context
{
    [MenuItem("Assets/IToy/Grayscale")]
    public static void Grayscale()
    {
        string assetGuid = Selection.assetGUIDs[0];
        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetNameWithExt = Path.GetFileName(assetPath);
        string assetDirPath = Path.GetDirectoryName(assetPath);

        //Set Read/Write access for GetPixel OP
        Utility.ReadWriteAccess(assetPath, true);

        //Grayscale OP
        Texture2D originalImage = Resources.Load<Texture2D>(assetName);
        Texture2D grayscaleImageBuffer = new(originalImage.width, originalImage.height);
        for (int i = 0; i < originalImage.width; i++)
            for (int j = 0; j < originalImage.height; j++)
            {
                Color pixel = originalImage.GetPixel(i, j);
                float grayScalePixel = (pixel.r / 3) + (pixel.g / 3) + (pixel.b / 3); //Or use (Unity)pixel.grayscale?
                Color pixelColor = new(grayScalePixel, grayScalePixel, grayScalePixel);
                grayscaleImageBuffer.SetPixel(i, j, pixelColor);
            }
        grayscaleImageBuffer.Apply();

        //OP done - Unset Read/Write access
        Utility.ReadWriteAccess(assetPath, false);

        //Move original file to temp (for restore option)
        string tmpAssetsDir = Path.Combine(assetDirPath, ".IToy");
        string tmpAssetPath = Path.Combine(tmpAssetsDir, assetNameWithExt);
        if(!Directory.Exists(tmpAssetsDir))
        {
            Directory.CreateDirectory(tmpAssetsDir);
        }
        FileUtil.MoveFileOrDirectory(assetPath, tmpAssetPath);
        Texture2D tmpAsset = Resources.Load<Texture2D>(tmpAssetPath);

        //Write grayscale to .png file
        string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
        byte[] grayScaleImage = grayscaleImageBuffer.EncodeToPNG();
        File.WriteAllBytes(grayscaleAssetPath, grayScaleImage);

        //Create IToyControl SO
        IToyControl control = ScriptableObject.CreateInstance<IToyControl>();
        control.Original = tmpAsset;
        control.Current = grayscaleImageBuffer;
        AssetDatabase.CreateAsset(control, Path.Combine(assetDirPath, assetName + ".asset"));
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
    }
}

