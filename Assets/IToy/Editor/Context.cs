using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Init : Editor
{
    [Serializable]
    public class Control
    {
        public Vector3 Saturation;
        public string GUID;

    }

    [MenuItem("Assets/IToy/Grayscale")]
    public static void Grayscale()
    {
        string assetGuid = Selection.assetGUIDs[0];
        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string assetNameWithExt = Path.GetFileName(assetPath);
        string assetDirPath = Path.GetDirectoryName(assetPath);

        Control control = new()
        {
            GUID = assetGuid
        };
        
        string json = JsonUtility.ToJson(control);

        File.Copy(assetPath, Path.Combine(assetDirPath + "/" + assetNameWithExt + ".tmp"), true);
        File.WriteAllText(Path.Combine(assetDirPath, assetName + ".control"), json);

        //Set Read/Write access for GetPixel
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        if(!ti.isReadable)
        {
            ti.isReadable = true;
            ti.SaveAndReimport();
        }

        Texture2D originalImage = Resources.Load<Texture2D>(assetName);
        Texture2D grayscaleImageBuffer = new(originalImage.width, originalImage.height);
        for (int i = 0; i < originalImage.width; i++)
            for (int j = 0; j < originalImage.height; j++)
            {
                Color pixel = originalImage.GetPixel(i, j);
                float grayScalePixel = (pixel.r / 3) + (pixel.g / 3) + (pixel.b / 3); //or use pixel.grayscale?
                Color pixelColor = new(grayScalePixel, grayScalePixel, grayScalePixel);
                grayscaleImageBuffer.SetPixel(i, j, pixelColor);
            }

        grayscaleImageBuffer.Apply();
        var grayScaleImage = grayscaleImageBuffer.EncodeToPNG();

        File.WriteAllBytes(assetDirPath + "/" + "testing.png", grayScaleImage);

        //OP done - Unset Read/Write access
        ti.isReadable = false;
        ti.SaveAndReimport();

        AssetDatabase.Refresh();
    }

    [InitializeOnLoad]
    public class IniFileGlobal
    {
        private static IniFileWrapper wrapper = null;
        private static bool selectionChanged = false;

        static IniFileGlobal()
        {
            Selection.selectionChanged += SelectionChanged;
            EditorApplication.update += Update;
        }

        private static void SelectionChanged()
        {
            selectionChanged = true;
        }

        private static void Update()
        {
            if (selectionChanged == false) return;

            selectionChanged = false;
            if (Selection.activeObject != wrapper)
            {
                string fn = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                if (fn.ToLower().EndsWith(".control"))
                {
                    if (wrapper == null)
                    {
                        wrapper = ScriptableObject.CreateInstance<IniFileWrapper>();
                        wrapper.hideFlags = HideFlags.DontSave;
                    }

                    wrapper.fileName = fn;
                    Selection.activeObject = wrapper;

                    Editor[] ed = Resources.FindObjectsOfTypeAll<IniFileWrapperInspector>();
                    if (ed.Length > 0) ed[0].Repaint();
                }
            }
        }
    }

    public class IniFileWrapper : ScriptableObject
    {
        [System.NonSerialized] public string fileName;
    }

    [CustomEditor(typeof(IniFileWrapper))]
    public class IniFileWrapperInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            IniFileWrapper Target = (IniFileWrapper)target;

            string jsonFileName = Path.GetFileName(Target.fileName);

            if (GUILayout.Button("Self destruct 切腹"))
            {
                using (var sr = new StreamReader(Target.fileName))
                {
                    string jsonStringified = sr.ReadToEnd();
                    Control json = JsonUtility.FromJson<Control>(jsonStringified);
                    string resPath = AssetDatabase.GUIDToAssetPath(json.GUID);
                    File.Delete(resPath);

                    string assetDirPath = Path.GetDirectoryName(Target.fileName);
                    string fileName = Path.GetFileNameWithoutExtension(Target.fileName);
                    File.Move(assetDirPath + "/" + fileName + ".jpg.tmp", fileName + ".jpg");
                    
                    //Seppuku
                    //File.Delete(Target.fileName);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
