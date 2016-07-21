namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorMenu : Editor
    {
        internal enum MenuStates
        {
            Services,
            Sdks,
            Settings,
            Logout,
            None
        }

        internal static MenuStates _menuState = MenuStates.Sdks;
        //private static readonly GUIStyle GlobalButtonStyle = PlayFabEditorHelper.GetTextButtonStyle();
        
        //Create a color vector for the background;
        //private static Vector3 ColorVector = PlayFabEditorHelper.GetColorVector(62);
        //private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVector.x, ColorVector.y, ColorVector.z));

        public static void DrawMenu()
        {
            if (EditorPrefs.HasKey("PLAYFAB_CURRENT_MENU"))
            {
                if (PlayFabEditorSDKTools.IsInstalled)
                {
                    _menuState = (MenuStates) EditorPrefs.GetInt("PLAYFAB_CURRENT_MENU");
                }
            }

            //Create a GUI Style
//            var style = new GUIStyle();
//            //Set the fixed height of this container
//            style.fixedHeight = 25f;
//            style.margin.top = 0;
//            style.normal.background = Background;

            //using Begin Vertical as our container.
            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.Height(25), GUILayout.ExpandWidth(true));
            var servicesButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");

            //TODO Move to state machine for states.
            if (_menuState == MenuStates.Services)
            {
                servicesButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                servicesButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

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

            GUILayout.Space(5);

            if (GUILayout.Button("SDK", sdksButtonStyle, GUILayout.MaxWidth(35)))
            {
                _menuState = MenuStates.Sdks;
                OnSdKsClicked();
            }

            GUILayout.Space(15);

            if (PlayFabEditorSDKTools.IsInstalled)
            {

                if (GUILayout.Button("SERVICES", servicesButtonStyle, GUILayout.MaxWidth(60)))
                {
                    _menuState = MenuStates.Services;
                    OnServicesClicked();
                }

                GUILayout.Space(15);

                if (GUILayout.Button("SETTINGS", settingsButtonStyle, GUILayout.MaxWidth(60)))
                {
                    _menuState = MenuStates.Settings;
                    OnSettingsClicked();
                }

                GUILayout.Space(15);
            }

            if (GUILayout.Button("LOGOUT", logoutButtonStyle, GUILayout.MaxWidth(55)))
            {
                _menuState = MenuStates.Logout;
                OnLogoutClicked();
            }

            GUILayout.EndHorizontal();
        }

        public static void OnServicesClicked()
        {
            Debug.Log("Services Clicked");
            EditorPrefs.SetInt("PLAYFAB_CURRENT_MENU",(int)MenuStates.Services);
        }

        public static void OnSdKsClicked()
        {
            Debug.Log("SDKS Clicked");
            EditorPrefs.SetInt("PLAYFAB_CURRENT_MENU", (int)MenuStates.Sdks);
        }

        public static void OnSettingsClicked()
        {
            Debug.Log("Settings Clicked");
            EditorPrefs.SetInt("PLAYFAB_CURRENT_MENU", (int)MenuStates.Settings);
        }

        public static void OnLogoutClicked()
        {
            Debug.Log("Logout Clicked");
            PlayFabEditorAuthenticate.Logout();
            _menuState = MenuStates.Sdks;
            EditorPrefs.SetInt("PLAYFAB_CURRENT_MENU", (int)MenuStates.Sdks);
        }

    }
}