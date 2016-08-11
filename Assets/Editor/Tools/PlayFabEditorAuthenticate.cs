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
            //return EditorPrefs.HasKey("IsPlayFabAuthenticated") && EditorPrefs.GetBool("IsPlayFabAuthenticated");
            return string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.devToken) ? false : true;
        }

        public static void DrawAuthPanels()
        {
            if(!string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.email))
            {
                _userEmail = PlayFabEditorDataService.accountDetails.email;
            }

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
//                if(EditorPrefs.HasKey("PlayFabDevClientToken") && !string.IsNullOrEmpty(EditorPrefs.GetString("PlayFabDevClientToken")))
//                {
//                    EditorPrefs.SetBool("IsPlayFabAuthenticated", true);
//                    return;
//                }

               // auto login mode, first fetch cached password 


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

            if (GUILayout.Button("LOGIN", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
            {
               
                OnLoginButtonClicked();
            }


            GUILayout.EndHorizontal();


            GUILayout.EndVertical();



        }

        public static void Logout()
        {
            //EditorPrefs.SetBool("IsPlayFabAuthenticated", false);
            _autoLogin = false;
            _userPass = string.Empty;
            _userPass2 = string.Empty;

            activeState = PanelDisplayStates.Login;

            PlayFabEditorDataService.accountDetails.useAutoLogin = false;
            PlayFabEditorDataService.accountDetails.devToken = string.Empty;
            PlayFabEditorDataService.SaveAccountDetails();


            //EditorPrefs.SetBool("PlayFabAutoLogin", false);

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

            if (GUILayout.Button("LOGIN", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32)))
            {
                activeState = PanelDisplayStates.Login;
            }


            var buttonWidth = 100;
            GUILayout.FlexibleSpace();

            if(GUILayout.Button("  CREATE AN ACCOUNT  ", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
            {
               OnRegisterClicked();
            }
 


            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }


        private static bool startProgress;
        private static float progressCount;
        private static float progressMax;
        
        private static void OnLoginButtonClicked()
        {
            progressMax = 100;
            progressCount = 0f;
            startProgress = true;

            isLoggingIn = true;

            PlayFabEditorApi.Login(new LoginRequest()
            {
                DeveloperToolProductName = "PlayFabEditorExtension",  //TODO make this statics in a helper class
                DeveloperToolProductVersion = "1.01", //TODO make this statics in a helper class
                Email = _userEmail,
                Password = _userPass
            }, (result) =>
            {
                //Debug.Log(result.DeveloperClientToken);
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;
                PlayFabEditorDataService.accountDetails.useAutoLogin = _autoLogin;



//                EditorPrefs.SetString("PlayFabUserEmail",_userEmail);
//                EditorPrefs.SetString("PlayFabUserPass", _userPass);
//                EditorPrefs.SetString("PlayFabDevClientToken",result.DeveloperClientToken);
//                EditorPrefs.SetBool("IsPlayFabAuthenticated", true);
//                EditorPrefs.SetBool("PlayFabAutoLogin", _autoLogin);
                isLoggingIn = false;

                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                }, (getStudiosError) =>
                {
                    //TODO: Error Handling & have this update when the tab is opened.
                    Debug.LogError(getStudiosError.ToString());
                });
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

            }, (error) =>
            {
                progressCount = 0f;
                startProgress = false;
                isLoggingIn = false;
                Debug.LogError(error.ToString());
            });
        }

        private static void OnRegisterClicked()
        {
            if(_userPass != _userPass2)
            {
                Debug.LogError("PlayFab developer account passwords must match.");
                return;
            }

            progressMax = 100;
            progressCount = 0f;
            startProgress = true;

            isLoggingIn = true;

            PlayFabEditorApi.RegisterAccouint(new RegisterAccountRequest()
            {
                DeveloperToolProductName = "PlayFabEditorExtension",  //TODO make this statics in a helper class
                DeveloperToolProductVersion = "1.01", //TODO make this statics in a helper class
                Email = _userEmail,
                Password = _userPass,
                StudioName = _studio
            }, (result) =>
            {
                //Debug.Log(result.DeveloperClientToken);
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;
                PlayFabEditorDataService.accountDetails.useAutoLogin = _autoLogin;

//                EditorPrefs.SetString("PlayFabUserEmail",_userEmail);
//                EditorPrefs.SetString("PlayFabUserPass", _userPass);
//                EditorPrefs.SetString("PlayFabDevClientToken",result.DeveloperClientToken);
//                EditorPrefs.SetBool("IsPlayFabAuthenticated", true);
//                EditorPrefs.SetBool("PlayFabAutoLogin", _autoLogin);

                isLoggingIn = false;
                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                }, (getStudiosError) =>
                {
                    //TODO: Error Handling & have this update when the tab is opened.
                    Debug.LogError(getStudiosError.ToString());
                });
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

                //TODO add some help here since a new account was just created... will be added with the FTUE

            }, (error) =>
            {
                progressCount = 0f;
                startProgress = false;
                isLoggingIn = false;
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