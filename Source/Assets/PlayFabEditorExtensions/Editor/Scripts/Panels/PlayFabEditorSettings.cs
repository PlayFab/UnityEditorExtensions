using PlayFab.PfEditor.EditorModels;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    [InitializeOnLoad]
    public class PlayFabEditorSettings : UnityEditor.Editor
    {
        #region panel variables
        public enum SubMenuStates
        {
            StandardSettings,
            TitleSettings,
            ApiSettings,
            Packages
        }

        public enum WebRequestType
        {
            UnityWww, // High compatability Unity api calls
            HttpWebRequest // High performance multi-threaded api calls
        }

        private static readonly List<string> BuildTargets = new List<string>();

        private static SubMenuComponent _menu = null;

        private static string[] _titleOptions;
        private static string[] _studioOptions;

        private static int _selectedTitleIdIndex = 0;
        private static int _selectedStudioIndex = -1;
        private static int _prevSelectedTitleIdIndex = 0;
        private static int _prevSelectedStudioIndex = -1;

        private static string _titleId;
        private static string _prevTitleId;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
        private static string _DeveloperSecretKey;
#endif

        private static bool _isSettingsSet = false;
        private static bool _foundUnknownTitleId = false;
        private static bool _isFetchingStudios = false;

        private static Dictionary<string, StudioDisplaySet> studioFoldOutStates = new Dictionary<string, StudioDisplaySet>();
        private static Vector2 _titleScrollPos = Vector2.zero;
        private static Vector2 _packagesScrollPos = Vector2.zero;
        #endregion

        #region draw calls

        private static void DrawApiSubPanel()
        {
            float labelWidth = 160;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

            using (var fwl = new FixedWidthLabel("ENABLE CLIENT API: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                PlayFabEditorDataService.EnvDetails.isClientApiEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isClientApiEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            using (var fwl = new FixedWidthLabel("ENABLE ADMIN API:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                PlayFabEditorDataService.EnvDetails.isAdminApiEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isAdminApiEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            using (var fwl = new FixedWidthLabel("ENABLE SERVER API: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                PlayFabEditorDataService.EnvDetails.isServerApiEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isServerApiEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            using (var fwl = new FixedWidthLabel("ENABLE REQUEST TIMES: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }

            GUILayout.EndVertical();
        }

        public static void DrawSettingsPanel()
        {
            SetSettingsProperties();

            if (PlayFabEditorDataService.IsDataLoaded)
            {
                if (_menu != null)
                {
                    _menu.DrawMenu();
                    switch ((SubMenuStates)PlayFabEditorDataService.EditorView.currentSubMenu)
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
                            DrawPackagesSubPanel();
                            break;
                    }
                }
                else
                {
                    RegisterMenu();
                }
            }
        }

        private static void DrawTitleSettingsSubPanel()
        {
            float labelWidth = 100;

            if (PlayFabEditorDataService.AccountDetails.studios != null && PlayFabEditorDataService.AccountDetails.studios.Length != studioFoldOutStates.Count)
            {
                studioFoldOutStates.Clear();
                foreach (var studio in PlayFabEditorDataService.AccountDetails.studios)
                {
                    if (!studioFoldOutStates.ContainsKey(studio.Id))
                        studioFoldOutStates.Add(studio.Id, new StudioDisplaySet() { Studio = studio });
                    foreach (var title in studio.Titles)
                        if (!studioFoldOutStates[studio.Id].titleFoldOutStates.ContainsKey(title.Id))
                            studioFoldOutStates[studio.Id].titleFoldOutStates.Add(title.Id, new TitleDisplaySet { Title = title });
                }
            }

            _titleScrollPos = GUILayout.BeginScrollView(_titleScrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
            EditorGUILayout.LabelField("STUDIOS:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("REFRESH", PlayFabEditorHelper.uiStyle.GetStyle("Button")))
                RefreshStudiosList();
            GUILayout.EndHorizontal();

            foreach (var studio in studioFoldOutStates)
            {
                var style = new GUIStyle(EditorStyles.foldout);
                if (studio.Value.isCollapsed)
                    style.fontStyle = FontStyle.Normal;

                studio.Value.isCollapsed = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), studio.Value.isCollapsed, string.Format("{0} ({1})", studio.Value.Studio.Name, studio.Value.Studio.Titles.Length), true, PlayFabEditorHelper.uiStyle.GetStyle("foldOut_std"));
                if (studio.Value.isCollapsed)
                    continue;

                EditorGUI.indentLevel = 2;

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("TITLES:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                // draw title foldouts
                foreach (var title in studio.Value.titleFoldOutStates)
                {
                    title.Value.isCollapsed = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), title.Value.isCollapsed, string.Format("{0} [{1}]", title.Value.Title.Name, title.Value.Title.Id), true, PlayFabEditorHelper.uiStyle.GetStyle("foldOut_std"));
                    if (title.Value.isCollapsed)
                        continue;

                    EditorGUI.indentLevel = 3;
                    GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("SECRET KEY:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                    EditorGUILayout.TextField("" + title.Value.Title.SecretKey);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("URL:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("VIEW IN GAME MANAGER", PlayFabEditorHelper.uiStyle.GetStyle("textButton")))
                        Application.OpenURL(title.Value.Title.GameManagerUrl);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    EditorGUI.indentLevel = 2;
                }

                EditorGUI.indentLevel = 0;
            }
            GUILayout.EndScrollView();
        }

        private static void DrawStandardSettingsSubPanel()
        {
            float labelWidth = 160;

            if (_foundUnknownTitleId && (_titleId != _prevTitleId))
            {
                _prevTitleId = _titleId;
            }

            if (_selectedStudioIndex != _prevSelectedStudioIndex)
            {
                // handle our _OVERRIDE_ STATE FIRST
                if (_selectedStudioIndex == 0)
                {
                    _prevSelectedStudioIndex = 0;
                    _selectedTitleIdIndex = 0;
                    _foundUnknownTitleId = true;
                    _titleId = string.Empty;

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                    _DeveloperSecretKey = string.Empty;
#endif

                }
                else
                {
                    _foundUnknownTitleId = false;
                    _selectedTitleIdIndex = _selectedTitleIdIndex > PlayFabEditorDataService.AccountDetails.studios[_selectedStudioIndex - 1].Titles.Length ? 0 : _selectedTitleIdIndex; // reset our titles index
                    CompoundTitlesList();
                    _prevSelectedStudioIndex = _selectedStudioIndex;
                }
            }

            if (_selectedTitleIdIndex != _prevSelectedTitleIdIndex)
            {
                // this changed since the last loop
                _prevSelectedTitleIdIndex = _selectedStudioIndex;

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                _DeveloperSecretKey = PlayFabEditorDataService.AccountDetails.studios[_selectedStudioIndex - 1].Titles[_selectedTitleIdIndex].SecretKey;
#endif
            }

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true));
            if (_foundUnknownTitleId)
            {
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                GUILayout.Label("You are using a TitleId to which you are not a memeber. A title administrator can approve access for your account.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));
                GUILayout.EndHorizontal();
            }

            if (_studioOptions != null && _studioOptions.Length > 0)
            {
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("STUDIO: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                _selectedStudioIndex = EditorGUILayout.Popup(_selectedStudioIndex, _studioOptions, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();
            }

            // SPECIAL HANDLING FOR THESE FIELDS -- How we show the following depends on the data state  (TitleId, DevKey & Studio)
            if (_foundUnknownTitleId || _selectedStudioIndex == 0)
            {
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("TITLE ID: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                _titleId = EditorGUILayout.TextField(_titleId, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("DEVELOPER SECRET KEY: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                _DeveloperSecretKey = EditorGUILayout.TextField(_DeveloperSecretKey, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();
#endif
            }
            else
            {
                if (_titleOptions != null && _titleOptions.Length > 0)
                {
                    GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                    EditorGUILayout.LabelField("TITLE ID: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                    _selectedTitleIdIndex = EditorGUILayout.Popup(_selectedTitleIdIndex, _titleOptions, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                    GUILayout.EndHorizontal();
                }

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                EditorGUILayout.LabelField("DEVELOPER SECRET KEY: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                _DeveloperSecretKey = EditorGUILayout.TextField(_DeveloperSecretKey, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                GUILayout.EndHorizontal();
#endif
            }
            // ------------------------------------------------------------------------------------------------------------------------------------------------
            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
            EditorGUILayout.LabelField("REQUEST TYPE: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
            PlayFabEditorDataService.SharedSettings.WebRequestType = (WebRequestType)EditorGUILayout.EnumPopup(PlayFabEditorDataService.SharedSettings.WebRequestType, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.Height(25));
            GUILayout.EndHorizontal();

            if (PlayFabEditorDataService.SharedSettings.WebRequestType == WebRequestType.HttpWebRequest)
            {
                using (var fwl = new FixedWidthLabel(new GUIContent("REQUEST TIMEOUT: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.SharedSettings.TimeOut = EditorGUILayout.IntField(PlayFabEditorDataService.SharedSettings.TimeOut, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                }

                using (var fwl = new FixedWidthLabel(new GUIContent("KEEP ALIVE: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.SharedSettings.KeepAlive = EditorGUILayout.Toggle(PlayFabEditorDataService.SharedSettings.KeepAlive, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }
            }

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
            EditorGUILayout.LabelField("COMPRESS API DATA: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
            PlayFabEditorDataService.SharedSettings.CompressApiData = EditorGUILayout.Toggle(PlayFabEditorDataService.SharedSettings.CompressApiData, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
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

        private static void DrawPackagesSubPanel()
        {
            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
            GUILayout.Label("Packages are additional PlayFab features that can be installed. Enabling a package will install the AsssetPackage; disabling will remove the package.", PlayFabEditorHelper.uiStyle.GetStyle("genTxt"));
            GUILayout.EndHorizontal();

            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {
                float labelWidth = 245;
                _packagesScrollPos = GUILayout.BeginScrollView(_packagesScrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                using (var fwl = new FixedWidthLabel("Push Notification Plugin (Android): "))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorPackageManager.AndroidPushPlugin = EditorGUILayout.Toggle(PlayFabEditorPackageManager.AndroidPushPlugin, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"));
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                if (GUILayout.Button("VIEW GUIDE", PlayFabEditorHelper.uiStyle.GetStyle("Button")))
                {
                    Application.OpenURL("https://github.com/PlayFab/UnitySDK/tree/master/PluginsSource/UnityAndroidPluginSource#playfab-push-notification-plugin");
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }
        }
        #endregion

        #region unity-like loops

        public static void Update()
        {
            BuildTargets.Clear();
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            foreach (var each in symbols)
                BuildTargets.Add(each);
        }

        public static void After()
        {
            if (PlayFabEditorDataService.EnvDetails.isAdminApiEnabled && !BuildTargets.Contains(PlayFabEditorHelper.ADMIN_API))
            {
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.ADMIN_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (!PlayFabEditorDataService.EnvDetails.isAdminApiEnabled && BuildTargets.Contains(PlayFabEditorHelper.ADMIN_API))
            {
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.ADMIN_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

            if (PlayFabEditorDataService.EnvDetails.isServerApiEnabled && !BuildTargets.Contains(PlayFabEditorHelper.SERVER_API))
            {
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.SERVER_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (!PlayFabEditorDataService.EnvDetails.isServerApiEnabled && BuildTargets.Contains(PlayFabEditorHelper.SERVER_API))
            {
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.SERVER_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

            if (PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled && !BuildTargets.Contains(PlayFabEditorHelper.DEBUG_REQUEST_TIMING))
            {
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.DEBUG_REQUEST_TIMING);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (!PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled && BuildTargets.Contains(PlayFabEditorHelper.DEBUG_REQUEST_TIMING))
            {
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.DEBUG_REQUEST_TIMING);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

            if (!PlayFabEditorDataService.EnvDetails.isClientApiEnabled && !BuildTargets.Contains(PlayFabEditorHelper.CLIENT_API))
            {
                Debug.Log(PlayFabEditorHelper.CLIENT_API + ":" + BuildTargets.Contains(PlayFabEditorHelper.CLIENT_API));
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.CLIENT_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (PlayFabEditorDataService.EnvDetails.isClientApiEnabled && BuildTargets.Contains(PlayFabEditorHelper.CLIENT_API))
            {
                Debug.Log(PlayFabEditorHelper.CLIENT_API + "- Removed");
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.CLIENT_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

        }

        #endregion

        #region menu and helper methods
        private static void SetSettingsProperties()
        {
            if (PlayFabEditorSDKTools.IsInstalled && !_isSettingsSet)
            {

                if (!string.IsNullOrEmpty(PlayFabEditorDataService.EnvDetails.selectedTitleId))
                {
                    // exists in user's titles?
                    if (!PlayFabEditorDataService.DoesTitleExistInStudios(PlayFabEditorDataService.EnvDetails.selectedTitleId))
                    {
                        string msg = string.Format("A title id ({0}) that does not belong to one of your studios has been selected. You can update this value on the PROJECT SETTINGS tab within the PlayFab Editor Extensions.", PlayFabEditorDataService.EnvDetails.selectedTitleId);
                        PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnWarning, msg);
                        _foundUnknownTitleId = true;
                        _prevSelectedStudioIndex = 0;
                        _selectedStudioIndex = 0;

                        _titleId = PlayFabEditorDataService.EnvDetails.selectedTitleId;

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                        _DeveloperSecretKey = PlayFabEditorDataService.EnvDetails.developerSecretKey;
#endif
                    }
                    else
                    {
                        _foundUnknownTitleId = false;
                    }

                }

                if (_studioOptions == null || (_studioOptions.Length == 0 && PlayFabEditorDataService.AccountDetails.studios != null && PlayFabEditorDataService.AccountDetails.studios.Length > 0))
                {
                    BuildDropDownLists();
                }

                _isSettingsSet = true;
            }
        }

        private static void BuildDropDownLists()
        {
            if (PlayFabEditorDataService.AccountDetails.studios == null && _isFetchingStudios == false)
            {
                RefreshStudiosList();
                return;
            }
            var studioCount = PlayFabEditorDataService.AccountDetails.studios == null ? 0 : PlayFabEditorDataService.AccountDetails.studios.Length;
            if (studioCount == 0)
                return;

            _studioOptions = new string[studioCount + 1];

            for (var studioIdx = 0; studioIdx < studioCount + 1; studioIdx++)
            {
                // set up a manual override
                if (studioIdx == 0)
                {
                    _studioOptions[studioIdx] = PlayFabEditorHelper.STUDIO_OVERRIDE;
                    continue;
                }

                _studioOptions[studioIdx] = PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Name;

                if (PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Name == PlayFabEditorDataService.EnvDetails.selectedStudio)
                {
                    _prevSelectedStudioIndex = studioIdx;
                    _selectedStudioIndex = studioIdx;
                }

                bool foundTitle = false;
                for (var titleIdx = 0; titleIdx < PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Titles.Length; titleIdx++)
                {
                    if (foundTitle)
                    {
                        // then we know this is the correct studio
                        _titleOptions[titleIdx] = string.Format("[{0}] {1}", PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Titles[titleIdx].Id, PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Titles[titleIdx].Name);
                    }

                    string comp1 = PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Titles[titleIdx].Id.ToLower();
                    string comp2 = (PlayFabEditorDataService.EnvDetails.selectedTitleId ?? "").ToLower();

                    if (comp1 == comp2 && foundTitle == false)
                    {
                        foundTitle = true;
                        _titleOptions = new string[PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Titles.Length];

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                        _DeveloperSecretKey = PlayFabEditorDataService.AccountDetails.studios[studioIdx - 1].Titles[titleIdx].SecretKey;
#endif

                        _selectedTitleIdIndex = titleIdx;
                        _prevSelectedTitleIdIndex = titleIdx;

                        _selectedStudioIndex = studioIdx;
                        _prevSelectedStudioIndex = studioIdx;

                        // restart this inner loop to add titles to dropdown
                        titleIdx = -1;
                    }
                }
            }

            if (PlayFabEditorDataService.AccountDetails.studios == null || PlayFabEditorDataService.AccountDetails.studios.Length == 0)
            {
                // this should not happen. But it seems to be happening...
                _selectedStudioIndex = 0;
                _prevSelectedStudioIndex = 0;
                _selectedTitleIdIndex = 0;
                _prevSelectedTitleIdIndex = 0;
                _foundUnknownTitleId = true;
            }
            else if ((_titleOptions == null || _titleOptions.Length == 0) && _prevSelectedStudioIndex == -1)
            {
                // could not find our title, but lets build a list anyways 
                _titleOptions = new string[PlayFabEditorDataService.AccountDetails.studios[0].Titles.Length];
                for (var x = 0; x < _titleOptions.Length; x++)
                {
                    _titleOptions[x] = string.Format("[{0}] {1}", PlayFabEditorDataService.AccountDetails.studios[0].Titles[x].Id, PlayFabEditorDataService.AccountDetails.studios[0].Titles[x].Name);
                }
                _selectedStudioIndex = 1;
                _prevSelectedStudioIndex = 1;
                _selectedTitleIdIndex = 0;
                _prevSelectedTitleIdIndex = 0;
            }
        }

        private static void RegisterMenu()
        {
            if (_menu != null)
                return;

            _menu = CreateInstance<SubMenuComponent>();
            _menu.RegisterMenuItem("PROJECT", OnStandardSetttingsClicked);
            _menu.RegisterMenuItem("STUDIOS", OnTitleSettingsClicked);
            _menu.RegisterMenuItem("API", OnApiSettingsClicked);
            _menu.RegisterMenuItem("PACKAGES", OnPackagesClicked);
        }

        private static void OnPackagesClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.Packages.ToString(), "" + (int)SubMenuStates.Packages);
        }

        private static void OnApiSettingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.ApiSettings.ToString(), "" + (int)SubMenuStates.ApiSettings);
        }

        private static void OnStandardSetttingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.StandardSettings.ToString(), "" + (int)SubMenuStates.StandardSettings);
        }

        private static void OnTitleSettingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.TitleSettings.ToString(), "" + (int)SubMenuStates.TitleSettings);
        }
        
        private static void OnSaveSettings()
        {
            // make assessments on the override at studio positon 0
            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {
                if (_foundUnknownTitleId || _selectedStudioIndex == 0)
                {
                    if (PlayFabEditorDataService.EnvDetails.selectedTitleId != _titleId)
                    {
                        PlayFabEditorDataService.EnvDetails.titleData.Clear();
                        if (PlayFabEditorDataMenu.tdViewer != null)
                        {
                            PlayFabEditorDataMenu.tdViewer.items.Clear();
                        }
                    }

                    PlayFabEditorDataService.EnvDetails.selectedTitleId = _titleId;
                }
                else
                {
                    // if we switched titles clear titledata 
                    if (PlayFabEditorDataService.EnvDetails.selectedTitleId != GetSelectedTitleIdFromOptions())
                    {
                        PlayFabEditorDataService.EnvDetails.titleData.Clear();
                        if (PlayFabEditorDataMenu.tdViewer != null)
                        {
                            PlayFabEditorDataMenu.tdViewer.items.Clear();
                        }
                    }

                    PlayFabEditorDataService.EnvDetails.selectedTitleId = GetSelectedTitleIdFromOptions();
                }

                PlayFabEditorDataService.EnvDetails.selectedStudio = _studioOptions[_selectedStudioIndex];

#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                PlayFabEditorDataService.EnvDetails.developerSecretKey = _DeveloperSecretKey;
#endif

                PlayFabEditorDataService.SaveEnvDetails();

                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSuccess);

            }
            else
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, "SDK is unsupported or not installed");
            }
        }


        private static string AddToBuildTarget(List<string> targets, string define)
        {
            targets.Add(define);
            return string.Join(";", targets.ToArray());
        }

        private static string RemoveToBuildTarget(List<string> targets, string define)
        {
            targets.Remove(define);
            return string.Join(";", targets.ToArray());
        }

        //CTOR
        static PlayFabEditorSettings()
        {
            if (!PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
                PlayFabEditor.EdExStateUpdate += StateUpdateHandler;
        }

        private static string GetSelectedTitleIdFromOptions()
        {
            if (_titleOptions != null && _titleOptions.Length > 0)
                return _titleOptions[_selectedTitleIdIndex].Substring(1, _titleOptions[_selectedTitleIdIndex].IndexOf(']') - 1);
            return string.Empty;
        }

        private static void CompoundTitlesList()
        {
            _titleOptions = new string[PlayFabEditorDataService.AccountDetails.studios[_selectedStudioIndex - 1].Titles.Length];
            for (var z = 0; z < PlayFabEditorDataService.AccountDetails.studios[_selectedStudioIndex - 1].Titles.Length; z++)
            {
                _titleOptions[z] = string.Format("[{0}] {1}", PlayFabEditorDataService.AccountDetails.studios[_selectedStudioIndex - 1].Titles[z].Id, PlayFabEditorDataService.AccountDetails.studios[_selectedStudioIndex - 1].Titles[z].Name);
            }
        }

        private static void RefreshStudiosList()
        {
            _isFetchingStudios = true;
            PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
            {
                _isFetchingStudios = false;
                _isSettingsSet = false;
                _studioOptions = null;
                PlayFabEditorDataService.AccountDetails.studios = getStudioResult.Studios;
                PlayFabEditorDataService.SaveAccountDetails();
            }, PlayFabEditorHelper.SharedErrorCallback);
        }

        /// <summary>
        /// Handles state updates within the editor extension.
        /// </summary>
        /// <param name="state">the state that triggered this event.</param>
        /// <param name="status">a generic message about the status.</param>
        /// <param name="json">a generic container for additional JSON encoded info.</param>
        private static void StateUpdateHandler(PlayFabEditor.EdExStates state, string status, string json)
        {
            switch (state)
            {
                case PlayFabEditor.EdExStates.OnMenuItemClicked:
                    if (status == "Settings")
                    {
                        _isSettingsSet = false;
                        _studioOptions = null;
                        SetSettingsProperties();
                    }
                    break;
                case PlayFabEditor.EdExStates.OnLogin:
                    _studioOptions = null;
                    break;
            }
        }
        #endregion
    }
}
