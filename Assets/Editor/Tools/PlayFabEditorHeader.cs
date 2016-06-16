namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorHeader : Editor
    {
        private static Texture2D LogoTex = EditorGUIUtility.Load("Assets/Editor/images/playfablogo.png") as Texture2D;

        private static Texture2D DashboardIcon =
            EditorGUIUtility.Load("Assets/Editor/images/dashboardIcon.png") as Texture2D;

        private static Texture2D DashboardIconHover =
            EditorGUIUtility.Load("Assets/Editor/images/dashboardIconHover.png") as Texture2D;

        private static Texture2D Background = PlayFabEditorHelper.MakeTex(50, (int)EditorGUIUtility.currentViewWidth,
                new Color(PlayFabEditor.ColorVectorDarkGrey.x, PlayFabEditor.ColorVectorDarkGrey.y, PlayFabEditor.ColorVectorDarkGrey.z));

        public static void DrawHeader(float progress = 0f)
        {
            //Create a GUI Style
            var style = new GUIStyle();
            //Set the fixed height of this container
            style.fixedHeight = 52f;
            //draw the background
            style.normal.background = Background;

            //using Begin Vertical as our container.
            GUILayout.BeginHorizontal(style);
            //Set the image in the container
            if (EditorGUIUtility.currentViewWidth < 375)
            {
                GUILayout.Label(LogoTex, GUILayout.MaxHeight(40), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
            }
            else
            {
                GUILayout.Label(LogoTex, GUILayout.MaxHeight(50), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
            }

            if (EditorGUIUtility.currentViewWidth > 375)
            {
                var dashboardStyle = PlayFabEditorHelper.GetTextButtonStyle();
                dashboardStyle.font = PlayFabEditorHelper.buttonFont;
                dashboardStyle.fontSize = 11;
                dashboardStyle.margin.top = 10;
                if (GUILayout.Button("GAME MANAGER", dashboardStyle))
                {
                    OnDashbaordClicked();
                }

                GUILayout.Space(5);

                var dashboardIconStyle = PlayFabEditorHelper.GetTextButtonStyle();
                dashboardIconStyle.normal.background = DashboardIcon;
                dashboardIconStyle.hover.background = DashboardIconHover;
                dashboardIconStyle.active.background = DashboardIconHover;
                dashboardIconStyle.margin.top = 5;
                dashboardIconStyle.margin.right = 5;
                if (GUILayout.Button("      ", dashboardIconStyle, GUILayout.MaxHeight(24), GUILayout.MaxWidth(24)))
                {
                    OnDashbaordClicked();
                }
            }
            else
            {
                var dashboardIconStyle = PlayFabEditorHelper.GetTextButtonStyle();
                dashboardIconStyle.normal.background = DashboardIcon;
                dashboardIconStyle.hover.background = DashboardIconHover;
                dashboardIconStyle.active.background = DashboardIconHover;
                dashboardIconStyle.margin.top = 5;
                dashboardIconStyle.margin.right = 5;
                if (GUILayout.Button("      ", dashboardIconStyle, GUILayout.MaxHeight(24), GUILayout.MaxWidth(24)))
                {
                    OnDashbaordClicked();
                }
            }


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

        private static void DrawProgressBar(float progress)
        {
                //create our container and get the rect back for positioning
                var r = EditorGUILayout.BeginVertical();
                //create a new rect that will be used as our progress bar
                var rect = new Rect(0, r.position.y, EditorGUIUtility.currentViewWidth, 10);
                //draw progress bar
                EditorGUI.ProgressBar(rect, progress, "");
                //end the container
                GUILayout.EndVertical();
        }

        private static void OnDashbaordClicked()
        {
            Debug.Log("Dashboard Clicked");
            var url = @"https://developer.playfab.com";
            Help.BrowseURL(url);
            //PlayFabWebWindow.OpenWindow(url);
        }

    }
}