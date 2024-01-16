using System.IO;
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
        Material _cropMat;

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
            Shader cropShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Crop.shader");
            _flipHorizontalMat = new Material(flipHorizontalShader);
            _flipVerticalMat = new Material(flipVerticalShader);
            _cropMat = new Material(cropShader);
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

            int saturationLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Saturation").intValue;
            if(saturationLevel != 0)
            {
                _cropMat.SetInt("_Saturation", saturationLevel);
                preview = Utility.ApplyShader(preview, _cropMat);
            }

            //byte[] file = preview.EncodeToPNG();
            //File.WriteAllBytes("Assets/Resources/cat.png", file);
            //AssetDatabase.Refresh();
            _cropMat.RevertAllPropertyOverrides();
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