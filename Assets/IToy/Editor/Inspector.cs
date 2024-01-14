using UnityEditor;

[CustomEditor(typeof(DefaultAsset))]
public class ControlInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var path = AssetDatabase.GetAssetPath(target);

        if (path.EndsWith(".control"))
        {
            SVGInspectorGUI();
        }
        else
        {
            base.OnInspectorGUI();
        }
    }

    private void SVGInspectorGUI()
    {
        EditorGUILayout.LabelField("Wow!!!");
    }
}