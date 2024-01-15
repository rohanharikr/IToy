using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(IToyControl))]
public class ControlInspector : Editor
{
    bool IsBgRemovalExpanded = true;
    bool IsCropExpanded = true;
    bool IsTransformExpanded = true;
    bool IsCorrectionExpanded = true;

    public override void OnInspectorGUI()
    {
        IToyControl control = (IToyControl)target;

        // Create a texture. Texture size does not matter, since
        // LoadImage will replace with the size of the incoming image.
        Texture2D imageOne = new Texture2D(0, 0);

        using (new EditorGUILayout.HorizontalScope())
        {
            ImageConversion.LoadImage(imageOne, control.Original);
            GUILayout.Box(imageOne, GUILayout.Width(212), GUILayout.Height(212));
            Texture2D imageTwo = control.Current;
            GUILayout.Box(imageTwo, GUILayout.Width(212), GUILayout.Height(212));
        }

        IsBgRemovalExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsBgRemovalExpanded, "Remove background");
            if (IsBgRemovalExpanded)
            {
                EditorGUILayout.ToggleLeft("White", false);
                EditorGUILayout.ToggleLeft("Black", false);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.ToggleLeft("Custom", false);
                    EditorGUILayout.ColorField(Color.white);

                }
            }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        IsCropExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsCropExpanded, "Crop");
            if (IsCropExpanded)
            {
                EditorGUILayout.TextField("Left", "", GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField("Right", "", GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField("Top", "", GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField("Bottom", "", GUILayout.ExpandWidth(false));
            }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        IsTransformExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsTransformExpanded, "Transform");
            if (IsTransformExpanded)
            {
                EditorGUILayout.ToggleLeft("Flip horizontal", false);
                EditorGUILayout.ToggleLeft("Flip vertical", false);
            }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        IsCorrectionExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsCorrectionExpanded, "Correction");
            if(IsCorrectionExpanded)
            {
                EditorGUILayout.ColorField("Remove", Color.gray);
                EditorGUILayout.Slider("Brightness", 50f, 0f, 100f);
                EditorGUILayout.Slider("Contrast", 50f, 0f, 100f);
                EditorGUILayout.Slider("Saturation", 50f, 0f, 100f);
            }
        EditorGUILayout.EndFoldoutHeaderGroup();

        //If anyone has a better idea im all ears
        GUILayout.Label("_________________________________________________________________________________________________________________________________________________________________________");
        EditorGUILayout.Separator();

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Button("Save changes", GUILayout.ExpandWidth(false));
            GUILayout.Button("Reset", GUILayout.ExpandWidth(false));
            if(GUILayout.Button("Self Destruct", GUILayout.ExpandWidth(false)))
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
}