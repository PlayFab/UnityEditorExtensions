using System.ComponentModel;
using System.Linq;
using PlayFab.Editor.EditorModels;
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
        private static bool _saveLogin = false;

        //Create Color vector for background.
        private static Vector3 ColorVector = PlayFabEditorHelper.GetColorVector(62);
        private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVector.x, ColorVector.y, ColorVector.z));

        public static bool IsAuthenticated()
        {
            return EditorPrefs.HasKey("IsPlayFabAuthenticated") && EditorPrefs.GetBool("IsPlayFabAuthenticated");
        }

        public static void DrawLogin()
        {
//            var style = PlayFabEditorHelper.GetTextButtonStyle();
//            style.fixedHeight = 135;
//            style.normal.background = Background;
//            style.hover.background = Background;

//            var textFieldStyle = PlayFabEditorHelper.GetTextButtonStyle();
//            textFieldStyle.font = PlayFabEditorHelper.buttonFontBold;
//            textFieldStyle.normal.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
//            textFieldStyle.hover.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
//            textFieldStyle.active.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));

//            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
//            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
//            labelStyle.fontSize = 14;
//            labelStyle.fixedHeight = 25f;
            float labelWidth = 100;

            GUILayout.BeginVertical();

             using (FixedWidthLabel fwl = new FixedWidthLabel("EMAIL:"))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userEmail = EditorGUILayout.TextField(_userEmail, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("PASSWORD:"))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userPass = EditorGUILayout.PasswordField(_userPass, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("SAVE LOGIN:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _saveLogin = EditorGUILayout.Toggle(_saveLogin, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")); //var buttonRect = 


//            var linkStyle = PlayFabEditorHelper.GetTextButtonStyle();
//            linkStyle.font = PlayFabEditorHelper.buttonFontBold;
//            linkStyle.fontSize = 11;
//            linkStyle.wordWrap = true;
//            linkStyle.alignment = TextAnchor.MiddleCenter;

            if(GUILayout.Button("CREATE AN ACCOUNT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(100) ))
            {
                DrawRegister();
            }

            var buttonWidth = 100;
            GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth * 2);

//            var buttonStyle = PlayFabEditorHelper.GetButtonStyle();
//            buttonStyle.font = PlayFabEditorHelper.buttonFontBold;
//            buttonStyle.fontSize = 14;
//            buttonStyle.alignment = TextAnchor.MiddleCenter;
//            buttonStyle.margin.right = 10;

            if (GUILayout.Button("LOGIN", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
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
            progressMax = 100;
            progressCount = 0f;
            startProgress = true;
            PlayFabEditorApi.Login(new LoginRequest()
            {
                DeveloperToolProductName = "PlayFabEditorExtension",  //TODO make this statics in a helper class
                DeveloperToolProductVersion = "1.01", //TODO make this statics in a helper class
                Email = _userEmail,
                Password = _userPass
            }, (result) =>
            {
                //Debug.Log(result.DeveloperClientToken);
                EditorPrefs.SetString("PlayFabUserEmail",_userEmail);
                EditorPrefs.SetString("PlayFabUserPass", _userPass);
                EditorPrefs.SetString("PlayFabDevClientToken",result.DeveloperClientToken);
                EditorPrefs.SetBool("IsPlayFabAuthenticated", true);

                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditor.Studios = getStudioResult.Studios.ToList();
                }, (getStudiosError) =>
                {
                    //TODO: Error Handling & have this update when the tab is opened.
                    Debug.LogError(getStudiosError.ToString());
                });

            }, (error) =>
            {
                progressCount = 0f;
                startProgress = false;
                Debug.LogError(error.ToString());
            });
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