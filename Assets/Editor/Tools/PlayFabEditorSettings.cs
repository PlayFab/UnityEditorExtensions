namespace PlayFab.Editor
{
    using System;
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

        public enum WebRequestType
        {
            UnityWww, // High compatability Unity api calls
            HttpWebRequest // High performance multi-threaded api calls
        }

        private static readonly GUIStyle GlobalButtonStyle = PlayFabEditorHelper.GetTextButtonStyle();
        internal static List<string> buildTargets;
        private const string AdminAPI = "ENABLE_PLAYFABADMIN_API";
        private const string ServerAPI = "ENABLE_PLAYFABSERVER_API";
        private const string ClientAPI = "DISABLE_PLAYFABCLIENT_API";
        private const string DebugRequestTiming = "PLAYFAB_REQUEST_TIMING";

//        private static Texture2D CheckmarkIconOn =
//            EditorGUIUtility.Load("Assets/Editor/images/checkmark_on.png") as Texture2D;
//        private static Texture2D CheckmarkIconOff =
//            EditorGUIUtility.Load("Assets/Editor/images/checkmark_off.png") as Texture2D;

        private static bool _isAdminSdkEnabled;
        private static bool _isServerSdkEnabled;
        private static bool _isClientSdkEnabled = true;
        private static bool _IsDebugRequestTiming;

//        private static Vector3 _colorVector = PlayFabEditorHelper.GetColorVector(62);
//        private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(_colorVector.x, _colorVector.y, _colorVector.z));
//        private static Texture2D _textFieldBackground = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
        private static SubMenuStates _subMenuState;

        //Settings properties
        private static string _TitleId;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
        private static string _DeveloperSecretKey;
#endif
        private static WebRequestType _RequestType;
        private static int _RequestTimeOut;
        private static bool _KeepAlive;
        private static bool _CompressApiData;
        private static bool _EnableRealtimeLogging;
        private static string _LoggerHost;
        private static string _LoggerPort;
        private static int _LogCapLimit;

        public static void LoadBaseTextures()
        {
//            Background = Background ?? PlayFabEditorHelper.MakeTex(1, 1, new Color(_colorVector.x, _colorVector.y, _colorVector.z));
//            _textFieldBackground = _textFieldBackground ?? PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
//            CheckmarkIconOn = CheckmarkIconOn ?? EditorGUIUtility.Load("Assets/Editor/images/checkmark_on.png") as Texture2D;
//            CheckmarkIconOff = CheckmarkIconOff ?? EditorGUIUtility.Load("Assets/Editor/images/checkmark_off.png") as Texture2D;
        }

        private static bool _isSettingsSet = false;
        public static void SetSettingsProperties()
        {
            if (PlayFabEditorSDKTools.IsInstalled && !_isSettingsSet)
            {
                var playfabSettingsType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == "PlayFabSettings"
                    select type);
                if (playfabSettingsType.ToList().Count > 0)
                {
                    var type = playfabSettingsType.ToList().FirstOrDefault();
                    var fields = type.GetFields();
                    var props = type.GetProperties();

                    _TitleId = (string) props.ToList().Find(p => p.Name == "TitleId").GetValue(null, null) ?? string.Empty; 
                    _RequestType = (WebRequestType) props.ToList().Find(p => p.Name == "RequestType").GetValue(null, null);
                    _RequestTimeOut = (int) props.ToList().Find(p => p.Name == "RequestTimeout").GetValue(null, null);
                    _KeepAlive = (bool) props.ToList().Find(p => p.Name == "RequestKeepAlive").GetValue(null, null);
                    _CompressApiData = (bool)props.ToList().Find(p => p.Name == "CompressApiData").GetValue(null, null);
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                    _DeveloperSecretKey = (string) props.ToList().Find(p => p.Name == "DeveloperSecretKey").GetValue(null, null) ?? string.Empty;
#endif
                    _isSettingsSet = true; 
                }
            }
            
        }

        public static void DrawSettingsPanel()
        {
            SetSettingsProperties();
            LoadBaseTextures();
            //SetSettingsData();
            if (EditorPrefs.HasKey("PLAYFAB_CURRENT_SETTINGSMENU"))
            {
                _subMenuState = (SubMenuStates)EditorPrefs.GetInt("PLAYFAB_CURRENT_SETTINGSMENU");
            }

            //Create a GUI Style
//            var menuStyle = new GUIStyle();
//            //Set the fixed height of this container
//            menuStyle.fixedHeight = 25f;
//            menuStyle.margin.top = 10;
//            menuStyle.normal.background = Background;

            var apiSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var standardSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");

            if (_subMenuState == SubMenuStates.StandardSettings)
            {
                standardSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                standardSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_subMenuState == SubMenuStates.ApiSettings)
            {
                apiSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                apiSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("STANDARD SETTINGS", standardSettingsButtonStyle))
            {
                OnStandardSetttingsClicked();
            }
            if (GUILayout.Button("API SETTINGS", apiSettingsButtonStyle))
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
//            var style = PlayFabEditorHelper.GetTextButtonStyle();
//            style.fixedHeight = 250;
//            style.normal.background = Background;
//            style.hover.background = Background;

//            var textFieldStyle = PlayFabEditorHelper.GetTextButtonStyle();
//            textFieldStyle.font = PlayFabEditorHelper.buttonFontBold;
//            textFieldStyle.normal.background = _textFieldBackground;
//            textFieldStyle.hover.background = _textFieldBackground;
//            textFieldStyle.active.background = _textFieldBackground;

//            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
//            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
//            labelStyle.fontSize = 14;
//            labelStyle.fixedHeight = 25f;
//
//            var toggleStyle = new GUIStyle();
//            toggleStyle.normal.background = CheckmarkIconOff;
//            toggleStyle.hover.background = CheckmarkIconOff;
//            toggleStyle.active.background = CheckmarkIconOff;
//
//            toggleStyle.onNormal.background = CheckmarkIconOn;
//            toggleStyle.onHover.background = CheckmarkIconOn;
//            toggleStyle.onActive.background = CheckmarkIconOn;
//
//            toggleStyle.fixedHeight = 20;
//            toggleStyle.fixedWidth = 20;

            float labelWidth = 160;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true));
           

            //GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("TITLE ID: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                _TitleId = EditorGUILayout.TextField(_TitleId, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            GUILayout.EndHorizontal();



//            using (new FixedWidthLabel(new GUIContent("TITLE ID: "), labelStyle))
//            {
//                GUILayout.Space(40);
//                _TitleId = EditorGUILayout.TextField(_TitleId, textFieldStyle, GUILayout.MinHeight(25));
//            }
//            GUILayout.Space(10);
//            GUILayout.EndHorizontal();

//            GUILayout.Space(10);

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("DEVELOPER SECRET KEY: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
            _DeveloperSecretKey = EditorGUILayout.TextField(_DeveloperSecretKey, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            using (new FixedWidthLabel(new GUIContent("DEVELOPER SECRET KEY: "), labelStyle))
//            {
//                GUILayout.Space(40);
//                _DeveloperSecretKey = EditorGUILayout.TextField(_DeveloperSecretKey, textFieldStyle, GUILayout.MinHeight(25));
//            }
//            GUILayout.Space(10);
//            GUILayout.EndHorizontal();
//
//            GUILayout.Space(10);
            #endif

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("REQUEST TYPE: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
                _RequestType = (WebRequestType) EditorGUILayout.EnumPopup(_RequestType, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.Height(25));
            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();

//            using (new FixedWidthLabel(new GUIContent("REQUEST TYPE: "), labelStyle))
//            {
//                GUILayout.Space(40);
//                _RequestType = (WebRequestType) EditorGUILayout.EnumPopup(_RequestType, textFieldStyle, GUILayout.MinHeight(25)); //.TextField(_TitleId, textFieldStyle, GUILayout.MinHeight(25));
//            }
//            GUILayout.Space(10);
//            GUILayout.EndHorizontal();

            if (_RequestType == WebRequestType.HttpWebRequest)
            {
                using (FixedWidthLabel fwl = new FixedWidthLabel(new GUIContent("REQUEST TIMEOUT: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    _RequestTimeOut = EditorGUILayout.IntField(_RequestTimeOut, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                }

                using (FixedWidthLabel fwl = new FixedWidthLabel(new GUIContent("KEEP ALIVE: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    _KeepAlive = EditorGUILayout.Toggle(_KeepAlive, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }
            }


            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("COMPRESS API DATA: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
                _CompressApiData = EditorGUILayout.Toggle(_CompressApiData, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            GUILayout.EndHorizontal();
           

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                var buttonWidth = 100;
                GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth);

//                var buttonStyle = PlayFabEditorHelper.GetButtonStyle();
//                buttonStyle.font = PlayFabEditorHelper.buttonFontBold;
//                buttonStyle.fontSize = 14;
//                buttonStyle.alignment = TextAnchor.MiddleCenter;
//                buttonStyle.margin.right = 10;
                if (GUILayout.Button("SAVE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
                {
                    OnSaveSettings();
                }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private static void OnSaveSettings()
        {
            //#if ENABLE_PLAYFABCLIENT_API || ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
            //#endif

            Debug.Log("Save Settings Clicked");
            if (PlayFabEditorSDKTools.IsInstalled)
            {
                var playfabSettingsType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == "PlayFabSettings"
                    select type);
                if (playfabSettingsType.ToList().Count > 0)
                {
                    var type = playfabSettingsType.ToList().FirstOrDefault();
                    var fields = type.GetFields();
                    var props = type.GetProperties();

                    foreach (var property in props)
                    {
                        //Debug.Log(property.Name);
                        if (property.Name.ToLower() == "titleid")
                        {
                            property.SetValue(null, _TitleId, null);
                        }
                        if (property.Name.ToLower() == "requesttype")
                        {
                            property.SetValue(null, (int)_RequestType, null);
                        }
                        if (property.Name.ToLower() == "timeout")
                        {
                            property.SetValue(null, _RequestTimeOut, null);
                        }
                        if (property.Name.ToLower() == "requestkeepalive")
                        {
                            property.SetValue(null, _KeepAlive, null);
                        }
                        if (property.Name.ToLower() == "compressapidata")
                        {
                            property.SetValue(null, _CompressApiData, null);
                        }
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                        if (property.Name.ToLower() == "developersecretkey")
                        {
                            property.SetValue(null, _DeveloperSecretKey, null);
                        }
#endif
                    }

                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                Debug.Log("SDK is not installed.");
            }
        }



        public static void DrawApiSubPanel()
        {
//            var style = PlayFabEditorHelper.GetTextButtonStyle();
//            style.fixedHeight = 165;
//            style.normal.background = Background;
//            style.hover.background = Background;
//
//            var toggleStyle = new GUIStyle();
//            toggleStyle.normal.background = CheckmarkIconOff;
//            toggleStyle.hover.background = CheckmarkIconOff;
//            toggleStyle.active.background = CheckmarkIconOff;
//
//            toggleStyle.onNormal.background = CheckmarkIconOn;
//            toggleStyle.onHover.background = CheckmarkIconOn;
//            toggleStyle.onActive.background = CheckmarkIconOn;
//
//            toggleStyle.fixedHeight = 20;
//            toggleStyle.fixedWidth = 20;
//
//            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
//            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
//            labelStyle.fontSize = 14;
//            labelStyle.alignment = TextAnchor.MiddleLeft;

//            GUILayout.Space(10);

            float labelWidth = 160;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
//            GUILayout.Space(10);

            //var clientLabel = new GUIContent("Enable Client API: ");
            using (FixedWidthLabel fwl = new FixedWidthLabel("Enable Client API: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _isClientSdkEnabled = EditorGUILayout.Toggle(_isClientSdkEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("Enable Admin API:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _isAdminSdkEnabled = EditorGUILayout.Toggle(_isAdminSdkEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("Enable Server API: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _isServerSdkEnabled = EditorGUILayout.Toggle(_isServerSdkEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("Debug Request Times: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _IsDebugRequestTiming = EditorGUILayout.Toggle(_IsDebugRequestTiming, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
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