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
    bool IsAdvancedExpanded = false;

    public override void OnInspectorGUI()
    {
        IToyControl control = (IToyControl)target;
            
        Texture2D logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");

        GUILayout.Label(logo, GUILayout.Width(120), GUILayout.Height(60));
        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField("2023.02.9");
            EditorGUILayout.LinkButton("Source on GitHub");
        }

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
                EditorGUILayout.TextField("Left", "0", GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField("Right", "0", GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField("Top", "0", GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField("Bottom", "0", GUILayout.ExpandWidth(false));
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
                EditorGUILayout.Slider("Brightness", 0f, 0f, 100f);
                EditorGUILayout.Slider("Contrast", 0f, 0f, 100f);
                EditorGUILayout.Slider("Hue", 0f, 0f, 100f);
                EditorGUILayout.Slider("Saturation", 0f, 0f, 100f);
            }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        IsAdvancedExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsAdvancedExpanded, "Advanced");
            if (IsAdvancedExpanded)
            {
                GUILayout.Button("Reset", GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Self Destruct", GUILayout.ExpandWidth(false)))
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
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Button("Apply changes", GUILayout.ExpandWidth(false));
                GUILayout.Button("Discard changes", GUILayout.ExpandWidth(false));
            }
        }
    }
}