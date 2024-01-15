using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IToyControl))]
[CanEditMultipleObjects]
public class ControlInspector : Editor
{
    #region Expandables
    bool _isBgRemovalExpanded = true;
    bool _isTransformExpanded = true;
    bool _isCorrectionExpanded = true;
    bool _isAdvancedExpanded = false;
    #endregion

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        IToyControl control = (IToyControl)target;

        Texture2D logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");

        GUILayout.Label(logo, GUILayout.Width(120), GUILayout.Height(60));

        // Texture size does not matter, since
        // LoadImage will replace with the size of the incoming image.
        Texture2D original = new Texture2D(0, 0);
        Texture2D current = control.Current;
        using (new EditorGUILayout.HorizontalScope())
        {
            ImageConversion.LoadImage(original, control.Original);
            GUILayout.Box(original, GUILayout.Width(212), GUILayout.Height(212));
            GUILayout.Box(current, GUILayout.Width(212), GUILayout.Height(212));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoveBackground"));
        if (serializedObject.FindProperty("RemoveBackground").intValue == 0)
            EditorGUILayout.ColorField(" ", Color.white);

        EditorGUILayout.Separator();

        _isTransformExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isTransformExpanded, "Transform");
        if (_isTransformExpanded)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Crop"));
            EditorGUILayout.Toggle("Flip horizontal", false);
            EditorGUILayout.Toggle("Flip vertical", false);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        _isCorrectionExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isCorrectionExpanded, "Correction");
            if(_isCorrectionExpanded)
            {
                EditorGUILayout.Slider("Brightness", 0f, 0f, 100f);
                EditorGUILayout.Slider("Contrast", 0f, 0f, 100f);
                EditorGUILayout.Slider("Hue", 0f, 0f, 100f);
                EditorGUILayout.Slider("Saturation", 0f, 0f, 100f);
            }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        _isAdvancedExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isAdvancedExpanded, "Advanced");
        if (_isAdvancedExpanded)
        {
            using (new GUILayout.HorizontalScope())
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
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}