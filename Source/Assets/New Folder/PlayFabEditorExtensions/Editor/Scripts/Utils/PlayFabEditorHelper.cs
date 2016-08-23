using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PlayFab.Editor
{
    [InitializeOnLoad]
    public class PlayFabEditorHelper : UnityEditor.Editor 
    {
        #region EDITOR_STRINGS
        public static string DEV_API_ENDPOINT = "https://editor.playfabapi.com";
        public static string TITLE_ENDPOINT = ".playfabapi.com";
        public static string GAMEMANAGER_URL = "https://developer.playfab.com";
        public static string PLAYFAB_ASSEMBLY = "PlayFabSettings";
        public static string PLAYFAB_EDEX_MAINFILE = "PlayFabEditor.cs";
        public static string SDK_DOWNLOAD_PATH = "/Resources/PlayFabUnitySdk.unitypackage";
        public static string VAR_REQUEST_TIMING = "PLAYFAB_REQUEST_TIMING";
        public static string EDEX_VERSION = "0.99 beta";
        public static string EDEX_NAME = "PlayFabEditorExtensions";
        public static string ADMIN_API = "ENABLE_PLAYFABADMIN_API";
        public static string SERVER_API = "ENABLE_PLAYFABSERVER_API";
        public static string CLIENT_API = "DISABLE_PLAYFABCLIENT_API";
        public static string DEBUG_REQUEST_TIMING = "PLAYFAB_REQUEST_TIMING";
        public static string EDITOR_ROOT =  Application.dataPath + "/PlayFabEditorExtensions/Editor";
        public static string DEFAULT_SDK_LOCATION = "Assets/PlayFabSdk";
        public static string STUDIO_OVERRIDE = "_OVERRIDE_";
        #endregion
            
        public static GUISkin uiStyle = GetUiStyle();



        static PlayFabEditorHelper()
        {
            // scan for changes to the editor folder / structure.
            if(uiStyle == null)
            {
                string[] rootFiles = new string[0];
                bool relocatedEdEx = false;

                try
                {
                    EDITOR_ROOT = PlayFabEditorDataService.envDetails.edexPath ?? EDITOR_ROOT;
                    rootFiles = Directory.GetDirectories(EDITOR_ROOT);

                    uiStyle = GetUiStyle();
                }
                catch
                {

                    if(rootFiles.Length == 0)
                    {
                        // this probably means the editor folder was moved.
                        //see if we can locate the moved root
                        // and reload the assets

                        var movedRootFiles = Directory.GetFiles(Application.dataPath, PLAYFAB_EDEX_MAINFILE, SearchOption.AllDirectories);
                        if(movedRootFiles.Length > 0)
                        {
                            relocatedEdEx = true;
                            EDITOR_ROOT = movedRootFiles[0].Substring(0, movedRootFiles[0].IndexOf(PLAYFAB_EDEX_MAINFILE)-1);
                            PlayFabEditorDataService.envDetails.edexPath = EDITOR_ROOT;
                            PlayFabEditorDataService.SaveEnvDetails();

                            uiStyle = GetUiStyle();
                        }

                    }
                }
                finally
                {
                    if(relocatedEdEx && rootFiles.Length == 0)
                    {
                        Debug.Log(string.Format("Found new EdEx root: {0}", EDITOR_ROOT));
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

        public static GUISkin GetUiStyle()
        {
            if(uiStyle == null)
            {
                var relRoot = EDITOR_ROOT.Substring(EDITOR_ROOT.IndexOf("Assets/"));
                return (GUISkin)AssetDatabase.LoadAssetAtPath(relRoot+ "/UI/PlayFabStyles.guiskin", typeof(GUISkin));
            }
            else
            {
                return uiStyle;
            }
        }


        #region unused, but could be useful

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
        #endregion
    }
}