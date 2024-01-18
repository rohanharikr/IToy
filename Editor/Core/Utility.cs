using UnityEditor;
using UnityEngine;

namespace IToy.Core
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
    }
}
