using System;
using System.IO;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Control
{
    public string GUID;
}

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

        //Write grayscale to .png file
        string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
        byte[] grayScaleImage = grayscaleImageBuffer.EncodeToPNG();
        File.WriteAllBytes(grayscaleAssetPath, grayScaleImage);

        //Move original file to .tmp - use to reset back to original state in future
        string tmpAssetPath = Path.Combine(assetDirPath, assetNameWithExt + ".tmp");
        FileUtil.MoveFileOrDirectory(assetPath, tmpAssetPath);

        //Create control file
        Control control = new Control();
        control.GUID = AssetDatabase.AssetPathToGUID(grayscaleAssetPath); //GUID of IToy-generated image
        string jsonStr = JsonUtility.ToJson(control);
        string controlAssetPath = Path.Combine(assetDirPath + "/" + assetName + ".control");
        File.WriteAllText(controlAssetPath, jsonStr);

        AssetDatabase.Refresh();
    }
}

