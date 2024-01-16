using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderData;

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

        #region Original values
        int _removeBackgroundOriginal;
        bool _flipHorizontalOriginal;
        bool _flipVerticalOriginal;
        RectInt _cropOriginal;
        int _brightnessOriginal;
        int _contrastOriginal;
        int _hueOriginal;
        int _saturationOriginal;
        #endregion

        #region Current
        SerializedProperty _removeBackground;
        SerializedProperty _flipHorizontal;
        SerializedProperty _flipVertical;
        SerializedProperty _crop;
        SerializedProperty _brightness;
        SerializedProperty _contrast;
        SerializedProperty _hue;
        SerializedProperty _saturation;
        #endregion

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

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(logo, GUILayout.Width(100), GUILayout.Height(100));
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    EditorGUILayout.Space(20);
                    EditorGUILayout.LabelField("IToy Image Processor", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("@rohanharikr / 2024.01.17");
                    if (EditorGUILayout.LinkButton("Source on GitHub"))
                        Application.OpenURL("https://github.com/rohanharikr/IToy");
                }
            }

            EditorGUILayout.Separator();

            using (new EditorGUILayout.HorizontalScope())
            {
                Texture2D transparencyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/transparency-bg.jpg");
                int previewSize = 208;
                GUI.Box(new Rect(18, 115, previewSize, previewSize), transparencyTex);
                GUI.Box(new Rect(241, 115, previewSize, previewSize), transparencyTex);

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(original, GUILayout.Width(previewSize), GUILayout.Height(previewSize));
                    EditorGUILayout.LabelField("Original");
                }
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(preview, GUILayout.Width(previewSize), GUILayout.Height(previewSize));

                    GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.textColor = new Color(242 / 255f, 109 / 255f, 28 / 255f);
                    EditorGUILayout.LabelField("Preview", labelStyle);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_removeBackground);
            if (_removeBackground.intValue == (int)RemoveBackgroundOpts.Custom)
                EditorGUILayout.PropertyField(_removeBackground, new GUIContent(" "));

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Transform"));

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Correction"));

            EditorGUILayout.Separator();

            _isAdvancedExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isAdvancedExpanded, "Advanced");
            if (_isAdvancedExpanded)
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Reset the image to it's former glory.");
                    GUILayout.Button("Reset / リセット", GUILayout.ExpandWidth(false));
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Reset the image to it's former glory.");
                    GUILayout.Button("Send off IToy / 別れ", GUILayout.ExpandWidth(false));
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Reset + Send off");
                    if (GUILayout.Button("Self-Destruct / 自己破壊", GUILayout.ExpandWidth(false)))
                    {
                        SelfDestruct(control);
                    };
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            
            if(ChangesMade())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save changes", GUILayout.ExpandWidth(false)))
                    {
                        byte[] file = preview.EncodeToPNG();
                        File.WriteAllBytes("Assets/Resources/cat.png", file);
                        AssetDatabase.Refresh();
                    };
                    if (GUILayout.Button("Discard changes", GUILayout.ExpandWidth(false)))
                    {
                        _flipHorizontal.boolValue = _flipHorizontalOriginal;
                        //TBD
                    };
                }
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                DrawPreview();
            }
        }

        bool ChangesMade()
        {
            if (_removeBackgroundOriginal != _removeBackground.enumValueIndex)
                return true;

            if (_flipHorizontalOriginal != _flipHorizontal.boolValue)
                return true;

            if (_flipVerticalOriginal != _flipVertical.boolValue)
                return true;

            if (_cropOriginal.x != _crop.rectIntValue.x ||
                _cropOriginal.y != _crop.rectIntValue.y ||
                _cropOriginal.width != _crop.rectIntValue.width ||
                _cropOriginal.height != _crop.rectIntValue.height
                )
                return true;

            if (_brightnessOriginal != _brightness.intValue)
                return true;

            if (_contrastOriginal != _contrast.intValue)
                return true;

            if (_hueOriginal != _hue.intValue)
                return true;

            if (_saturationOriginal != _saturation.intValue)
                return true;

            return false;
        }

        void Init()
        {
            control = (IToyControl)target;
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");

            #region Init serializable properties
            _removeBackground = serializedObject.FindProperty("RemoveBackground");
            _removeBackgroundOriginal = _removeBackground.enumValueIndex;
            _flipHorizontal = serializedObject.FindProperty("Transform").FindPropertyRelative("FlipHorizontal");
            _flipHorizontalOriginal = _flipHorizontal.boolValue;
            _flipVertical = serializedObject.FindProperty("Transform").FindPropertyRelative("FlipVertical");
            _flipVerticalOriginal = _flipVertical.boolValue;
            _crop = serializedObject.FindProperty("Transform").FindPropertyRelative("Crop");
            _cropOriginal = _flipVertical.rectIntValue;
            _brightness = serializedObject.FindProperty("Correction").FindPropertyRelative("Brightness");
            _brightnessOriginal = _brightness.intValue;
            _contrast = serializedObject.FindProperty("Correction").FindPropertyRelative("Contrast");
            _contrastOriginal = _contrast.intValue;
            _hue = serializedObject.FindProperty("Correction").FindPropertyRelative("Hue");
            _hueOriginal = _hue.intValue;
            _saturation = serializedObject.FindProperty("Correction").FindPropertyRelative("Saturation");
            _saturationOriginal = _saturation.intValue;
            #endregion

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

            _backgroundMat.RevertAllPropertyOverrides();
            _cropMat.RevertAllPropertyOverrides();
            _brightnessMat.RevertAllPropertyOverrides();
            _contrastMat.RevertAllPropertyOverrides();
            _hueMat.RevertAllPropertyOverrides();
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