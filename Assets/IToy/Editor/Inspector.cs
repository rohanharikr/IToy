using System.IO;
using UnityEditor;
using UnityEngine;

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

        #region Materials
        Material _backgroundMat;
        Material _flipHorizontalMat;
        Material _flipVerticalMat;
        Material _cropMat;
        Material _brightnessMat;
        Material _contrastMat;
        Material _hueMat;
        Material _saturationMat;
        #endregion

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
            if (serializedObject.FindProperty("RemoveBackground").intValue == (int)RemoveBackgroundOpts.Custom)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoveBackgroundColor"), new GUIContent(" "));

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
            Shader backgroundShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Background.shader");
            Shader flipHorizontalShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Transform/FlipHorizontal.shader");
            Shader flipVerticalShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Transform/FlipVertical.shader");
            Shader cropShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Transform/Crop.shader");
            Shader brightnessShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Brightness.shader");
            Shader contrastShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Contrast.shader");
            Shader hueShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Hue.shader");
            Shader saturationShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/Correction/Saturation.shader");
            _backgroundMat = new Material(backgroundShader);
            _flipHorizontalMat = new Material(flipHorizontalShader);
            _flipVerticalMat = new Material(flipVerticalShader);
            _cropMat = new Material(cropShader);
            _brightnessMat = new Material(brightnessShader);
            _contrastMat = new Material(contrastShader);
            _hueMat = new Material(hueShader);
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

            RectInt cropValues = serializedObject.FindProperty("Transform").FindPropertyRelative("Crop").rectIntValue;
            if (cropValues.x != 0 || cropValues.y != 0 || cropValues.width != 0 || cropValues.height != 0)
            {
                _cropMat.SetInteger("_Top", cropValues.x);
                _cropMat.SetInteger("_Right", cropValues.y);
                _cropMat.SetInteger("_Bottom", cropValues.width);
                _cropMat.SetInteger("_Left", cropValues.height);
               preview = Utility.ApplyShader(preview, _cropMat);
            }

            int removeBackground = serializedObject.FindProperty("RemoveBackground").enumValueIndex;
            if (removeBackground != (int)RemoveBackgroundOpts.None)
            {
                Color colorToRemove;
                if(removeBackground == (int)RemoveBackgroundOpts.White)
                    colorToRemove = Color.white;
                else if(removeBackground == (int)RemoveBackgroundOpts.Black)
                    colorToRemove = Color.black;
                else
                    colorToRemove = serializedObject.FindProperty("RemoveBackgroundColor").colorValue;

                _backgroundMat.SetColor("_RemoveColor", colorToRemove);
                preview = Utility.ApplyShader(preview, _backgroundMat);
            }

            if (control.Transform.FlipHorizontal)
                preview = Utility.ApplyShader(preview, _flipHorizontalMat);

            if (control.Transform.FlipVertical)
                preview = Utility.ApplyShader(preview, _flipVerticalMat);

            int brightnessLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Brightness").intValue;
            if(brightnessLevel != 0)
            {
                _brightnessMat.SetInteger("_Brightness", brightnessLevel);
                preview = Utility.ApplyShader(preview, _brightnessMat);
            }

            int contrastLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Contrast").intValue;
            if (contrastLevel != 0)
            {
                _contrastMat.SetInteger("_Contrast", contrastLevel);
                preview = Utility.ApplyShader(preview, _contrastMat);
            }

            int hueLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Hue").intValue;
            if (hueLevel != 0)
            {
                _hueMat.SetInteger("_Hue", hueLevel);
                preview = Utility.ApplyShader(preview, _hueMat);
            }

            int saturationLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Saturation").intValue;
            if (saturationLevel != 0)
            {
                _saturationMat.SetInteger("_Saturation", saturationLevel);
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