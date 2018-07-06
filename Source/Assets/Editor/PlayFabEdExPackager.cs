using PlayFab.PfEditor;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PlayFab.Internal
{
    public static class PlayFabEdExPackager
    {
        private static readonly string[] SdkAssets = {
            "Assets/PlayFabEditorExtensions"
        };

        /// <summary>
        /// We deliberately don't check things that would cause exceptions here.
        /// If something fails, we need it to throw an exception to cause the error code to be != 0,
        ///   which means catching and throwing it/another anyways, so just don't bother
        /// </summary>
        [MenuItem("PlayFab/Testing/Build PlayFab EdEx UnityPackage")]
        public static void BuildUnityPackage()
        {
            var versionSrcFile = "C:/depot/API_Specs/SdkManualNotes.json"; // TODO: Don't hard code this
            var notes = File.ReadAllText(versionSrcFile);
            var searchRegex = "\"unity-v2\": \"([0-9]+\\.[0-9]+\\.[0-9]+)\"";
            var match = Regex.Match(notes, searchRegex);
            var unitySdkVersion = match.Captures[0].Value.Replace("\"", "").Replace("unity-v2:", "").Trim();

            var versionDefFiles = Directory.GetFiles(Application.dataPath, "PlayFabEditorVersion.cs", SearchOption.AllDirectories);
            var versionDefFile = versionDefFiles[0];
            var contents = PlayFabEditorHelper.EDEX_VERSION_TEMPLATE.Replace("{sdkVersion}", unitySdkVersion);
            File.WriteAllText(versionDefFile, contents);

            // We just changed a file we're about to publish - May not work, we might have to change this to be two console calls
            AssetDatabase.Refresh();

            var packagePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Packages/PlayFabEditorExtensions.unitypackage"));
            if (!File.Exists(packagePath))
                throw new System.Exception(packagePath + " should already exist");
            AssetDatabase.ExportPackage(SdkAssets, packagePath, ExportPackageOptions.Recurse);
            Debug.Log("Package built: " + packagePath);
        }
    }
}
