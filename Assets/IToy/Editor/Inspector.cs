using System.IO;
using UnityEditor;
using UnityEngine;

namespace IToy
{
    [CustomEditor(typeof(Toy))]
    [CanEditMultipleObjects]
    public class ToyInspector : Editor
    {
        bool _isAdvancedExpanded = false;

        Toy toy;
        Texture2D logo;
        Texture2D original;

        Texture2D preview;

        #region Serialized properties
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
                Texture2D transparencyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Media/transparency-bg.jpg");
                int previewSize = 208;
                GUI.Box(new Rect(18, 115, previewSize, previewSize), transparencyTex);
                GUI.Box(new Rect(241, 115, previewSize, previewSize), transparencyTex);

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(original, GUILayout.Width(previewSize), GUILayout.Height(previewSize));
                    EditorGUILayout.LabelField("Original / オリジナル");
                }
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(preview, GUILayout.Width(previewSize), GUILayout.Height(previewSize));

                    GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.textColor = new Color(242 / 255f, 109 / 255f, 28 / 255f);
                    EditorGUILayout.LabelField("Preview / プレビュー", labelStyle);
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
                    EditorGUILayout.LabelField("Reset the image to it's former glory");
                    GUILayout.Button("Reset / リセット", GUILayout.ExpandWidth(false));
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Keep the image but bid the toy farewell");
                    GUILayout.Button("Send off IToy / 別れ", GUILayout.ExpandWidth(false));
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Reset the image and remove the toy");
                    if (GUILayout.Button("Self-Destruct / 自己破壊", GUILayout.ExpandWidth(false)))
                    {
                        SelfDestruct(toy);
                    };
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Update / アップデート", GUILayout.ExpandWidth(false)))
                Update();
              
            EditorGUILayout.Space(20);

            if (serializedObject.ApplyModifiedProperties())
            {
                DrawPreview();
            }
        }

        void Update()
        {
            byte[] file = preview.EncodeToPNG();
            File.WriteAllBytes("Assets/Resources/cat.png", file);
            AssetDatabase.Refresh();
        }

        void Init()
        {
            toy = (Toy)target;
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Media/logo.png");

            #region Init serializable properties
            _removeBackground = serializedObject.FindProperty("RemoveBackground");
            _flipHorizontal = serializedObject.FindProperty("Transform").FindPropertyRelative("FlipHorizontal");
            _flipVertical = serializedObject.FindProperty("Transform").FindPropertyRelative("FlipVertical");
            _crop = serializedObject.FindProperty("Transform").FindPropertyRelative("Crop");
            _brightness = serializedObject.FindProperty("Correction").FindPropertyRelative("Brightness");
            _contrast = serializedObject.FindProperty("Correction").FindPropertyRelative("Contrast");
            _hue = serializedObject.FindProperty("Correction").FindPropertyRelative("Hue");
            _saturation = serializedObject.FindProperty("Correction").FindPropertyRelative("Saturation");
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

            ImageConversion.LoadImage(original, toy.Original);

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

            if (toy.Transform.FlipHorizontal)
                preview = Utility.ApplyShader(preview, _flipHorizontalMat);

            if (toy.Transform.FlipVertical)
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

        void SelfDestruct(Toy toy)
        {
            string currentAssetPath = AssetDatabase.GetAssetPath(toy.Current);
            string currentAssetDirPath = Path.GetDirectoryName(currentAssetPath);
            AssetDatabase.DeleteAsset(currentAssetPath);
            File.WriteAllBytes(Path.Combine(currentAssetDirPath, "cat.png"), toy.Original);
            string toyPath = AssetDatabase.GetAssetPath(toy);
            AssetDatabase.DeleteAsset(toyPath);
            AssetDatabase.Refresh();
        }
    }
}