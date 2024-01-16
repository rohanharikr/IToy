using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.PackageManager.UI;

namespace IToy
{
    public class Context
    {
        [MenuItem("Assets/IToy/Flip Horizontal", true)]
        static bool ValidateFlipHorizontal()
        {
            //TBD
            return true;
        }

        [MenuItem("Assets/IToy/Flip Horizontal")]
        static void FlipHorizontal()
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

            //Delete original image (we save this in Toy SO)
            AssetDatabase.DeleteAsset(assetPath);

            //Write grayscale to .png file
            string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
            byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
            File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
            AssetDatabase.Refresh();
            Texture2D currentAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(grayscaleAssetPath);

            //Create Toy SO
            Toy toy = ScriptableObject.CreateInstance<Toy>();
            toy.Original = originalAsset;
            toy.Current = currentAsset;
            toy.Transform = new IToy.Transform();
            toy.Transform.FlipHorizontal = true;
            AssetDatabase.CreateAsset(toy, Path.Combine(assetDirPath, assetName + ".asset"));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = toy;
        }

        [MenuItem("Assets/IToy/Flip Vertical")]
        static void FlipVertical()
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
            Color32[] colors = originalImage.GetPixels32();
            Array.Reverse(colors);
            grayscaleImageBuffer.SetPixels32(colors);
            grayscaleImageBuffer.Apply();

            //OP done - Unset Read/Write access
            Utility.ReadWriteAccess(assetPath, false);

            //Delete original image (we save this in Toy SO)
            AssetDatabase.DeleteAsset(assetPath);

            //Write grayscale to .png file
            string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
            byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
            File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
            AssetDatabase.Refresh();
            Texture2D currentAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(grayscaleAssetPath);

            //Create Toy SO
            Toy toy = ScriptableObject.CreateInstance<Toy>();
            toy.Original = originalAsset;
            toy.Current = currentAsset;
            toy.Transform = new IToy.Transform();
            toy.Transform.FlipVertical = true;
            AssetDatabase.CreateAsset(toy, Path.Combine(assetDirPath, assetName + ".asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/IToy/Grayscale")]
        static void Grayscale()
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

            //Delete original image (we save this in Toy SO)
            AssetDatabase.DeleteAsset(assetPath);

            //Write grayscale to .png file
            string grayscaleAssetPath = Path.Combine(assetDirPath, assetName + ".png");
            byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
            File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
            AssetDatabase.Refresh();
            Texture2D currentAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(grayscaleAssetPath);

            //Create Toy SO
            Toy toy = ScriptableObject.CreateInstance<Toy>();
            toy.Original = originalAsset;
            toy.Current = currentAsset;
            toy.Correction = new IToy.Correction();
            toy.Correction.Saturation = -100;
            AssetDatabase.CreateAsset(toy, Path.Combine(assetDirPath, assetName + ".asset"));
            AssetDatabase.SaveAssets();
        }
    }
}

