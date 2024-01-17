using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy
{
    public class Utility
    {
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

        public static Toy CreateOrUpdateToy(Object selection, string key, object value)
        {
            Toy toy = SetupCreateOrUpdate(selection);

            switch (key)
            {
                case "FlipHorizontal":
                    toy.Transform.FlipHorizontal = (bool)value;
                    break;
                case "FlipVertical":
                    toy.Transform.FlipVertical = (bool)value;
                    break;
                case "Grayscale":
                    toy.Correction.Saturation = (int)value;
                    break;
            }

            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = toy;

            return toy;
        }

        public static Toy CreateOrUpdateToy(Object selection, RemoveBackgroundOpts removeBackground)
        {
            Toy toy = SetupCreateOrUpdate(selection);

            toy.RemoveBackground = removeBackground;

            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = toy;

            return toy;
        }

        private static Toy SetupCreateOrUpdate(Object selection)
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
                ReadWriteAccess(selectionPath, true); //Set read/write permission for texture to be read va script
                toy.Original = ((Texture2D)selection).EncodeToPNG();
                ReadWriteAccess(selectionPath, false); //OP done - revert permissions
                toy.Current = AssetDatabase.GetAssetPath(selection);
                AssetDatabase.CreateAsset(toy, toyAssetPath);
                AssetDatabase.SaveAssets();
            }

            return toy;
        }
    }
}
