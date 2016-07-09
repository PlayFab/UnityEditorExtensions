using System.ComponentModel;
using UnityEngine.UI;

namespace PlayFab.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    public class PlayFabEditorAuthenticate : Editor
    {
        private static string _userEmail = string.Empty;
        private static string _userPass = string.Empty;

        //Create Color vector for background.
        private static Vector3 ColorVector = PlayFabEditorHelper.GetColorVector(62);
        private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVector.x, ColorVector.y, ColorVector.z));

        public static bool IsAuthenticated()
        {
            return EditorPrefs.HasKey("IsPlayFabAuthenticated") && EditorPrefs.GetBool("IsPlayFabAuthenticated");
        }

        public static void DrawLogin()
        {
            var style = PlayFabEditorHelper.GetTextButtonStyle();
            style.fixedHeight = 135;
            style.normal.background = Background;
            style.hover.background = Background;

            var textFieldStyle = PlayFabEditorHelper.GetTextButtonStyle();
            textFieldStyle.font = PlayFabEditorHelper.buttonFontBold;
            textFieldStyle.normal.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
            textFieldStyle.hover.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
            textFieldStyle.active.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));

            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
            labelStyle.fontSize = 14;
            labelStyle.fixedHeight = 25f;
            GUILayout.Space(10);

            GUILayout.BeginVertical(style);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            
            using (new FixedWidthLabel(new GUIContent("EMAIL: "), labelStyle))
            {
                GUILayout.Space(40);
                _userEmail = EditorGUILayout.TextField(_userEmail,textFieldStyle, GUILayout.MinHeight(25));
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            using (new FixedWidthLabel(new GUIContent("PASSWORD: "), labelStyle))
            {
                GUILayout.Space(5);
                _userPass = EditorGUILayout.PasswordField(_userPass, textFieldStyle, GUILayout.MinHeight(25));
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            EditorGUILayout.BeginHorizontal(); //var buttonRect = 
            var buttonWidth = 100;

            var linkStyle = PlayFabEditorHelper.GetTextButtonStyle();
            linkStyle.font = PlayFabEditorHelper.buttonFontBold;
            linkStyle.fontSize = 11;
            linkStyle.wordWrap = true;
            linkStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Button("CREATE AN ACCOUNT", linkStyle, GUILayout.MaxWidth(100) );

            GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth * 2);

            var buttonStyle = PlayFabEditorHelper.GetButtonStyle();
            buttonStyle.font = PlayFabEditorHelper.buttonFontBold;
            buttonStyle.fontSize = 14;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.margin.right = 10;
            if (GUILayout.Button("LOGIN", buttonStyle, GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
            {
                OnLoginButtonClicked();
            }


            GUILayout.EndHorizontal();


            GUILayout.EndVertical();



        }

        public static void Logout()
        {
            EditorPrefs.SetBool("IsPlayFabAuthenticated", false);
        }

        public static void DrawRegister()
        {
            
        }


        private static bool startProgress;
        private static float progressCount;
        private static float progressMax;
        
        private static void OnLoginButtonClicked()
        {
            EditorPrefs.SetBool("IsPlayFabAuthenticated", true);
            progressMax = 100;
            progressCount = 0f;
            startProgress = true;
        }

        internal static void Update()
        {
            if (startProgress)
            {
                PlayFabEditor.Progress = progressCount/progressMax;
                progressCount++;
                if (progressCount >= progressMax)
                {
                    progressCount = 0f;
                    startProgress = false;
                }
            }
        }

    }
}