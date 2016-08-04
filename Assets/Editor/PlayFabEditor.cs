

namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UI;
    using PlayFab.Editor.EditorModels;
    public class PlayFabEditor : EditorWindow
    {
        public static List<Studio> Studios = new List<Studio>();



        // testing alt update loop
        public delegate void playFabEditorUpdate();
        public static event playFabEditorUpdate UpdateLoopTick;

        internal static PlayFabEditor window;
        internal static float Progress = 0f;
        internal static bool HasEditorShown;

        //Create a color vector for the background;  Dark Grey
        internal static Vector3 ColorVectorDarkGrey;

        //Create a color vector for the background;  Light Grey
        internal static Vector3 ColorVectorLightGrey;

        //create background texture
        internal static Texture2D Background;

        private ListDisplay listDisplay;
       

        void OnEnable()
        {
            ColorVectorDarkGrey = PlayFabEditorHelper.GetColorVector(41);
            ColorVectorLightGrey = PlayFabEditorHelper.GetColorVector(30);
            Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVectorDarkGrey.x, ColorVectorDarkGrey.y, ColorVectorDarkGrey.z));

            if (window == null)
            {
                window = this;
                window.minSize = new Vector2(300, 0);
            }


        }

        void OnFocus()
        {
            OnEnable();
        }

        [MenuItem("Window/PlayFab/Services")]
        static void PlayFabServices()
        {
            var editorAsm = typeof (Editor).Assembly;
            //var inspWndType = editorAsm.GetType("UnityEditor.SceneHierarchyWindow"); //UnityEditor.InspectorWindow
            var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
            window = EditorWindow.GetWindow<PlayFabEditor>(inspWndType);
            window.titleContent = new GUIContent("PlayFab");
            EditorPrefs.SetBool("PlayFabToolsShown", true);
        }

        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                if (!EditorPrefs.HasKey("PlayFabToolsShown") || !PlayFabEditorSDKTools.IsInstalled)
                {
                    EditorCoroutine.start(OpenPlayServices());
                }
            }
        }

        static IEnumerator OpenPlayServices()
        {
            yield return new WaitForSeconds(1f);
            if (!Application.isPlaying)
            {
                PlayFabServices();
            }
        }


        void OnGUI()
        {
            try
            {
                //GUI.skin = PlayFabEditorHelper.uiStyle;

                 //Create a GUI Style
//                var style = new GUIStyle();
//                style.stretchHeight = true;
//                style.normal.background = Background;
//                //create global container with background properties.




                GUILayout.BeginVertical(); //style



                //Run all updaters prior to drawing;  
                PlayFabEditorAuthenticate.Update();
                PlayFabEditorSettings.Update();

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
                                PlayFabEditorSettings.DrawSettingsPanel();
                                PlayFabEditorSettings.After(); //TODO why is this getting called every frame?
                                break;
                            case PlayFabEditorMenu.MenuStates.Help:
                                PlayFabEditorHelpMenu.DrawHelpPanel();
                                break;
                                
                            default:
                                break;
                        }




                    }
                    catch (Exception e)
                    {
                        //Do Nothing.
                        //Debug.LogException(e); // currently gettting a few errores: Getting control 1's position in a group with only 1 controls when doing Repaint
                    }
                }
                else
                {
                    PlayFabEditorAuthenticate.DrawAuthPanels();
                }

                if(UpdateLoopTick != null)
                {
                    UpdateLoopTick();
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
            catch (Exception e)
            {
                //Do Nothing.. 
            }


            //GUI.Button(new Rect(0,0,EditorGUIUtility.currentViewWidth,1000), "", PlayFabEditorHelper.uiStyle.GetStyle("gpStyleBlur"));
            //GUILayout.EndArea();

        }

        void OnDisable()
        {
            // clean up objects:
            //Object.DestroyImmediate(testObjA);
            //Object.DestroyImmediate(testObjB);
            //Object.DestroyImmediate(testObjC);
            UnityEngine.Object.DestroyImmediate(listDisplay);
            EditorPrefs.SetBool("IsPlayFabAuthenticated", false);
        }
    }
}