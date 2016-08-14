namespace PlayFab.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.IO;

    [InitializeOnLoad]
    public class PlayFabEditorHelper : Editor 
    {
        public static Font buttonFont = EditorGUIUtility.Load("Assets/PlayFabEditorExtensions/Editor/fonts/Avalon.ttf") as Font;
        public static Font buttonFontBold = EditorGUIUtility.Load("Assets/PlayFabEditorExtensions/Editor/fonts/Avalon Bold.ttf") as Font;
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
            
            if(buttonFont == null || buttonFontBold == null || uiStyle == null)
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
                            buttonFont = EditorGUIUtility.Load(relRoot+ "/fonts/Avalon.ttf") as Font;
                            buttonFontBold = EditorGUIUtility.Load(relRoot+ "/fonts/Avalon Bold.ttf") as Font;
                            uiStyle = (GUISkin)AssetDatabase.LoadAssetAtPath(relRoot+ "/ui/PlayFabStyles.guiskin", typeof(GUISkin));
                           // Debug.Log();
                        }

                    }
                }
                finally
                {
                    if(relocatedEdEx)
                    {
                        Debug.Log(string.Format("Found new EdEx root: {0}", editorRoot));
                    }
                    else
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



        public static GUIStyle GetOrangeButtonStyle()
        {
            var buttonStyle = PlayFabEditorHelper.GetButtonStyle();
            buttonStyle.font = PlayFabEditorHelper.buttonFontBold;
            buttonStyle.fontSize = 14;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.margin.right = 10;
            return buttonStyle;
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

        public static Texture2D GetTransparentBackground()
        {
            Color[] pix = new Color[32*32];
            var color = GetColor(255, 255, 255);
            color.a = 0f;
            for (int i = 0; i < pix.Length; i++)
                pix[i] = color;

            Texture2D result = new Texture2D(32, 32);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Color GetColor(int r, int g, int b)
        {
            return new Color((r/255f), (g/255f), (b/255f));
        }

        public static GUIStyle GetTextButtonStyle()
        {
            var buttonStyle = new GUIStyle
            {
                clipping =TextClipping.Clip,
                font = buttonFontBold,
                padding =
                {
                    left = 5,
                    top = 5
                },
                fontSize = 11,
                normal =
                {
                    textColor = PlayFabEditorHelper.GetColor(149, 153, 149),
                    background = GetTransparentBackground() 
                },
                //onNormal = {textColor = PlayFabEditorHelper.GetColor(149, 153, 149), background = GetTransparentBackground()},
                hover =
                {
                    textColor = PlayFabEditorHelper.GetColor(24, 180, 194),
                    background = GetTransparentBackground()
                },
                active =
                {
                    textColor = PlayFabEditorHelper.GetColor(24, 180, 194),
                    background = GetTransparentBackground()
                }
            };
            //buttonStyle.normal.textColor = PlayFabEditorHelper.GetColor(149, 153, 149);
            return buttonStyle;
        }

        public static GUIStyle GetButtonStyle()
        {
            var buttonStyle = new GUIStyle
            {
                font = buttonFontBold,
                padding =
                {
                    left = 0,
                    top = 0
                },
                fontSize = 11,
                normal =
                {
                    textColor = PlayFabEditorHelper.GetColor(255, 255, 255),
                    background = MakeTex(24,24, GetColor(253,107,13))
                },
                //onNormal = {textColor = PlayFabEditorHelper.GetColor(149, 153, 149), background = GetTransparentBackground()},
                hover =
                {
                    textColor = PlayFabEditorHelper.GetColor(255, 255, 255),
                    background = MakeTex(24,24, GetColor(255,125,41))
                },
                active =
                {
                    textColor = PlayFabEditorHelper.GetColor(255, 255, 255),
                    background = MakeTex(24,24, GetColor(255,125,41))
                }
            };
            //buttonStyle.normal.textColor = PlayFabEditorHelper.GetColor(149, 153, 149);
            return buttonStyle;
        }

    }
}