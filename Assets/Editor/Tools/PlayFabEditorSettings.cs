using System;
using System.Text;

namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    public class PlayFabEditorSettings : Editor
    {
        public enum SubMenuStates
        {
            StandardSettings,
            ApiSettings
        }

        private static readonly GUIStyle GlobalButtonStyle = PlayFabEditorHelper.GetTextButtonStyle();
        internal static List<string> buildTargets;
        private const string AdminAPI = "ENABLE_PLAYFABADMIN_API";
        private const string ServerAPI = "ENABLE_PLAYFABSERVER_API";
        private const string ClientAPI = "DISABLE_PLAYFABCLIENT_API";
        private const string DebugRequestTiming = "PLAYFAB_REQUEST_TIMING";

        private static Texture2D CheckmarkIconOn =
            EditorGUIUtility.Load("Assets/Editor/images/checkmark_on.png") as Texture2D;
        private static Texture2D CheckmarkIconOff =
            EditorGUIUtility.Load("Assets/Editor/images/checkmark_off.png") as Texture2D;

        private static bool _isAdminSdkEnabled;
        private static bool _isServerSdkEnabled;
        private static bool _isClientSdkEnabled = true;
        private static bool _IsDebugRequestTiming;

        private static Vector3 _colorVector = PlayFabEditorHelper.GetColorVector(62);
        private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(_colorVector.x, _colorVector.y, _colorVector.z));
        private static Texture2D _textFieldBackground = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
        private static SubMenuStates _subMenuState;

        //Settings properties
        private static string _TitleId;
        private static WebRequestType _RequestType;
        private static int _RequestTimeOut;
        private static bool _KeepAlive;
        private static bool _CompressApiData;
        private static bool _EnableRealtimeLogging;
        private static string _LoggerHost;
        private static string _LoggerPort;
        private static int _LogCapLimit;


        public static PlayFabSharedSettings GetSharedSettingsObject()
        {
            var settingsList = Resources.FindObjectsOfTypeAll<PlayFabSharedSettings>();
            if (settingsList.Length != 1)
            {
                throw new Exception("Either Missing PlayFabSharedSettings data file or multiple data files exist.");
            }
            return settingsList[0];
        }

        public static void SetSettingsData()
        {
            _TitleId = _TitleId ?? PlayFabSettings.TitleId;
        }

        public static void LoadBaseTextures()
        {
            Background = Background ?? PlayFabEditorHelper.MakeTex(1, 1, new Color(_colorVector.x, _colorVector.y, _colorVector.z));
            _textFieldBackground = _textFieldBackground ?? PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
            CheckmarkIconOn = CheckmarkIconOn ?? EditorGUIUtility.Load("Assets/Editor/images/checkmark_on.png") as Texture2D;
            CheckmarkIconOff = CheckmarkIconOff ?? EditorGUIUtility.Load("Assets/Editor/images/checkmark_off.png") as Texture2D;
        }

        public static void DrawSettingsPanel()
        {
            LoadBaseTextures();
            //SetSettingsData();
            if (EditorPrefs.HasKey("PLAYFAB_CURRENT_SETTINGSMENU"))
            {
                _subMenuState = (SubMenuStates)EditorPrefs.GetInt("PLAYFAB_CURRENT_SETTINGSMENU");
            }

            //Create a GUI Style
            var menuStyle = new GUIStyle();
            //Set the fixed height of this container
            menuStyle.fixedHeight = 25f;
            menuStyle.margin.top = 10;
            menuStyle.normal.background = Background;
            var apiSettingsButtonStyle = PlayFabEditorHelper.GetTextButtonStyle();
            var standardSettingsButtonStyle = PlayFabEditorHelper.GetTextButtonStyle();

            if (_subMenuState == SubMenuStates.StandardSettings)
            {
                standardSettingsButtonStyle.normal = GlobalButtonStyle.active;
            }
            else
            {
                standardSettingsButtonStyle.normal = GlobalButtonStyle.normal;
            }

            if (_subMenuState == SubMenuStates.ApiSettings)
            {
                apiSettingsButtonStyle.normal = GlobalButtonStyle.active;
            }
            else
            {
                apiSettingsButtonStyle.normal = GlobalButtonStyle.normal;
            }

            GUILayout.BeginHorizontal(menuStyle);
            GUILayout.Space(5);
            if (GUILayout.Button("STANDARD SETTINGS", standardSettingsButtonStyle, GUILayout.MaxWidth(100)))
            {
                OnStandardSetttingsClicked();
            }
            GUILayout.Space(20);
            if (GUILayout.Button("API SETTINGS", apiSettingsButtonStyle, GUILayout.MaxWidth(100)))
            {
                OnApiSettingsClicked();
            }
            GUILayout.EndHorizontal();

            switch (_subMenuState)
            {
                case SubMenuStates.StandardSettings:
                    DrawStandardSettingsSubPanel();
                    break;
                case SubMenuStates.ApiSettings:
                    DrawApiSubPanel();
                    break;
            }


        }

        private static void OnApiSettingsClicked()
        {
            EditorPrefs.SetInt("PLAYFAB_CURRENT_SETTINGSMENU", (int)SubMenuStates.ApiSettings);
        }

        private static void OnStandardSetttingsClicked()
        {
            EditorPrefs.SetInt("PLAYFAB_CURRENT_SETTINGSMENU", (int)SubMenuStates.StandardSettings);
        }


        public static void DrawStandardSettingsSubPanel()
        {
            var style = PlayFabEditorHelper.GetTextButtonStyle();
            style.fixedHeight = 185;
            style.normal.background = Background;
            style.hover.background = Background;

            var textFieldStyle = PlayFabEditorHelper.GetTextButtonStyle();
            textFieldStyle.font = PlayFabEditorHelper.buttonFontBold;
            textFieldStyle.normal.background = _textFieldBackground;
            textFieldStyle.hover.background = _textFieldBackground;
            textFieldStyle.active.background = _textFieldBackground;

            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
            labelStyle.fontSize = 14;

            var toggleStyle = new GUIStyle();
            toggleStyle.normal.background = CheckmarkIconOff;
            toggleStyle.hover.background = CheckmarkIconOff;
            toggleStyle.active.background = CheckmarkIconOff;

            toggleStyle.onNormal.background = CheckmarkIconOn;
            toggleStyle.onHover.background = CheckmarkIconOn;
            toggleStyle.onActive.background = CheckmarkIconOn;

            toggleStyle.fixedHeight = 20;
            toggleStyle.fixedWidth = 20;

            GUILayout.Space(10);
            GUILayout.BeginVertical(style);
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            using (new FixedWidthLabel(new GUIContent("TITLE ID: "), labelStyle))
            {
                GUILayout.Space(40);
                _TitleId = EditorGUILayout.TextField(_TitleId, textFieldStyle, GUILayout.MinHeight(25));
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            using (new FixedWidthLabel(new GUIContent("REQUEST TIMEOUT: "), labelStyle))
            {
                GUILayout.Space(40);
                _RequestTimeOut = EditorGUILayout.IntField(_RequestTimeOut, textFieldStyle, GUILayout.MinHeight(25));
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            var keepAliveLabel = new GUIContent("KEEP ALIVE: ");
            using (new FixedWidthLabel(keepAliveLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(keepAliveLabel).x - 40);
                _KeepAlive = EditorGUILayout.Toggle(_KeepAlive, toggleStyle, GUILayout.MinHeight(25));
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            var buttonWidth = 100;
            GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth);

            var buttonStyle = PlayFabEditorHelper.GetButtonStyle();
            buttonStyle.font = PlayFabEditorHelper.buttonFontBold;
            buttonStyle.fontSize = 14;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.margin.right = 10;
            if (GUILayout.Button("SAVE", buttonStyle, GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
            {
                OnSaveSettings();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private static void OnSaveSettings()
        {

        }

        public static void DrawApiSubPanel()
        {
            var style = PlayFabEditorHelper.GetTextButtonStyle();
            style.fixedHeight = 165;
            style.normal.background = Background;
            style.hover.background = Background;

            var toggleStyle = new GUIStyle();
            toggleStyle.normal.background = CheckmarkIconOff;
            toggleStyle.hover.background = CheckmarkIconOff;
            toggleStyle.active.background = CheckmarkIconOff;

            toggleStyle.onNormal.background = CheckmarkIconOn;
            toggleStyle.onHover.background = CheckmarkIconOn;
            toggleStyle.onActive.background = CheckmarkIconOn;

            toggleStyle.fixedHeight = 20;
            toggleStyle.fixedWidth = 20;

            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
            labelStyle.fontSize = 14;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            GUILayout.Space(10);

            GUILayout.BeginVertical(style);
            GUILayout.Space(10);

            var clientLabel = new GUIContent("Enable Client API: ");
            using (new FixedWidthLabel(clientLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(clientLabel).x - 40);
                _isClientSdkEnabled = EditorGUILayout.Toggle(_isClientSdkEnabled, toggleStyle, GUILayout.MinHeight(25));
            }

            GUILayout.Space(10);

            var adminLabel = new GUIContent("Enable Admin API: ");
            using (new FixedWidthLabel(adminLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(adminLabel).x - 40);
                _isAdminSdkEnabled = EditorGUILayout.Toggle(_isAdminSdkEnabled, toggleStyle, GUILayout.MinHeight(25));
            }

            GUILayout.Space(10);

            var serverLabel = new GUIContent("Enable Server API: ");
            using (new FixedWidthLabel(serverLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(serverLabel).x - 40);
                _isServerSdkEnabled = EditorGUILayout.Toggle(_isServerSdkEnabled, toggleStyle, GUILayout.MinHeight(25));
            }

            GUILayout.Space(10);

            var debugRequestLabel = new GUIContent("Debug Request Times: ");
            using (new FixedWidthLabel(debugRequestLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(debugRequestLabel).x - 40);
                _IsDebugRequestTiming = EditorGUILayout.Toggle(_IsDebugRequestTiming, toggleStyle, GUILayout.MinHeight(25));

            }

            GUILayout.EndVertical();
        }



        public static void Update()
        {
            buildTargets = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            if (EditorPrefs.HasKey(AdminAPI))
            {
                _isAdminSdkEnabled = true;
            }
            if (EditorPrefs.HasKey(ServerAPI))
            {
                _isServerSdkEnabled = true;
            }
            if (EditorPrefs.HasKey(DebugRequestTiming))
            {
                _IsDebugRequestTiming = true;
            }
            if (EditorPrefs.HasKey(ClientAPI))
            {
                _isClientSdkEnabled = false;
            }
        }

        public static void After()
        {
            if (_isAdminSdkEnabled && !buildTargets.Contains(AdminAPI))
            {
                var str = AddToBuildTarget(buildTargets, AdminAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(AdminAPI, "1");
            }
            else if (!_isAdminSdkEnabled && buildTargets.Contains(AdminAPI))
            {
                var str = RemoveToBuildTarget(buildTargets, AdminAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(AdminAPI);
            }

            if (_isServerSdkEnabled && !buildTargets.Contains(ServerAPI))
            {
                var str = AddToBuildTarget(buildTargets, ServerAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(ServerAPI, "1");
            }
            else if (!_isServerSdkEnabled && buildTargets.Contains(ServerAPI))
            {
                var str = RemoveToBuildTarget(buildTargets, ServerAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(ServerAPI);
            }

            if (_IsDebugRequestTiming && !buildTargets.Contains(DebugRequestTiming))
            {
                var str = AddToBuildTarget(buildTargets, DebugRequestTiming);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(DebugRequestTiming, "1");
            }
            else if (!_IsDebugRequestTiming && buildTargets.Contains(DebugRequestTiming))
            {
                var str = RemoveToBuildTarget(buildTargets, DebugRequestTiming);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(DebugRequestTiming);
            }

            if (!_isClientSdkEnabled && !buildTargets.Contains(ClientAPI))
            {
                Debug.Log(ClientAPI + ":" + buildTargets.Contains(ClientAPI));
                var str = AddToBuildTarget(buildTargets, ClientAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(ClientAPI, "1");
            }
            else if (_isClientSdkEnabled && buildTargets.Contains(ClientAPI))
            {
                Debug.Log(ClientAPI + "- Removed");
                var str = RemoveToBuildTarget(buildTargets, ClientAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(ClientAPI);
            }

        }

        public static string AddToBuildTarget(List<string> targets, string define)
        {
            targets.Add(define);
            return string.Join(";", targets.ToArray());
        }

        public static string RemoveToBuildTarget(List<string> targets, string define)
        {
            targets.Remove(define);
            return string.Join(";", targets.ToArray());
        }


    }
}