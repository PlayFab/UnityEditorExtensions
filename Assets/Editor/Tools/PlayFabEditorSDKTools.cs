using System;
using System.Linq;

namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorSDKTools : Editor
    {
        internal static bool IsInstalled = IsSDKInstalled();
        private static Vector3 ColorVector = PlayFabEditorHelper.GetColorVector(62);
        private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVector.x, ColorVector.y, ColorVector.z));

        public static void DrawSdkPanel()
        {
            if (IsInstalled)
            {
                //SDK is installed.
            }
            else
            {
                //NO SDK Is Installed..

                //Create a GUI Style
//                var style = new GUIStyle();
//                //Set the fixed height of this container
//                style.fixedHeight = 100f;
//                style.margin.top = 10;
//                style.normal.background = Background;
//
//                var textStyle = PlayFabEditorHelper.GetTextButtonStyle();
//                textStyle.alignment = TextAnchor.MiddleCenter;
//                textStyle.fontSize = 14;
                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

                GUIStyle labelStyle = new GUIStyle(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 18;

                GUILayout.Label("No SDK is installed.", labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                GUILayout.Space(20);

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                var buttonWidth = 150;

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Install PlayFab SDK", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32)))
                {
                    ImportSDK();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            checkSdkVersion();
        }

        public static void ImportSDK()
        {
            AssetDatabase.ImportPackage(@"C:\_GitRoot\_PlayFab\sdks\UnitySDK_Experimental\PlayFabUnitySDK_Bak.unitypackage", false);
        }

        private static bool IsSDKInstalled()
        {
            var playfabVersionType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabSettings"
                        select type);
            
            //Debug.Log(playfabVersionType.GetType());

            return playfabVersionType.ToList().Count > 0;
        }

        private static void checkSdkVersion()
        {

        }

    }
}