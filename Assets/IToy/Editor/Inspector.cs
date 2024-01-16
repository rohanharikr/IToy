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

        public Material flipHorizontalMaterial;

        private void OnEnable()
        {
            control = (IToyControl)target;
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IToy/Data/logo.png");
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/IToy/Shaders/FlipHorizontal.shader");
            flipHorizontalMaterial = new Material(shader);

            // Create a texture. Texture size does not matter, since
            // LoadImage will replace with the size of the incoming image.
            original = new Texture2D(0, 0);

            ImageConversion.LoadImage(original, control.Original);

            preview = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/cat.png");
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
                RedrawPreview();
            }
        }

        void RedrawPreview()
        {
            preview = original;
            if(control.Transform.FlipHorizontal)
            {
                preview = ApplyShader(preview, flipHorizontalMaterial);
            }
        }

        Texture2D ApplyShader(Texture2D inputTexture, Material shaderMaterial)
        {
            Texture2D res = new Texture2D(inputTexture.width, inputTexture.height);
            RenderTexture rt = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height);
            Graphics.Blit(inputTexture, rt, shaderMaterial);
            RenderTexture.active = rt;
            res.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            res.Apply();
            return res;
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