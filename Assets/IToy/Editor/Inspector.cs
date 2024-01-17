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

        Toy _toy;
        Texture2D _logo;
        Texture2D _original;

        Processor _processor;

        SerializedProperty _removeBackground;

        private void OnEnable() => Init();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(_logo, GUILayout.Width(100), GUILayout.Height(100));
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    EditorGUILayout.Space(20);
                    EditorGUILayout.LabelField("IToy Image Processor", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("@rohanharikr / 2024.01");
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
                    GUILayout.Label(_original, GUILayout.Width(previewSize), GUILayout.Height(previewSize));
                    EditorGUILayout.LabelField("Original / オリジナル");
                }
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(_processor.texture, GUILayout.Width(previewSize), GUILayout.Height(previewSize));

                    GUIStyle labelStyle = new(EditorStyles.label);
                    labelStyle.normal.textColor = new Color(242 / 255f, 109 / 255f, 28 / 255f);
                    EditorGUILayout.LabelField("Preview / プレビュー", labelStyle);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_removeBackground);
            if (_removeBackground.intValue == (int)RemoveBackgroundOpts.Custom)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RemoveBackgroundColor"), new GUIContent(" "));

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
            string currentFileGUID = serializedObject.FindProperty("Current").stringValue;
            string filePath = AssetDatabase.GUIDToAssetPath(currentFileGUID);
            _processor.WriteToDisk(filePath);
        }

        void Init()
        {
            _toy = (Toy)target;
            _logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Media/logo.png");
            _removeBackground = serializedObject.FindProperty("RemoveBackground");
            _processor = new Processor();

            // Create a texture. Texture size does not matter, since
            // LoadImage will replace with the size of the incoming image.
            _original = new Texture2D(0, 0);

            ImageConversion.LoadImage(_original, _toy.Original);

            DrawPreview();
        }

        void DrawPreview()
        {
            _processor.texture = _original;

            RectInt cropValues = serializedObject.FindProperty("Transform").FindPropertyRelative("Crop").rectIntValue;
            if (cropValues.x != 0 || cropValues.y != 0 || cropValues.width != 0 || cropValues.height != 0)
                _processor.Crop(cropValues);

            int removeBackground = serializedObject.FindProperty("RemoveBackground").enumValueIndex;
            if (_toy.RemoveBackground != (int)RemoveBackgroundOpts.None)
            {
                Color colorToRemove;
                if(removeBackground == (int)RemoveBackgroundOpts.White)
                    colorToRemove = Color.white;
                else if(removeBackground == (int)RemoveBackgroundOpts.Black)
                    colorToRemove = Color.black;
                else
                    colorToRemove = serializedObject.FindProperty("RemoveBackgroundColor").colorValue;

                _processor.RemoveBackground(colorToRemove);
            }

            if (_toy.Transform.FlipHorizontal)
                _processor.FlipHorizontal();

            if (_toy.Transform.FlipVertical)
                _processor.FlipVertical();

            int brightnessLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Brightness").intValue;
            if(brightnessLevel != 0)
                _processor.Brightness(brightnessLevel);

            int contrastLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Contrast").intValue;
            if (contrastLevel != 0)
                _processor.Brightness(contrastLevel);

            int hueLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Hue").intValue;
            if (hueLevel != 0)
                _processor.Brightness(hueLevel);

            int saturationLevel = serializedObject.FindProperty("Correction").FindPropertyRelative("Saturation").intValue;
            if (saturationLevel != 0)
                _processor.Brightness(saturationLevel);
        }
    }
}