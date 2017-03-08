using PlayFab.PfEditor.EditorModels;
using PlayFab.PfEditor.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    [InitializeOnLoad]
    public class PlayFabEditorDataService : UnityEditor.Editor
    {
        #region EditorPref data classes
        public class PlayFab_DeveloperAccountDetails
        {
            public static string Name = "PlayFab_DeveloperAccountDetails";

            public string email;
            public string devToken;
            public Studio[] studios;
            public PlayFab_DeveloperAccountDetails()
            {
                studios = null; // Null means not fetched, empty is a possible return result from GetStudios
            }
        }

        public class PlayFab_DeveloperEnvironmentDetails
        {
            public static string Name = "PlayFab_DeveloperEnvironmentDetails";

            public bool isAdminApiEnabled;
            public bool isClientApiEnabled;
            public bool isServerApiEnabled;
            public bool isDebugRequestTimesEnabled;
            public string selectedStudio;
            public string selectedTitleId;
            public string developerSecretKey;
            public Dictionary<string, string> titleData;
            public Dictionary<string, string> titleInternalData;
            public string sdkPath;
            public string edexPath;
            public string localCloudScriptPath;

            public PlayFabEditorSettings.WebRequestType webRequestType;
            public bool compressApiData;
            public bool keepAlive;
            public int timeOut;

            public PlayFab_DeveloperEnvironmentDetails()
            {
                titleData = new Dictionary<string, string>();
                titleInternalData = new Dictionary<string, string>();
            }
        }

        public class PlayFab_EditorSettings
        {
            public static string Name = "PlayFab_EditorSettings";

            private bool _isEdExShown;
            private string _latestSdkVersion;
            private string _latestEdExVersion;
            private DateTime _lastSdkVersionCheck;
            private DateTime _lastEdExVersionCheck;
            public bool isEdExShown { get { return _isEdExShown; } set { _isEdExShown = value; Save(); } }
            public string latestSdkVersion { get { return _latestSdkVersion; } set { _latestSdkVersion = value; _lastSdkVersionCheck = DateTime.UtcNow; Save(); } }
            public string latestEdExVersion { get { return _latestEdExVersion; } set { _latestEdExVersion = value; _lastEdExVersionCheck = DateTime.UtcNow; Save(); } }
            public DateTime lastSdkVersionCheck { get { return _lastSdkVersionCheck; } }
            public DateTime lastEdExVersionCheck { get { return _lastEdExVersionCheck; } }

            public static void Save()
            {
                SaveToEditorPrefs(EditorSettings, Name);
            }
        }

        public class PlayFab_EditorView
        {
            public static string Name = "PlayFab_EditorView";

            private int _currentMainMenu;
            private int _currentSubMenu;
            public int currentMainMenu { get { return _currentMainMenu; } set { _currentMainMenu = value; Save(); } }
            public int currentSubMenu { get { return _currentSubMenu; } set { _currentSubMenu = value; Save(); } }

            private static void Save()
            {
                SaveToEditorPrefs(EditorView, Name);
            }

        }
        #endregion EditorPref data classes

        public static PlayFab_DeveloperAccountDetails AccountDetails;
        public static PlayFab_DeveloperEnvironmentDetails EnvDetails;
        public static PlayFab_EditorSettings EditorSettings;
        public static PlayFab_EditorView EditorView;

        private static string KeyPrefix
        {
            get
            {
                var dataPath = Application.dataPath;
                var lastIndex = dataPath.LastIndexOf('/');
                var secondToLastIndex = dataPath.LastIndexOf('/', lastIndex - 1);
                return dataPath.Substring(secondToLastIndex, lastIndex - secondToLastIndex);
            }
        }

        private static bool _IsDataLoaded = false;
        public static bool IsDataLoaded { get { return _IsDataLoaded && AccountDetails != null && EnvDetails != null && EditorSettings != null && EditorView != null; } }

        public static Title ActiveTitle
        {
            get
            {
                if (AccountDetails != null && AccountDetails.studios != null && AccountDetails.studios.Length > 0 && EnvDetails != null)
                {
                    if (string.IsNullOrEmpty(EnvDetails.selectedStudio) || EnvDetails.selectedStudio == PlayFabEditorHelper.STUDIO_OVERRIDE)
                        return new Title { Id = EnvDetails.selectedTitleId, SecretKey = EnvDetails.developerSecretKey, GameManagerUrl = PlayFabEditorHelper.GAMEMANAGER_URL };

                    if (string.IsNullOrEmpty(EnvDetails.selectedStudio) || string.IsNullOrEmpty(EnvDetails.selectedTitleId))
                        return null;

                    try
                    {
                        int studioIndex; int titleIndex;
                        if (DoesTitleExistInStudios(EnvDetails.selectedTitleId, out studioIndex, out titleIndex))
                            return AccountDetails.studios[studioIndex].Titles[titleIndex];
                    }
                    catch (Exception ex)
                    {
                        PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                    }
                }
                return null;
            }
        }

        private static void SaveToEditorPrefs(object obj, string key)
        {
            try
            {
                var json = JsonWrapper.SerializeObject(obj);
                EditorPrefs.SetString(KeyPrefix + key, json);
            }
            catch (Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveAccountDetails()
        {
            SaveToEditorPrefs(AccountDetails, PlayFab_DeveloperAccountDetails.Name);
        }

        public static void SaveEnvDetails(bool updateToScriptableObj = true)
        {
            SaveToEditorPrefs(EnvDetails, PlayFab_DeveloperEnvironmentDetails.Name);
            if (updateToScriptableObj)
                UpdateScriptableObject();
        }

        private static TResult LoadFromEditorPrefs<TResult>(string key) where TResult : class, new()
        {
            if (!EditorPrefs.HasKey(KeyPrefix + key))
                return new TResult();

            var serialized = EditorPrefs.GetString(KeyPrefix + key);
            try
            {
                var result = JsonWrapper.DeserializeObject<TResult>(serialized);
                if (result != null)
                    return JsonWrapper.DeserializeObject<TResult>(serialized);
            }
            catch (Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
            return new TResult();
        }

        public static void LoadAllData()
        {
            if (IsDataLoaded)
                return;

            AccountDetails = LoadFromEditorPrefs<PlayFab_DeveloperAccountDetails>(PlayFab_DeveloperAccountDetails.Name);
            EnvDetails = LoadFromEditorPrefs<PlayFab_DeveloperEnvironmentDetails>(PlayFab_DeveloperEnvironmentDetails.Name);
            EditorSettings = LoadFromEditorPrefs<PlayFab_EditorSettings>(PlayFab_EditorSettings.Name);
            EditorView = LoadFromEditorPrefs<PlayFab_EditorView>(PlayFab_EditorView.Name);
            LoadFromScriptableObject();

            _IsDataLoaded = true;
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnDataLoaded, "Complete");
        }

        private static void LoadFromScriptableObject()
        {
            if (EnvDetails == null)
                return;

            Type playfabSettingsType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (type.Name == "PlayFabSettings")
                        playfabSettingsType = type;

            if (playfabSettingsType == null || !PlayFabEditorSDKTools.IsInstalled || !PlayFabEditorSDKTools.isSdkSupported)
                return;

            var props = playfabSettingsType.GetProperties();
            try
            {
                foreach (var prop in props)
                {
                    switch (prop.Name)
                    {
                        case "TitleId":
                            var propValue = (string)prop.GetValue(null, null);
                            EnvDetails.selectedTitleId = string.IsNullOrEmpty(propValue) ? EnvDetails.selectedTitleId : propValue;
                            break;
                        case "RequestType":
                            EnvDetails.webRequestType = (PlayFabEditorSettings.WebRequestType)prop.GetValue(null, null);
                            break;
                        case "RequestTimeout":
                            EnvDetails.timeOut = (int)prop.GetValue(null, null);
                            break;
                        case "RequestKeepAlive":
                            EnvDetails.keepAlive = (bool)prop.GetValue(null, null);
                            break;
                        case "CompressApiData":
                            EnvDetails.compressApiData = (bool)prop.GetValue(null, null);
                            break;
                        case "DeveloperSecretKey":
                            EnvDetails.developerSecretKey = string.Empty;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                            EnvDetails.developerSecretKey = (string)prop.GetValue(null, null) ?? EnvDetails.developerSecretKey;
#endif
                            break;
                    }
                }
            }
            catch
            {
                // do nothing, this cathes issues in really old sdks; clearly there is something wrong here.
                PlayFabEditorSDKTools.isSdkSupported = false;
            }
        }

        private static void UpdateScriptableObject()
        {
            //TODO move this logic to the data service
            Type playfabSettingsType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (type.Name == "PlayFabSettings")
                        playfabSettingsType = type;

            if (playfabSettingsType == null || !PlayFabEditorSDKTools.IsInstalled || !PlayFabEditorSDKTools.isSdkSupported)
                return;

            var props = playfabSettingsType.GetProperties();
            foreach (var property in props)
            {
                switch (property.Name.ToLower())
                {
                    case "titleid":
                        property.SetValue(null, EnvDetails.selectedTitleId, null); break;
                    case "requesttype":
                        property.SetValue(null, (int)EnvDetails.webRequestType, null); break;
                    case "timeout":
                        property.SetValue(null, EnvDetails.timeOut, null); break;
                    case "requestkeepalive":
                        property.SetValue(null, EnvDetails.keepAlive, null); break;
                    case "compressapidata":
                        property.SetValue(null, EnvDetails.compressApiData, null); break;
                    case "productionenvironmenturl":
                        property.SetValue(null, PlayFabEditorHelper.TITLE_ENDPOINT, null); break;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                    case "developersecretkey":
                        property.SetValue(null, EnvDetails.developerSecretKey, null); break;
#endif
                }
            }

            var getSoMethod = playfabSettingsType.GetMethod("GetSharedSettingsObjectPrivate", BindingFlags.NonPublic | BindingFlags.Static);
            if (getSoMethod != null)
            {
                var so = getSoMethod.Invoke(null, new object[0]) as ScriptableObject;
                if (so != null)
                    EditorUtility.SetDirty(so);
            }
            AssetDatabase.SaveAssets();
        }

        public static void RemoveEditorPrefs()
        {
            EditorPrefs.DeleteKey(KeyPrefix + PlayFab_EditorSettings.Name);
            EditorPrefs.DeleteKey(KeyPrefix + PlayFab_DeveloperEnvironmentDetails.Name);
            EditorPrefs.DeleteKey(KeyPrefix + PlayFab_DeveloperAccountDetails.Name);
            EditorPrefs.DeleteKey(KeyPrefix + PlayFab_EditorView.Name);
        }

        public static bool DoesTitleExistInStudios(string searchFor) //out Studio studio
        {
            if (AccountDetails.studios == null)
                return false;

            searchFor = searchFor.ToLower();
            foreach (var studio in AccountDetails.studios)
                foreach (var title in studio.Titles)
                    if (title.Id.ToLower() == searchFor)
                        return true;
            return false;
        }

        private static bool DoesTitleExistInStudios(string searchFor, out int studioIndex, out int titleIndex) //out Studio studio
        {
            studioIndex = 0; // corresponds to our _OVERRIDE_
            titleIndex = -1;

            if (AccountDetails.studios == null)
                return false;

            for (var studioIdx = 0; studioIdx < AccountDetails.studios.Length; studioIdx++)
            {
                for (var titleIdx = 0; titleIdx < AccountDetails.studios[studioIdx].Titles.Length; titleIdx++)
                {
                    if (AccountDetails.studios[studioIdx].Titles[titleIdx].Id.ToLower() == searchFor.ToLower())
                    {
                        studioIndex = studioIdx;
                        titleIndex = titleIdx;
                        return true;
                    }
                }
            }

            return false;
        }

        public static void GetStudios()
        {
            PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
            {
                AccountDetails.studios = getStudioResult.Studios;
                SaveAccountDetails();
            }, PlayFabEditorHelper.SharedErrorCallback);
        }
    }
}
