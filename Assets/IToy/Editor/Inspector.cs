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

    public Material flipHorizontalMaterial;

    private void OnEnable()
    {
        control = (IToyControl)target;
        logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");
        current = control.Current;
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/FlipHorizontal.shader");
        flipHorizontalMaterial = new Material(shader);

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
            Texture2D flippedTexture = ApplyShader(current, flipHorizontalMaterial);
            byte[] file = flippedTexture.EncodeToPNG();
            File.WriteAllBytes("Assets/Resources/cat.png", file);
            AssetDatabase.Refresh();
        }
    }

    Texture2D ApplyShader(Texture2D inputTexture, Material shaderMaterial)
    {
        // Create a temporary RenderTexture
        RenderTexture rt = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        // Create a temporary Texture2D
        Texture2D outputTexture = new Texture2D(inputTexture.width, inputTexture.height);

        // Set the RenderTexture as the active render target
        Graphics.SetRenderTarget(rt);

        // Clear the render target
        GL.Clear(true, true, Color.clear);

        // Set the material with the shader
        Graphics.Blit(inputTexture, rt, shaderMaterial);

        // Read the RenderTexture data into the Texture2D
        RenderTexture.active = rt;
        outputTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        // Release the temporary objects
        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.active = null;

        return outputTexture;
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