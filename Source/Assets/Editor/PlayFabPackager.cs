using UnityEditor;
using UnityEngine;

namespace PlayFab.Internal
{
    public static class PlayFabPackager
    {
        private static readonly string[] SdkAssets = {
        "Assets/PlayFabEditorExtensions"
    };

        [MenuItem("PlayFab/Build PlayFab UnityPackage")]
        public static void BuildUnityPackage()
        {
            var packagePath = "C:/depot/sdks/UnityEditorExtensions/Packages/PlayFabEditorExtensions.unitypackage";
            AssetDatabase.ExportPackage(SdkAssets, packagePath, ExportPackageOptions.Recurse);
            Debug.Log("Package built: " + packagePath);
        }
    }
}
