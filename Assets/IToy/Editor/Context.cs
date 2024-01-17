using UnityEditor;

namespace IToy
{
    public class Context
    {
        [MenuItem("Assets/IToy/Remove white background", true)]
        [MenuItem("Assets/IToy/Remove black background", true)]
        [MenuItem("Assets/IToy/Flip horizontal", true)]
        [MenuItem("Assets/IToy/Flip vertical", true)]
        [MenuItem("Assets/IToy/Create toy", true)]
        static bool IsSupportedFileType() => Utility.IsSupportedFileType(Selection.activeObject);

        [MenuItem("Assets/IToy/Remove white background", secondaryPriority = 0)]
        static void RemoveWhiteBackground() => 
            Utility.CreateOrUpdateToy(Selection.activeObject, RemoveBackgroundOpts.White);

        [MenuItem("Assets/IToy/Flip Horizontal")]
        static void FlipHorizontal() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, "FlipHorizontal", true);

        [MenuItem("Assets/IToy/Flip Vertical")]
        static void FlipVertical() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, "FlipVertical", true);

        [MenuItem("Assets/IToy/Grayscale")]
        static void Grayscale() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, "Grayscale", 0);
    }
}

