using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IToyControl))]
[CanEditMultipleObjects]
public class ControlInspector : Editor
{
    #region Expandables
    bool _isCorrectionExpanded = true;
    bool _isAdvancedExpanded = false;
    #endregion

    IToyControl control;
    Texture2D logo;
    Texture2D original;
    Texture2D current;

    private void OnEnable()
    {
        control = (IToyControl)target;
        logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");
        current = control.Current;

        // Create a texture. Texture size does not matter, since
        // LoadImage will replace with the size of the incoming image.
        original = new Texture2D(0, 0);

        ImageConversion.LoadImage(original, control.Original);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label(logo, GUILayout.Width(120), GUILayout.Height(60));

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Box(original, GUILayout.Width(212), GUILayout.Height(212));
            GUILayout.Box(current, GUILayout.Width(212), GUILayout.Height(212));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoveBackground"));
        if (serializedObject.FindProperty("RemoveBackground").intValue == 3)
            EditorGUILayout.ColorField(" ", Color.white);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Transform"));

            EditorGUILayout.Separator();

        _isCorrectionExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isCorrectionExpanded, "Correction");
        if(_isCorrectionExpanded)
        {
            EditorGUILayout.Slider("Brightness", 0f, 0f, 100f);
            EditorGUILayout.Slider("Contrast", 0f, 0f, 100f);
            EditorGUILayout.Slider("Hue", 0f, 0f, 100f);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Saturation"));
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
                    SelfDestruct(control);
                };
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (serializedObject.ApplyModifiedProperties())
        {
            Recompute();
        }
    }

    void Recompute()
    {
        //Grayscale OP
        Texture2D grayscaleImageBuffer = new(original.width, original.height);
        Color32[] colors = original.GetPixels32();
        for (int i = 0; i < original.width; i++)
            for (int j = 0; j < original.height; j++)
            {
                Color pixel = original.GetPixel(i, j);
                float grayScalePixel = pixel.grayscale;
                Color pixelColor = new(grayScalePixel, grayScalePixel, grayScalePixel);
                grayscaleImageBuffer.SetPixel(i, j, pixelColor);
            }
        grayscaleImageBuffer.SetPixels32(colors);
        grayscaleImageBuffer.Apply();

        string grayscaleAssetPath = AssetDatabase.GetAssetPath(control.Current);
        byte[] grayscaleImage = grayscaleImageBuffer.EncodeToPNG();
        File.WriteAllBytes(grayscaleAssetPath, grayscaleImage);
        AssetDatabase.Refresh();
    }

    void SelfDestruct(IToyControl control)
    {
        string currentAssetPath = AssetDatabase.GetAssetPath(control.Current);
        string currentAssetDirPath = Path.GetDirectoryName(currentAssetPath);
        AssetDatabase.DeleteAsset(currentAssetPath);
        File.WriteAllBytes(Path.Combine(currentAssetDirPath, "cat.png"), control.Original);
        string controlPath = AssetDatabase.GetAssetPath(control);
        AssetDatabase.DeleteAsset(controlPath);
        AssetDatabase.Refresh();
    }
}