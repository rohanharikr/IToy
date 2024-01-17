using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy
{
    public class Toy : ScriptableObject
    {
        //Store original image in SO since we do not want to show in inspector
        public byte[] Original;

        public string Current;

        public RemoveBackgroundOpts RemoveBackground;

        public Transform Transform;

        public Correction Correction;

        public Color RemoveBackgroundColor;
    }

    public enum RemoveBackgroundOpts
    {
        None,
        White,
        Black,
        Custom,
    }

    [Serializable]
    public class Transform
    {
        public bool FlipHorizontal = false;

        public bool FlipVertical = false;

        public RectInt Crop;
    }

    [Serializable]
    public class Correction
    {
        [Range(-100, 100)]
        public int Brightness = 0;

        [Range(-100, 100)]
        public int Contrast = 0;

        [Range(-100, 100)]
        public int Hue = 0;

        [Range(-100, 100)]
        public int Saturation = 0;
    }

    public class ToyUtility
    {
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
                    processor.Saturation(0);
                    break;
            }

            processor.WriteToDisk(AssetDatabase.GetAssetPath(selection));

            return toy;
        }

        public static Toy CreateOrUpdateToy(UnityEngine.Object selection, RemoveBackgroundOpts removeBackground)
        {
            Processor processor = new()
            {
                Texture = (Texture2D)selection
            };

            Toy toy = SetupCreateOrUpdate(selection);
            toy.RemoveBackground = removeBackground;

            Color colorToRemove = BackgroundEnumToColor(removeBackground, null);
            processor.RemoveBackground(colorToRemove);
            processor.WriteToDisk(AssetDatabase.GetAssetPath(selection));

            return toy;
        }

        public static Toy CreateOrUpdateToy(UnityEngine.Object selection) => SetupCreateOrUpdate(selection);

        static Toy SetupCreateOrUpdate(UnityEngine.Object selection)
        {
            string selectionPath = AssetDatabase.GetAssetPath(selection);
            string selectionName = Path.GetFileNameWithoutExtension(selectionPath);
            string selectionDirPath = Path.GetDirectoryName(selectionPath);
            string toyName = selectionName + ".asset";
            string toyAssetPath = Path.Combine(selectionDirPath, toyName);

            Toy toy = AssetDatabase.LoadAssetAtPath<Toy>(toyAssetPath);

            if (toy == null)
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
            string selectionName = Path.GetFileNameWithoutExtension(selectionPath);
            string selectionDirPath = Path.GetDirectoryName(selectionPath);
            string toyName = selectionName + ".asset";

            Toy toy = ScriptableObject.CreateInstance<Toy>();
            Utility.ReadWriteAccess(selectionPath, true); //Set read/write permission for texture to be read via script
            toy.Original = ((Texture2D)selection).EncodeToPNG();
            Utility.ReadWriteAccess(selectionPath, false); //OP done - revert permissions
            toy.Current = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selection));

            return toy;
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
    }
}
