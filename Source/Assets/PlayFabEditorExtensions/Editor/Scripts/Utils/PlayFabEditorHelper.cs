namespace PlayFab.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.IO;

    [InitializeOnLoad]
    public class PlayFabEditorHelper : Editor 
    {
        public static GUISkin uiStyle = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/PlayFabEditorExtensions/Editor/ui/PlayFabStyles.guiskin", typeof(GUISkin)));


        public static string editorRoot = Application.dataPath + "/PlayFabEditorExtensions/Editor";

        public static Dictionary<string, string> stringTable = new Dictionary<string, string>()
        {
            
            { "ApiEndpoint", @"https://editor.playfabapi.com" },
            { "TitleEndPoint", @".playfabapi.com" },

            {"DebugRequestTiming", "PLAYFAB_REQUEST_TIMING"},
            {"PlayFabAssembly", "PlayFabSettings"},
            {"SdkDownloadPath", "/Editor/Tools/Resources/PlayFabUnitySdk.unitypackage" }
        };





        static PlayFabEditorHelper()
        {
            
            if(uiStyle == null)
            {
                string[] rootFiles = new string[0];
                bool relocatedEdEx = false;

                try
                {
                    rootFiles = Directory.GetDirectories(editorRoot);
                }
                catch
                {

                    if(rootFiles.Length == 0)
                    {
                        // this probably means the editor folder was moved.
                        //see if we can locate the moved root
                        // and reload the assets

                        var movedRootFiles = Directory.GetFiles(Application.dataPath, "PlayFabEditor.cs", SearchOption.AllDirectories);
                        if(movedRootFiles.Length > 0)
                        {
                            relocatedEdEx = true;
                            editorRoot = movedRootFiles[0].Substring(0, movedRootFiles[0].IndexOf("PlayFabEditor.cs")-1);

                            var relRoot = editorRoot.Substring(editorRoot.IndexOf("Assets/"));
                            uiStyle = (GUISkin)AssetDatabase.LoadAssetAtPath(relRoot+ "/UI/PlayFabStyles.guiskin", typeof(GUISkin));
                        }

                    }
                }
                finally
                {
                    if(relocatedEdEx && rootFiles.Length == 0)
                    {
                        Debug.Log(string.Format("Found new EdEx root: {0}", editorRoot));
                    }
                    else if(rootFiles.Length == 0)
                    {
                        Debug.Log("Could not relocate the PlayFab Editor Extension");
                    }
                }
            }
        }




        public static string GetEventJson()
        {
            return "{\"useSpinner\":true, \"blockUi\":true }";
        }

      

        /// <summary>
        /// Tool to create a color background texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="col"></param>
        /// <returns>Texture2D</returns>
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width*height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Vector3 GetColorVector(int colorValue)
        {
            return new Vector3((colorValue/255f), (colorValue/255f), (colorValue/255f));
        }

    }
}