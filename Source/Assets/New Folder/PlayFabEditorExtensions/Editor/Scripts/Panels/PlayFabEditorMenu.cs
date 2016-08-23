namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorMenu : Editor
    {
    #region panel variables
        internal enum MenuStates
        {
            Data,
            Services,
            Sdks,
            Settings,
            Help,
            Logout,
            None
        }

        internal static MenuStates _menuState = MenuStates.Sdks;
    #endregion

        public static void DrawMenu()
        {
            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {
                _menuState = (MenuStates) PlayFabEditorDataService.editorSettings.currentMainMenu;
            }

            var sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var dataButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var helpButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");


            if (_menuState == MenuStates.Sdks)
            {
                sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Settings)
            {
                settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Logout)
            {
                logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Data)
            {
                dataButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                dataButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Help)
            {
                helpButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                helpButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.Height(25), GUILayout.ExpandWidth(true));
   
            GUILayout.Space(5);

            if (GUILayout.Button("SDK", sdksButtonStyle, GUILayout.MaxWidth(35)))
            {
                _menuState = MenuStates.Sdks;
                OnSdKsClicked();
            }


            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {

                if (GUILayout.Button("DATA", dataButtonStyle, GUILayout.MaxWidth(60)))
                {
                    _menuState = MenuStates.Data;
                    OnDataClicked();
                }

                if (GUILayout.Button("SETTINGS", settingsButtonStyle, GUILayout.MaxWidth(60)))
                {
                    _menuState = MenuStates.Settings;
                    OnSettingsClicked();
                }

            }

            if (GUILayout.Button("HELP", helpButtonStyle, GUILayout.MaxWidth(60)))
                {
                    _menuState = MenuStates.Help;
                    OnHelpClicked();
                   
                }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("LOGOUT", logoutButtonStyle, GUILayout.MaxWidth(55)))
            {
                _menuState = MenuStates.Logout;
                OnLogoutClicked();
            }

            GUILayout.EndHorizontal();
        }



        public static void OnDataClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Data.ToString());

            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Data;
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnHelpClicked()
        {

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Help.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Help;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnServicesClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Services.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Services;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnSdKsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Sdks.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Sdks;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnSettingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Settings.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Settings;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnLogoutClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Logout.ToString());

            PlayFabEditorAuthenticate.Logout();


            _menuState = MenuStates.Sdks;

            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Sdks;      
            PlayFabEditorDataService.SaveEditorSettings();
        }
    }
}