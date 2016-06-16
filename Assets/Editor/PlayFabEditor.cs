using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;
    using UnityEditor.UI;

    public class PlayFabEditor : EditorWindow
    {

        internal static PlayFabEditor window;
        private static GUISkin skin;
        internal static float Progress = 0f;

        //Create a color vector for the background;  Dark Grey
        internal static Vector3 ColorVectorDarkGrey = PlayFabEditorHelper.GetColorVector(41);

        //Create a color vector for the background;  Light Grey
        internal static Vector3 ColorVectorLightGrey = PlayFabEditorHelper.GetColorVector(30);


        //create background texture
        internal static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVectorDarkGrey.x, ColorVectorDarkGrey.y, ColorVectorDarkGrey.z));

        [MenuItem("Window/PlayFab/Services")]
        static void PlayFabServices()
        {
            var editorAsm = typeof (Editor).Assembly;
            var inspWndType = editorAsm.GetType("UnityEditor.SceneHierarchyWindow"); //UnityEditor.InspectorWindow
            window = EditorWindow.GetWindow<PlayFabEditor>(inspWndType);
            window.titleContent = new GUIContent("PlayFab");
            skin = EditorGUIUtility.Load("Assets/Editor/PlayFabSkin.GUISkin") as GUISkin;
        }

        void OnGUI()
        {
            //Create a GUI Style
            var style = new GUIStyle();
            style.stretchHeight = true;
            style.normal.background = Background;
            //create global container with background properties.
            GUILayout.BeginVertical(style);

           //Run all updaters prior to drawing;  
            PlayFabEditorAuthenticate.Update();

            PlayFabEditorHeader.DrawHeader(Progress);

            if (PlayFabEditorAuthenticate.IsAuthenticated())
            {
                //Try catching to avoid Draw errors that do not actually impact the functionality
                //EG. Mismatch Draw Layout errors.
                try
                {
                    if (Progress >= .99f)
                    {
                        Progress = 0f;
                    }
                    PlayFabEditorMenu.DrawMenu();

                    switch (PlayFabEditorMenu._menuState)
                    {
                        case PlayFabEditorMenu.MenuStates.Sdks:
                            PlayFabEditorSDKTools.DrawSdkPanel();
                            break;
                        case PlayFabEditorMenu.MenuStates.Services:
                            break;
                        case PlayFabEditorMenu.MenuStates.Settings:
                            break;
                        default:
                            break;
                    }

                }
                catch
                {
                    //Do Nothing.
                }
            }
            else
            {
                PlayFabEditorAuthenticate.DrawLogin();
            }

            GUILayout.EndVertical();

            try
            {
                Repaint();
            }
            catch
            {
                //Do nothing.
            }
        }


    }
}