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
        private static string _userPass2 = string.Empty;
        private static string _studio = string.Empty;
        private static bool _autoLogin = false;

        private static bool isLoggingIn = false;

        public enum PanelDisplayStates { Register, Login }
        private static PanelDisplayStates activeState = PanelDisplayStates.Register;


        public static bool IsAuthenticated()
        {
            return string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.devToken) ? false : true;
        }

        public static void DrawAuthPanels()
        {
            if(!string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.email))
            {
                _userEmail = PlayFabEditorDataService.accountDetails.email;
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                GUILayout.Label("Welcome to PlayFab!", PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
            GUILayout.EndHorizontal();

            if(PlayFabEditorDataService.accountDetails.useAutoLogin)
            {
                if(!string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.devToken))
                {
                    _autoLogin = true;
                    if(isLoggingIn == false)
                    {
                        Debug.Log("PlayFab developer credentials saved, logging in...");
                        OnLoginButtonClicked();
                    }
                }
                else
                {
                    // lost password and token, cannot auto login
                    activeState = PanelDisplayStates.Login;
                }
            }
            else if(activeState == PanelDisplayStates.Login)
            {
                // login mode, this state either logged out, or did not have auto-login checked.
                DrawLogin();

            }
            else if (activeState == PanelDisplayStates.Register)
            {
                // register mode 
                DrawRegister();
            }
            else
            {
                DrawRegister();
            }

                

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
            GUILayout.Label("Our editor extension provides an easy way to manage the PlayFab SDK. \n\nExisting users may LOG IN using their developer account. \n\nNew users must CREATE AN ACCOUNT.", PlayFabEditorHelper.uiStyle.GetStyle("cGenTxt"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));



            GUILayout.EndVertical();
        }



        public static void DrawLogin()
        {

            float labelWidth = 120;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

             using (FixedWidthLabel fwl = new FixedWidthLabel("EMAIL: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userEmail = EditorGUILayout.TextField(_userEmail, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("PASSWORD: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userPass = EditorGUILayout.PasswordField(_userPass, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("AUTO-LOGIN: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _autoLogin = EditorGUILayout.Toggle(_autoLogin, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")); //var buttonRect = 


            if(GUILayout.Button("CREATE AN ACCOUNT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(100) ))
            {
                activeState = PanelDisplayStates.Register;
            }

            var buttonWidth = 100;
            GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth * 2);

            if (GUILayout.Button("LOG IN", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
            {
               
                OnLoginButtonClicked();
            }


            GUILayout.EndHorizontal();


            GUILayout.EndVertical();



        }

        public static void Logout()
        {
            _autoLogin = false;
            _userPass = string.Empty;
            _userPass2 = string.Empty;

            activeState = PanelDisplayStates.Login;

            PlayFabEditorDataService.accountDetails.useAutoLogin = false;
            PlayFabEditorDataService.accountDetails.devToken = string.Empty;
            PlayFabEditorDataService.SaveAccountDetails();

            //TODO make sure to clean up login tokens.
        }

        public static void DrawRegister()
        {
            float labelWidth = 150;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

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

            using (FixedWidthLabel fwl = new FixedWidthLabel("CONFIRM PASSWORD:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userPass2 = EditorGUILayout.PasswordField(_userPass2, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("STUDIO NAME:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _studio = EditorGUILayout.TextField(_studio, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")); //var buttonRect = 

            if (GUILayout.Button("LOG IN", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32)))
            {
                activeState = PanelDisplayStates.Login;
            }

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("  CREATE AN ACCOUNT  ", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
            {
               OnRegisterClicked();
            }

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }

  
        
        private static void OnLoginButtonClicked()
        {
            isLoggingIn = true;

            PlayFabEditorApi.Login(new LoginRequest()
            {
                DeveloperToolProductName = "PlayFabEditorExtension",  //TODO make this statics in a helper class
                DeveloperToolProductVersion = PlayFabEditor.edexVersion, //TODO make this statics in a helper class
                Email = _userEmail,
                Password = _userPass
            }, (result) =>
            {
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;
                PlayFabEditorDataService.accountDetails.useAutoLogin = _autoLogin;

                isLoggingIn = false;

                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                }, (getStudiosError) =>
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, getStudiosError.ToString());
                });
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

            }, (error) =>
            {
                isLoggingIn = false;
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, error.ErrorMessage);
            });
        }

        private static void OnRegisterClicked()
        {
            if(_userPass != _userPass2)
            {
                Debug.LogError("PlayFab developer account passwords must match.");
                return;
            }


            isLoggingIn = true;

            PlayFabEditorApi.RegisterAccouint(new RegisterAccountRequest()
            {
                DeveloperToolProductName = "PlayFabEditorExtension",  //TODO make this statics in a helper class
                DeveloperToolProductVersion = PlayFabEditor.edexVersion, //TODO make this statics in a helper class
                Email = _userEmail,
                Password = _userPass,
                StudioName = _studio
            }, (result) =>
            {
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;
                PlayFabEditorDataService.accountDetails.useAutoLogin = _autoLogin;
                 
                isLoggingIn = false;
                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                }, (getStudiosError) =>
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, getStudiosError.ToString());
                });
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

 

            }, (error) =>
            {
                isLoggingIn = false;
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, error.ErrorMessage);
            });
        }

        internal static void Update()
        {

        }

    }
}