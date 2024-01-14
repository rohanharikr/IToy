using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IToyControl))]
public class ControlInspector : Editor
{
    public override void OnInspectorGUI()
    {
        IToyControl control = (IToyControl)target;

        // Create a texture. Texture size does not matter, since
        // LoadImage will replace with the size of the incoming image.
        Texture2D imageOne = new Texture2D(2, 2);

        using (new EditorGUILayout.HorizontalScope())
        {
            ImageConversion.LoadImage(imageOne, control.Original);
            GUILayout.Box(imageOne, GUILayout.Width(212), GUILayout.Height(212));
            Texture2D imageTwo = control.Current;
            GUILayout.Box(imageTwo, GUILayout.Width(212), GUILayout.Height(212));
        }

        EditorGUILayout.ColorField("Remove", Color.gray);
        EditorGUILayout.Slider("Brightness", 50f, 0f, 100f);
        EditorGUILayout.Slider("Contrast", 50f, 0f, 100f);
        EditorGUILayout.Slider("Saturation", 50f, 0f, 100f);

        GUILayout.Button("Reset");
        
        if(GUILayout.Button("Self Destruct"))
        {
            string currentAssetPath = AssetDatabase.GetAssetPath(control.Current);
            string currentAssetDirPath = Path.GetDirectoryName(currentAssetPath);
            AssetDatabase.DeleteAsset(currentAssetPath);
            File.WriteAllBytes(Path.Combine(currentAssetDirPath, "cat.png"), control.Original);
            string controlPath = AssetDatabase.GetAssetPath(control);
            AssetDatabase.DeleteAsset(controlPath);
            AssetDatabase.Refresh();
        };
    }
}