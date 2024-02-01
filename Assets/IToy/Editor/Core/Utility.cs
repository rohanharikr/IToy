using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy.Core
{
    public static class Utility
    {
        public static string PackageName = "com.rohanharikr.itoy";

        public static void ReadWriteAccess(string assetPath, bool set = true)
        {
            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            ti.isReadable = set;

            ti.SaveAndReimport();
        }

        public static Texture2D ApplyShader(Texture2D inputTexture, Material shaderMaterial)
        {
            Texture2D res = new(inputTexture.width, inputTexture.height);
            RenderTexture rt = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height);
            Graphics.Blit(inputTexture, rt, shaderMaterial);
            RenderTexture.active = rt;
            res.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            res.Apply();
            return res;
        }

        public static bool IsSupportedFileType(Object selection)
        {
            return selection != null && selection is Texture2D;
        }

        public static Color BackgroundEnumToColor(RemoveBackgroundOpts removeBackgroundEnum, Color? customColor)
        {
            if (removeBackgroundEnum == RemoveBackgroundOpts.White)
                return Color.white;
            else if (removeBackgroundEnum == RemoveBackgroundOpts.Black)
                return Color.black;
            else if (customColor != null)
                return (Color)customColor;

            return new Color(0, 0, 0, 0);
        }

        public static Toy CreateOrUpdateToy(UnityEngine.Object selection) => SetupCreateOrUpdate(selection);
        public static Toy CreateOrUpdateToy(UnityEngine.Object selection, RemoveBackgroundOpts removeBackground)
        {
            Processor processor = new()
            {
                Texture = (Texture2D)selection
            };

            Toy toy = SetupCreateOrUpdate(selection);
            toy.RemoveBackground = removeBackground;

            Color colorToRemove = Utility.BackgroundEnumToColor(removeBackground, null);
            processor.RemoveBackground(colorToRemove);
            processor.WriteToDisk(AssetDatabase.GetAssetPath(selection));

            return toy;
        }
        public static Toy CreateOrUpdateToy(UnityEngine.Object selection, string key, object value)
        {
            Processor processor = new()
            {
                Texture = (Texture2D)selection
            };

            Toy toy = SetupCreateOrUpdate(selection);

            switch (key)
            {
                case "FlipHorizontal":
                    toy.Transform.FlipHorizontal = (bool)value;
                    processor.FlipHorizontal();
                    break;
                case "FlipVertical":
                    toy.Transform.FlipVertical = (bool)value;
                    processor.FlipVertical();
                    break;
                case "Grayscale":
                    toy.Correction.Saturation = (int)value;
                    processor.Saturation((int)value);
                    break;
            }

            processor.WriteToDisk(AssetDatabase.GetAssetPath(selection));

            return toy;
        }

        private static Toy SetupCreateOrUpdate(UnityEngine.Object selection)
        {
            string selectionPath = AssetDatabase.GetAssetPath(selection);
            string selectionName = Path.GetFileNameWithoutExtension(selectionPath);
            string selectionDirPath = Path.GetDirectoryName(selectionPath);
            string toyName = selectionName + ".asset";
            string toyAssetPath = Path.Combine(selectionDirPath, toyName);

            Toy toy = AssetDatabase.LoadAssetAtPath<Toy>(toyAssetPath);

            if (toy == null) //Toy does not exist, create one
            {
                toy = ScriptableObject.CreateInstance<Toy>();
                Utility.ReadWriteAccess(selectionPath, true); //Set read/write permission for texture to be read va script
                toy.Original = ((Texture2D)selection).EncodeToPNG();
                Utility.ReadWriteAccess(selectionPath, false); //OP done - revert permissions
                toy.Current = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selection));
                AssetDatabase.CreateAsset(toy, toyAssetPath);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = toy;

            return toy;
        }

        public static Toy GenerateToy(UnityEngine.Object selection)
        {
            string selectionPath = AssetDatabase.GetAssetPath(selection);

            Toy toy = ScriptableObject.CreateInstance<Toy>();
            Utility.ReadWriteAccess(selectionPath, true); //Set read/write permission for texture to be read via script
            toy.Original = ((Texture2D)selection).EncodeToPNG();
            Utility.ReadWriteAccess(selectionPath, false); //OP done - revert permissions
            toy.Current = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selection));

            return toy;
        }
    }
}
