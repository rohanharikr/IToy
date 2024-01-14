using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IToyControl))]
public class ControlInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        IToyControl control = (IToyControl)target;
        Debug.Log(control.Original.width);

        using (new EditorGUILayout.HorizontalScope())
        {
            Texture2D imageOne = Resources.Load<Texture2D>("cat");
            GUILayout.Box(imageOne, GUILayout.Width(212), GUILayout.Height(212));
            Texture2D imageTwo = Resources.Load<Texture2D>("cat");
            GUILayout.Box(imageTwo, GUILayout.Width(212), GUILayout.Height(212));
        }

        EditorGUILayout.ColorField("Remove", Color.gray);
        EditorGUILayout.Slider("Brightness", 50f, 0f, 100f);
        EditorGUILayout.Slider("Contrast", 50f, 0f, 100f);
        EditorGUILayout.Slider("Saturation", 50f, 0f, 100f);

        GUILayout.Button("Reset");
        GUILayout.Button("Self Destruct");
    }
}