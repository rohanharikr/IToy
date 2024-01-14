using UnityEditor;

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
}
