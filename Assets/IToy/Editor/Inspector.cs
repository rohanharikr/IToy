﻿using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IToy
{
    [CustomEditor(typeof(IToyControl))]
    [CanEditMultipleObjects]
    public class ControlInspector : Editor
    {
        bool _isAdvancedExpanded = false;

        IToyControl control;
        Texture2D logo;
        Texture2D original;

        Texture2D preview;

        Material _flipHorizontalMat;
        Material _flipVerticalMat;
        Material _brightnessMat;
        Material _contrastMat;
        Material _saturationMat;

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            //TBD warn if unsaved changes
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Label(logo, GUILayout.Width(120), GUILayout.Height(60));

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Box(original, GUILayout.Width(212), GUILayout.Height(212));
                GUILayout.Box(preview, GUILayout.Width(212), GUILayout.Height(212));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoveBackground"));
            if (serializedObject.FindProperty("RemoveBackground").intValue == 3)
                EditorGUILayout.ColorField(" ", Color.white);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Transform"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Correction"));

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

            EditorGUILayout.Separator();

            if (GUILayout.Button("Save changes", GUILayout.ExpandWidth(false)))
            {
                byte[] file = preview.EncodeToPNG();
                File.WriteAllBytes("Assets/Resources/cat.png", file);
                AssetDatabase.Refresh();
            };

            if (serializedObject.ApplyModifiedProperties())
            {
                DrawPreview();
            }
        }

        void Init()
        {
            control = (IToyControl)target;
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");

            #region Init shaders
            Shader flipHorizontalShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/FlipHorizontal.shader");
            Shader flipVerticalShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/FlipVertical.shader");
            Shader brightnessShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Brightness.shader");
            Shader contrastShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Contrast.shader");
            Shader saturationShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Saturation.shader");
            _flipHorizontalMat = new Material(flipHorizontalShader);
            _flipVerticalMat = new Material(flipVerticalShader);
            _brightnessMat = new Material(brightnessShader);
            _contrastMat = new Material(contrastShader);
            _saturationMat = new Material(saturationShader);
            #endregion

            // Create a texture. Texture size does not matter, since
            // LoadImage will replace with the size of the incoming image.
            original = new Texture2D(0, 0);

            ImageConversion.LoadImage(original, control.Original);

            DrawPreview();
        }

        void DrawPreview()
        {
            preview = original;
            if(control.Transform.FlipHorizontal)
                preview = Utility.ApplyShader(preview, _flipHorizontalMat);
            if (control.Transform.FlipVertical)
                preview = Utility.ApplyShader(preview, _flipVerticalMat);

            int brightnessLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Brightness").intValue;
            if(brightnessLevel != 0)
            {
                _brightnessMat.SetInt("_Brightness", brightnessLevel);
                preview = Utility.ApplyShader(preview, _brightnessMat);
            }

            int contrastLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Contrast").intValue;
            if (contrastLevel != 0)
            {
                _contrastMat.SetInt("_Contrast", contrastLevel);
                preview = Utility.ApplyShader(preview, _contrastMat);
            }

            int saturationLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Saturation").intValue;
            if (saturationLevel != 0)
            {
                _saturationMat.SetInt("_Saturation", saturationLevel);
                preview = Utility.ApplyShader(preview, _saturationMat);
            }

            _brightnessMat.RevertAllPropertyOverrides();
            _saturationMat.RevertAllPropertyOverrides();
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
}