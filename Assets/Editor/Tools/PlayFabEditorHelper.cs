namespace PlayFab.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;

    public class PlayFabEditorHelper : Editor 
    {
        public static Font buttonFont = EditorGUIUtility.Load("Assets/Editor/fonts/Avalon.ttf") as Font;
        public static Font buttonFontBold = EditorGUIUtility.Load("Assets/Editor/fonts/Avalon Bold.ttf") as Font;
        public static GUISkin uiStyle = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/Editor/ui/PlayFabStyles.guiskin", typeof(GUISkin)));




        public static Dictionary<string, string> stringTable = new Dictionary<string, string>()
        {
            
            { "ApiEndpoint", @"https://editor.playfabapi.com" },
            { "TitleEndPoint", @".playfabapi.com" },

            {"DebugRequestTiming", "PLAYFAB_REQUEST_TIMING"},
            {"PlayFabAssembly", "PlayFabSettings"},
            {"SdkDownloadPath", "/Editor/Tools/Resources/PlayFabUnitySdk.unitypackage" }
        };







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