namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    public class PlayFabEditorDataService : Editor {

        public static PlayFab_DeveloperAccountDetails accountDetails;
        public static PlayFab_DeveloperEnvironmentDetails envDetails;
        public static PlayFab_EditorSettings editorSettings;
        
        //save accountDetails

        //save envDetails

        //save editorSettings


        //load accountDetails

        //load envDetails

        //load editorSettings
    }





    //TODO move these classes to editor models

    public class PlayFab_DeveloperAccountDetails
    {
        public string userName { get; set; }
        public string devToken { get; set; }
        public List<EditorModels.Studio> studios { get; set; }
    }

    public class PlayFab_DeveloperEnvironmentDetails
    {
        public bool isAdminApiEnabled { get; set; }
        public bool isClientApiEnabled { get; set; }
        public bool isServerApiEnabled { get; set; }
        public bool isDebugRequestTimesEnabled { get; set; }
        public string selectedStudio { get; set; }
        public string selectedTitleId { get; set; }
        public Dictionary<string, string> titleData { get; set; }

    }

    public class PlayFab_EditorSettings
    {
        public int currentMainMenu { get; set; }
        // sub menu?
        public bool isEdExShown { get; set; }
    

    }



}