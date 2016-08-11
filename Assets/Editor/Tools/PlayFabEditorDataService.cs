namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    [InitializeOnLoad]
    public class PlayFabEditorDataService : Editor {
        public static PlayFab_DeveloperAccountDetails accountDetails;
        public static PlayFab_DeveloperEnvironmentDetails envDetails;
        public static PlayFab_EditorSettings editorSettings;

       public static EditorModels.Title activeTitle 
       { 
            get {
                if(accountDetails != null && accountDetails.studios.Count > 0)
                {
                    if(envDetails != null && !string.IsNullOrEmpty(envDetails.selectedStudio) && !string.IsNullOrEmpty(envDetails.selectedTitleId))
                    {
                        try
                        {
                            return accountDetails.studios.Find((EditorModels.Studio item) => { return item.Name == envDetails.selectedStudio; }).Titles.First((EditorModels.Title title) => { return title.Id == envDetails.selectedTitleId; });
                        }
                        catch(Exception ex)
                        {
                            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                        }
                    }
                    return accountDetails.studios[0].Titles[0]; 
                }
                return null;
            }
       }

        public static void SaveAccountDetails()
        {
            try
            {
                var serialized = Json.JsonWrapper.SerializeObject(accountDetails);
                EditorPrefs.SetString("PlayFab_DeveloperAccountDetails", serialized);
            }
            catch(Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveEnvDetails()
        {
            try
            {
                var serialized = Json.JsonWrapper.SerializeObject(envDetails);
                EditorPrefs.SetString("PlayFab_DeveloperEnvironmentDetails", serialized);

                //update scriptable object
            }
            catch(Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveEditorSettings()
        {
            try
            {
                var serialized = Json.JsonWrapper.SerializeObject(editorSettings);
                EditorPrefs.SetString("PlayFab_EditorSettings", serialized);
            }
            catch(Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void LoadAccountDetails()
        {
            if(EditorPrefs.HasKey("PlayFab_DeveloperAccountDetails"))
            {
                var serialized = EditorPrefs.GetString("PlayFab_DeveloperAccountDetails");
                try
                {
                    accountDetails = Json.JsonWrapper.DeserializeObject<PlayFab_DeveloperAccountDetails>(serialized);
                    return;

                }
                catch(Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            accountDetails = new PlayFab_DeveloperAccountDetails();
        }

        public static void LoadEnvDetails()
        {
            if(EditorPrefs.HasKey("PlayFab_DeveloperEnvironmentDetails"))
            {
                var serialized = EditorPrefs.GetString("PlayFab_DeveloperEnvironmentDetails");
                try
                {
                    envDetails = Json.JsonWrapper.DeserializeObject<PlayFab_DeveloperEnvironmentDetails>(serialized);
                    return;

                }
                catch(Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            envDetails = new PlayFab_DeveloperEnvironmentDetails();
        }

        public static void LoadEditorSettings()
        {
            if(EditorPrefs.HasKey("PlayFab_EditorSettings"))
            {
                var serialized = EditorPrefs.GetString("PlayFab_EditorSettings");
                try
                {
                    editorSettings = Json.JsonWrapper.DeserializeObject<PlayFab_EditorSettings>(serialized);
                    return;

                }
                catch(Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            editorSettings = new PlayFab_EditorSettings();
        }


        public static void SaveAllData()
        {
            SaveAccountDetails();
            SaveEnvDetails();
            SaveEditorSettings();
        }

        public static void LoadAllData()
        {
            LoadAccountDetails();
            LoadEnvDetails();
            LoadEditorSettings();
        }


        public static void UpdateScriptableObject()
        {
                //TODO move this logic to the data service
                var playfabSettingsType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == "PlayFabSettings"
                    select type);
                if (playfabSettingsType.ToList().Count > 0)
                {
                    var type = playfabSettingsType.ToList().FirstOrDefault();
                    var fields = type.GetFields();
                    var props = type.GetProperties();

//                    foreach (var property in props)
//                    {
//                        //Debug.Log(property.Name);
//                        if (property.Name.ToLower() == "titleid")
//                        {
//                            property.SetValue(null, titleOptions[_selectedTitleIdIndex], null);
//                        }
//                        if (property.Name.ToLower() == "requesttype")
//                        {
//                            property.SetValue(null, (int)_RequestType, null);
//                        }
//                        if (property.Name.ToLower() == "timeout")
//                        {
//                            property.SetValue(null, _RequestTimeOut, null);
//                        }
//                        if (property.Name.ToLower() == "requestkeepalive")
//                        {
//                            property.SetValue(null, _KeepAlive, null);
//                        }
//                        if (property.Name.ToLower() == "compressapidata")
//                        {
//                            property.SetValue(null, _CompressApiData, null);
//                        }
//#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
//                        if (property.Name.ToLower() == "developersecretkey")
//                        {
//                            property.SetValue(null, _DeveloperSecretKey, null);
//                        }
//#endif
//                    }

                    AssetDatabase.SaveAssets();
                 }
        }

        //CTOR
        static PlayFabEditorDataService()
        {
            if(!PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate += StateUpdateHandler;
            }
            LoadAllData();
        }


        public void OnDestroy()
        {
            if(PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate -= StateUpdateHandler;
            }
        }


        public static void StateUpdateHandler(PlayFabEditor.EdExStates state, string status, string json)
        {
            //Debug.Log(string.Format("PFE: Handled EdExStatusUpdate:{0}, Status:{1}, Misc:{2}", state, status, json)); 

            switch(state)
            {
//                case EdExStates.OnEnable:
//                   
//                break;
//                case EdExStates.OnDisable:
//                   
//                break;
//                case EdExStates.OnLogin:
//                   
//                break;
//                case EdExStates.OnLogout:
//                   
//                break;
                case PlayFabEditor.EdExStates.OnMenuItemClicked:
                    
                    //Debug.Log(string.Format("MenuItem: {0} Clicked", status));
                break;
//
//                case EdExStates.OnSubmenuItemClicked:
//                   
//                break;
//
//                case EdExStates.OnHttpReq:
//                    //JsonWrapper.SerializeObject(request, PlayFabEditorUtil.ApiSerializerStrategy);
//                    object temp;
//                    if(!string.IsNullOrEmpty(json) && Json.PlayFabSimpleJson.TryDeserializeObject(json, out temp))  // Json.JsonWrapper.DeserializeObject(json);)
//                    {
//                       Json.JsonObject deserialized = temp as Json.JsonObject;
//                       object useSpinner = false;
//                       object blockUi = false;
//
//                        if(deserialized.TryGetValue("useSpinner", out useSpinner) && bool.Parse(useSpinner.ToString()))
//                        {
//                            ProgressBar.UpdateState(ProgressBar.ProgressBarStates.spin);
//                        }
//
//                        if(deserialized.TryGetValue("blockUi", out blockUi) && bool.Parse(blockUi.ToString()))
//                        {
//                            AddBlockingRequest(status);
//                        }
//
//                    }
//                break;
//
//                case EdExStates.OnHttpRes:
//                    //var httpResult = JsonWrapper.DeserializeObject<HttpResponseObject>(response, PlayFabEditorUtil.ApiSerializerStrategy);
//                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.off);
//                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.success);
//                    ClearBlockingRequest(status);
//                break;
            }
        }

    }



  

    //TODO move these classes to editor models

    public class PlayFab_DeveloperAccountDetails
    {
        public string email { get; set; }
        public string devToken { get; set; }
        public List<EditorModels.Studio> studios { get; set; }
        public bool useAutoLogin { get; set; }

        public PlayFab_DeveloperAccountDetails()
        {
            studios = new List<EditorModels.Studio>();
        }
    }

    public class PlayFab_DeveloperEnvironmentDetails
    {
        public bool isAdminApiEnabled { get; set; }
        public bool isClientApiEnabled { get; set; }
        public bool isServerApiEnabled { get; set; }
        public bool isDebugRequestTimesEnabled { get; set; }
        public string selectedStudio { get; set; }
        public string selectedTitleId { get; set; }
        public string developerSecretKey { get; set; }
        public Dictionary<string, string> titleData { get; set; }

        public PlayFab_DeveloperEnvironmentDetails()
        {
            titleData = new Dictionary<string, string>();
        }
    }

    public class PlayFab_EditorSettings
    {
        public int currentMainMenu { get; set; }
        // sub menu?
        public bool isEdExShown { get; set; }
    

    }



}