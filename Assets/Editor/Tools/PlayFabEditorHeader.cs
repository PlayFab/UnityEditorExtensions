namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorHeader : Editor
    {

        public static void DrawHeader(float progress = 0f)
        {
            //Create a GUI Style
//            var style = new GUIStyle();
//            //Set the fixed height of this container
//            style.fixedHeight = 52f;
//            //draw the background
//            style.normal.background = Background;

            //using Begin Vertical as our container.
            GUILayout.BeginHorizontal();

            //Set the image in the container
            if (EditorGUIUtility.currentViewWidth < 375)
            {
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("pfLogo"), GUILayout.MaxHeight(40), GUILayout.Width(186));
            }
            else
            {
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("pfLogo"), GUILayout.MaxHeight(50), GUILayout.Width(233));
            }


            float gmAnchor = EditorGUIUtility.currentViewWidth - 30;


                if (EditorGUIUtility.currentViewWidth > 375)
                {
                    gmAnchor = EditorGUIUtility.currentViewWidth - 140;
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, 140, 42));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("GAME MANAGER", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(105)))
                    {
                        OnDashbaordClicked();
                    }
                }
                else
                {
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, EditorGUIUtility.currentViewWidth * .25f, 42));
                    GUILayout.BeginHorizontal();
                }

                if (GUILayout.Button("", PlayFabEditorHelper.uiStyle.GetStyle("gmIcon")))
                    {
                        OnDashbaordClicked();
                    }
               GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //end the vertical container
            GUILayout.EndHorizontal();

            //define a progress bar or just create empty space where the bar would go.
            if (progress > 0)
            {
                DrawProgressBar(progress); 
            }
            /*
            else
            {
                GUILayout.Space(10);
            }*/

        }

        private static void DrawProgressBar(float progress) //progress = 0 -> 1
        {
            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("progressBarBg"));
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("progressBarFg"), GUILayout.Width(EditorGUIUtility.currentViewWidth * progress));
            GUILayout.EndHorizontal();
        }

        private static void OnDashbaordClicked()
        {
            Debug.Log("Dashboard Clicked");
            var url = @"https://developer.playfab.com";


            Help.BrowseURL(EditorPrefs.HasKey("PlayFabActiveTitleUrl") ? EditorPrefs.GetString("PlayFabActiveTitleUrl") : url);
            //PlayFabWebWindow.OpenWindow(url);
        }

    }
}