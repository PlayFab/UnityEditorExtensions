using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace PlayFab.Editor
{
    public class PlayFabEditorHelpMenu : UnityEditor.Editor
    {
        public static float buttonWidth = 200;
        public static Vector2 scrollPos = Vector2.zero;

        public static void DrawHelpPanel()
        {
            buttonWidth = EditorGUIUtility.currentViewWidth > 400 ? EditorGUIUtility.currentViewWidth/2 : 200;

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

                    if (GUILayout.Button("ASK QUESTIONS", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        Application.OpenURL("https://community.playfab.com/index.html");
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
        }
  
    }

}
