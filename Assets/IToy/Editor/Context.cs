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
            ToyUtility.CreateOrUpdateToy(Selection.activeObject, RemoveBackgroundOpts.White);

        [MenuItem("Assets/IToy/Remove black background", secondaryPriority = 1)]
        static void RemoveBlackBackground() =>
            ToyUtility.CreateOrUpdateToy(Selection.activeObject, RemoveBackgroundOpts.White);

        [MenuItem("Assets/IToy/Flip horizontal", secondaryPriority = 2)]
        static void FlipHorizontal() =>
            ToyUtility.CreateOrUpdateToy(Selection.activeObject, "FlipHorizontal", true);

        [MenuItem("Assets/IToy/Flip vertical", secondaryPriority = 3)]
        static void FlipVertical() =>
            ToyUtility.CreateOrUpdateToy(Selection.activeObject, "FlipVertical", true);

        [MenuItem("Assets/IToy/Grayscale", secondaryPriority = 4)]
        static void Grayscale() =>
            ToyUtility.CreateOrUpdateToy(Selection.activeObject, "Grayscale", 0);

        [MenuItem("Assets/IToy/Create toy", secondaryPriority = 5)]
        static void CreateToy() =>
            ToyUtility.CreateOrUpdateToy(Selection.activeObject);
    }
}

