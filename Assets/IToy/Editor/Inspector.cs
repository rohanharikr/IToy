using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[CustomEditor(typeof(IToyControl))]
public class ControlInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}