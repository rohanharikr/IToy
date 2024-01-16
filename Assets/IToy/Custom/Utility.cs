using UnityEditor;
using UnityEngine;

namespace IToy
{
    public class Utility
    {
        public static void ReadWriteAccess(string assetPath, bool set = true)
        {
            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            if (set)
                ti.isReadable = true;
            else
                ti.isReadable = true;

            ti.SaveAndReimport();
        }

        public static Texture2D ApplyShader(Texture2D inputTexture, Material shaderMaterial)
        {
            Texture2D res = new Texture2D(inputTexture.width, inputTexture.height);
            RenderTexture rt = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height);
            Graphics.Blit(inputTexture, rt, shaderMaterial);
            RenderTexture.active = rt;
            res.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            res.Apply();
            return res;
        }
    }
}
