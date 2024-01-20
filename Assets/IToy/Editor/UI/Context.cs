using IToy.Core;
using UnityEditor;

namespace IToy.UI
{
    public class Context
    {
        [MenuItem("Assets/IToy/Remove white background", true)]
        [MenuItem("Assets/IToy/Remove black background", true)]
        [MenuItem("Assets/IToy/Flip horizontal", true)]
        [MenuItem("Assets/IToy/Flip vertical", true)]
        [MenuItem("Assets/IToy/Grayscale", true)]
        [MenuItem("Assets/IToy/Create toy", true)]
        static bool IsSupportedFileType() => Utility.IsSupportedFileType(Selection.activeObject);

        [MenuItem("Assets/IToy/Remove white background", secondaryPriority = 0)]
        static void RemoveWhiteBackground() => 
            Utility.CreateOrUpdateToy(Selection.activeObject, RemoveBackgroundOpts.White);

        [MenuItem("Assets/IToy/Remove black background", secondaryPriority = 1)]
        static void RemoveBlackBackground() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, RemoveBackgroundOpts.Black);

        [MenuItem("Assets/IToy/Flip horizontal", secondaryPriority = 2)]
        static void FlipHorizontal() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, "FlipHorizontal", true);

        [MenuItem("Assets/IToy/Flip vertical", secondaryPriority = 3)]
        static void FlipVertical() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, "FlipVertical", true);

        [MenuItem("Assets/IToy/Grayscale", secondaryPriority = 4)]
        static void Grayscale() =>
            Utility.CreateOrUpdateToy(Selection.activeObject, "Grayscale", -100);

        [MenuItem("Assets/IToy/Create toy", secondaryPriority = 5)]
        static void CreateToy() =>
            Utility.CreateOrUpdateToy(Selection.activeObject);
    }
}

