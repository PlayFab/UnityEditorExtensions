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
            ApiSettings,
            TitleSettings,
            Packages
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

        private static bool _isAdminSdkEnabled;
        private static bool _isServerSdkEnabled;
        private static bool _isClientSdkEnabled = true;
        private static bool _IsDebugRequestTiming;

        private static SubMenuStates _subMenuState;

        //Settings properties
        private static string _TitleId;

        private static string[] titleOptions;
        private static string[] studioOptions;
       
        private static int _selectedTitleIdIndex = 0;
        private static int _selectedStudioIndex = 0;
        private static int _prevSelectedTitleIdIndex = 0;
        private static int _prevSelectedStudioIndex = 0;


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
            if (EditorPrefs.HasKey("PLAYFAB_CURRENT_SETTINGSMENU"))
            {
                _subMenuState = (SubMenuStates)EditorPrefs.GetInt("PLAYFAB_CURRENT_SETTINGSMENU");
            }

            var apiSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var standardSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var titleSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var packagesSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");

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

            if (_subMenuState == SubMenuStates.TitleSettings)
            {
                titleSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                titleSettingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("PROJECT", standardSettingsButtonStyle))
            {
                OnStandardSetttingsClicked();
            }
            if (GUILayout.Button("TITLE", titleSettingsButtonStyle))
            {
                OnTitleSettingsClicked();
            }

            if (GUILayout.Button("API", apiSettingsButtonStyle))
            {
                OnApiSettingsClicked();
            }

            if (GUILayout.Button("PACKAGES", PlayFabEditorHelper.uiStyle.GetStyle("textButton")))
            {
                //OnStandardSetttingsClicked();
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
                 case SubMenuStates.TitleSettings:
                    DrawTitleSettingsSubPanel();
                    break;
                 case SubMenuStates.Packages:
                    //DrawPackagesSubPanel();
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

        private static void OnTitleSettingsClicked()
        {
            EditorPrefs.SetInt("PLAYFAB_CURRENT_SETTINGSMENU", (int)SubMenuStates.TitleSettings);
        }


        //
        private static Dictionary<string, StudioDisplaySet > studioFoldOutStates = new Dictionary<string, StudioDisplaySet>();
        private static Vector2 TitleScrollPos = Vector2.zero;

        public static void DrawTitleSettingsSubPanel()
        {
            float labelWidth = 100;

            // this probably does not need to run every update.

            if(PlayFabEditor.Studios.Count != studioFoldOutStates.Count)
            {
                studioFoldOutStates.Clear();
                foreach(var studio in PlayFabEditor.Studios)
                {
                    if(!studioFoldOutStates.ContainsKey(studio.Id))
                    {
                        studioFoldOutStates.Add(studio.Id, new StudioDisplaySet(){ Studio = studio });
                    }

                    foreach(var title in studio.Titles)
                    {
                        // studioFoldOutStates[studio.Id].titleFoldOutStates
                        if(!studioFoldOutStates[studio.Id].titleFoldOutStates.ContainsKey(title.Id))
                        {
                            studioFoldOutStates[studio.Id].titleFoldOutStates.Add(title.Id, new TitleDisplaySet(){ Title = title });
                        }
                    }
                }
             }

            
            TitleScrollPos = GUILayout.BeginScrollView(TitleScrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("STUDIOS:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("REFRESH", PlayFabEditorHelper.uiStyle.GetStyle("Button")))
                {
                    PlayFabEditorApi.GetStudios(new PlayFab.Editor.EditorModels.GetStudiosRequest(), (getStudioResult) =>
                    {
                        PlayFabEditor.Studios = getStudioResult.Studios.ToList();
                    }, (getStudiosError) =>
                    {
                        //TODO: Error Handling & have this update when the tab is opened.
                        Debug.LogError(getStudiosError.ToString());
                    });
                }
            GUILayout.EndHorizontal(); 

            foreach(var studio in studioFoldOutStates)
            {
                //Foldout(EditorGUILayout.GetControlRect(), commonFoldout, "Common issues", true);
                var style = new GUIStyle(EditorStyles.foldout);

                if(studio.Value.isCollapsed)
                {
                    style.fontStyle = FontStyle.Normal;
                }

                studio.Value.isCollapsed = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), studio.Value.isCollapsed, string.Format("{0} ({1})", studio.Value.Studio.Name, studio.Value.Studio.Titles.Length), true, style);

                if(!studio.Value.isCollapsed)
                {
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.LabelField("TITLES:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                    // draw title foldouts
                    foreach(var title in studio.Value.titleFoldOutStates)
                    {
                        title.Value.isCollapsed = EditorGUILayout.Foldout(title.Value.isCollapsed, string.Format("{0} [{1}]", title.Value.Title.Name, title.Value.Title.Id));
                       
                        if(! title.Value.isCollapsed)
                        {
                            EditorGUI.indentLevel = 2;
                            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                                EditorGUILayout.LabelField("SECRET KEY:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                                EditorGUILayout.TextField(""+title.Value.Title.SecretKey);
                            GUILayout.EndHorizontal();   

                            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                                EditorGUILayout.LabelField("URL:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                                EditorGUILayout.TextField(""+title.Value.Title.GameManagerUrl);
                            GUILayout.EndHorizontal();  
                            EditorGUI.indentLevel = 1;
                        }
                    }

                    EditorGUI.indentLevel = 0;
                }
            }




            //TODO START BACK HERE
           // EditorGUILayout.Foldout

//            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
//                EditorGUILayout.LabelField("TITLE ID: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
//                _TitleId = EditorGUILayout.TextField(_TitleId, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
//            GUILayout.EndHorizontal();


//            #if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
//                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
//                    EditorGUILayout.LabelField("DEVELOPER SECRET KEY: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
//            _DeveloperSecretKey = EditorGUILayout.TextField(_DeveloperSecretKey, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
//                GUILayout.EndHorizontal();
//
//            #endif
//
//            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
//                EditorGUILayout.LabelField("REQUEST TYPE: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
//                _RequestType = (WebRequestType) EditorGUILayout.EnumPopup(_RequestType, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.Height(25));
//            GUILayout.EndHorizontal();
//
//
//            if (_RequestType == WebRequestType.HttpWebRequest)
//            {
//                using (FixedWidthLabel fwl = new FixedWidthLabel(new GUIContent("REQUEST TIMEOUT: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
//                {
//                    GUILayout.Space(labelWidth - fwl.fieldWidth);
//                    _RequestTimeOut = EditorGUILayout.IntField(_RequestTimeOut, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
//                }
//
//                using (FixedWidthLabel fwl = new FixedWidthLabel(new GUIContent("KEEP ALIVE: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
//                {
//                    GUILayout.Space(labelWidth - fwl.fieldWidth);
//                    _KeepAlive = EditorGUILayout.Toggle(_KeepAlive, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
//                }
//            }
//
//
//            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
//                EditorGUILayout.LabelField("COMPRESS API DATA: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
//                _CompressApiData = EditorGUILayout.Toggle(_CompressApiData, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
//            GUILayout.EndHorizontal();
//           
//
//            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
//                var buttonWidth = 100;
//                GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth);
//
//                if (GUILayout.Button("SAVE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
//                {
//                    OnSaveSettings();
//                }
//            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }


        public static void DrawStandardSettingsSubPanel()
        {
            float labelWidth = 160;

            if(studioOptions == null || studioOptions.Length == 0 && PlayFabEditor.Studios.Count > 0)
            {
                studioOptions = new string[PlayFabEditor.Studios.Count];
                for(var z = 0; z < PlayFabEditor.Studios.Count; z++)
                {
                    studioOptions[z] = PlayFabEditor.Studios[z].Name;
                }

                // if nothing is selected, then we will want to preload the titles for 0, otherwise
                titleOptions = new string[PlayFabEditor.Studios[0].Titles.Length];
                for(var z = 0; z < PlayFabEditor.Studios[0].Titles.Length; z++)
                {
                    titleOptions[z] = PlayFabEditor.Studios[0].Titles[z].Id;
                }

                #if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                _DeveloperSecretKey = PlayFabEditor.Studios[_selectedStudioIndex].Titles[_selectedTitleIdIndex].SecretKey;
                #endif
            }

            if(_selectedTitleIdIndex != _prevSelectedTitleIdIndex)
            {
                // this changed since the last loop
                _prevSelectedTitleIdIndex = _selectedStudioIndex;
                _TitleId = titleOptions[_selectedTitleIdIndex]; 



             #if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                _DeveloperSecretKey = PlayFabEditor.Studios[_selectedStudioIndex].Titles[_selectedTitleIdIndex].SecretKey;
             #endif

            }

            if(_selectedStudioIndex != _prevSelectedStudioIndex)
            {
                // this changed since the last loop
                _selectedTitleIdIndex = 0; // reset our titles index

                titleOptions = new string[PlayFabEditor.Studios[_selectedStudioIndex].Titles.Length];
                for(var z = 0; z < PlayFabEditor.Studios[_selectedStudioIndex].Titles.Length; z++)
                {
                    titleOptions[z] = PlayFabEditor.Studios[_selectedStudioIndex].Titles[z].Id;
                }

                _prevSelectedStudioIndex = _selectedStudioIndex;
            }




            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true));

            if(studioOptions != null && studioOptions.Length > 0)
            {
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("STUDIO: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                    _selectedStudioIndex = EditorGUILayout.Popup(_selectedStudioIndex, studioOptions, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();
            }

            if(titleOptions != null && titleOptions.Length > 0)
            {
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("TITLE ID: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                    _selectedTitleIdIndex = EditorGUILayout.Popup(_selectedTitleIdIndex, titleOptions, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();
            }

            #if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("DEVELOPER SECRET KEY: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
            _DeveloperSecretKey = EditorGUILayout.TextField(_DeveloperSecretKey, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();

            #endif

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("REQUEST TYPE: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
                _RequestType = (WebRequestType) EditorGUILayout.EnumPopup(_RequestType, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.Height(25));
            GUILayout.EndHorizontal();


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
                EditorPrefs.SetString("PlayFabActiveTitleUrl", PlayFabEditor.Studios[_selectedStudioIndex].Titles[_selectedTitleIdIndex].GameManagerUrl);

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

            float labelWidth = 160;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

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


    public class StudioDisplaySet
    {
        public PlayFab.Editor.EditorModels.Studio Studio;
        public bool isCollapsed = true;
        public Dictionary<string, TitleDisplaySet> titleFoldOutStates = new Dictionary<string, TitleDisplaySet>();
    }

    public class TitleDisplaySet
    {
        public PlayFab.Editor.EditorModels.Title Title;
        public bool isCollapsed = true;
    }


}

