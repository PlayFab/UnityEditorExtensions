using System;
using System.Linq;

namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEditor;

    public class PlayFabEditorHelpMenu : Editor
    {
        public static float buttonWidth = 200;
        public static Vector2 scrollPos = Vector2.zero;

        public static void DrawHelpPanel()
        {
            buttonWidth = EditorGUIUtility.currentViewWidth > 400 ? EditorGUIUtility.currentViewWidth/2 : 200;

            //scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.Width(EditorGUIUtility.currentViewWidth - 80));

                GUILayout.Label("LEARN PLAYFAB:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("BEGINNERS GUIDE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://api.playfab.com/docs/beginners-guide");
                    }

                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("RECIPES", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://api.playfab.com/docs/recipe-index");
                    }

                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("TUTORIALS", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://api.playfab.com/docs/tutorials");
                    }

                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("API REFERENCE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://api.playfab.com/documentation");
                    }

                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
               
            GUILayout.EndVertical();

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

                GUILayout.Label("TROUBLESHOOTING:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("REPORT ISSUES", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://support.playfab.com/hc/en-us/requests/new");
                    }

                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("ASK QUESTIONS", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://api.playfab.com/https://community.playfab.com/");
                    }

                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("VIEW SERVICE AVAILABILITY", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("http://status.playfab.com/");
                    }

                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


            GUILayout.EndVertical();
           // GUILayout.EndScrollView();


        }
  
    }

}
