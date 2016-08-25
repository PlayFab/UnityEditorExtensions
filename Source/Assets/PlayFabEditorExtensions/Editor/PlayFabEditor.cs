using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using PlayFab.Editor.EditorModels;

namespace PlayFab.Editor
{
    public class PlayFabEditor : EditorWindow
    {

#region EdEx Variables
        // vars for the plugin-wide event system
        public enum EdExStates { OnEnable, OnDisable, OnLogin, OnLogout, OnMenuItemClicked,  OnHttpReq, OnHttpRes, OnError, OnSuccess, OnWarning, OnDataLoaded  } //OnSubmenuItemClicked, OnWaitBegin, OnWaitEnd,

        public delegate void PlayFabEdExStateHandler(EdExStates state, string status, string misc);
        public static event PlayFabEdExStateHandler EdExStateUpdate;

        public static Dictionary<string, float> blockingRequests = new Dictionary<string, float>(); // key and blockingRequest start time
        private static float blockingRequestTimeOut = 10f; // abandon the block after this many seconds.

        internal static PlayFabEditor window;
 #endregion


 #region unity lopps & methods
        void OnEnable()
        {
            if (window == null)
            {
                window = this;
                window.minSize = new Vector2(300, 0);
            }

            if(!IsEventHandlerRegistered(StateUpdateHandler))
            {
                EdExStateUpdate += StateUpdateHandler;
            }

            RaiseStateUpdate(EdExStates.OnEnable);
        }

        void OnDisable()
        {
            // clean up objects:
            PlayFabEditorDataService.editorSettings.isEdExShown = false;
            PlayFabEditorDataService.SaveEditorSettings();

            if(IsEventHandlerRegistered(StateUpdateHandler))
            {
                EdExStateUpdate -= StateUpdateHandler;
            }
        }

        void OnFocus()
        {
            OnEnable();
        }

        [MenuItem("Window/PlayFab/Editor Extensions")]
        static void PlayFabServices()
        {
            var editorAsm = typeof (UnityEditor.Editor).Assembly;
            var inspWndType = editorAsm.GetType("UnityEditor.SceneHierarchyWindow");

            if(inspWndType == null)
            {
                inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
            }

            window = EditorWindow.GetWindow<PlayFabEditor>(inspWndType);
            window.titleContent = new GUIContent("PlayFab EdEx");

        }

        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                if (PlayFabEditorDataService.editorSettings.isEdExShown || !PlayFabEditorSDKTools.IsInstalled || PlayFabEditorDataService.isNewlyInstalled)
                {
                    EditorCoroutine.start(OpenPlayServices());
                }
               
            }
        }

        static IEnumerator OpenPlayServices()
        {
            yield return new WaitForSeconds(1f);
            if (!Application.isPlaying)
            {
                PlayFabServices();
            }

            PlayFabEditorDataService.editorSettings.isEdExShown = true;
            PlayFabEditorDataService.SaveEditorSettings();
        }


        void OnGUI()
        {
            try
            {
                GUI.skin = PlayFabEditorHelper.uiStyle;

                GUILayout.BeginVertical();

                //Run all updaters prior to drawing;  
                PlayFabEditorSettings.Update();

                PlayFabEditorHeader.DrawHeader();


                GUI.enabled = blockingRequests.Count > 0 || EditorApplication.isCompiling ? false : true;

                if(PlayFabEditorDataService.isDataLoaded)
                {
                    if (PlayFabEditorAuthenticate.IsAuthenticated())
                    {
                        //Try catching to avoid Draw errors that do not actually impact the functionality
                        try
                        {
                            PlayFabEditorMenu.DrawMenu();

                            switch (PlayFabEditorMenu._menuState)
                            {
                                case PlayFabEditorMenu.MenuStates.Sdks:
                                    PlayFabEditorSDKTools.DrawSdkPanel();
                                    break;
                                case PlayFabEditorMenu.MenuStates.Settings:
                                    PlayFabEditorSettings.DrawSettingsPanel();
                                    PlayFabEditorSettings.After();
                                    break;
                                case PlayFabEditorMenu.MenuStates.Help:
                                    PlayFabEditorHelpMenu.DrawHelpPanel();
                                    break;
                                case PlayFabEditorMenu.MenuStates.Data:
                                    PlayFabEditorDataMenu.DrawDataPanel();
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch //(Exception ex)
                        {
                            //RaiseStateUpdate(EdExStates.OnError, ex.Message);
                        }
                    }
                    else
                    {
                        PlayFabEditorAuthenticate.DrawAuthPanels();
                    }

                    GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();


                    // help tag at the bottom of the help menu.
                    if(PlayFabEditorMenu._menuState == PlayFabEditorMenu.MenuStates.Help)
                    {
                        GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                        GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                            GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField( string.Format("PlayFab Editor Extensions: {0}", PlayFabEditorHelper.EDEX_VERSION), PlayFabEditorHelper.uiStyle.GetStyle("versionText"));
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        //TODO Add plugin upgrade option here (if available);


                        GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if(GUILayout.Button("VIEW DOCUMENTATION", PlayFabEditorHelper.uiStyle.GetStyle("textButton") ))
                            {
                                Application.OpenURL("https://github.com/PlayFab/UnityEditorExtensions");
                            }
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if(GUILayout.Button("REPORT ISSUES", PlayFabEditorHelper.uiStyle.GetStyle("textButton") ))
                            {
                                Application.OpenURL("https://github.com/PlayFab/UnityEditorExtensions/issues");
                            }
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        if(!string.IsNullOrEmpty(PlayFabEditorHelper.EDITOR_ROOT))
                        {
                            GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                if(GUILayout.Button("UNINSTALL ", PlayFabEditorHelper.uiStyle.GetStyle("textButton") ))
                                {
                                    if(EditorUtility.DisplayDialog("Confirm Editor Extensions Removal", "This action will remove PlayFab Editor Extensions from the current project.", "Confirm", "Cancel"))
                                    {
                                        try
                                        {
                                            PlayFabEditor.window.Close();
                                            var edExRoot = new DirectoryInfo(PlayFabEditorHelper.EDITOR_ROOT);

                                            FileUtil.DeleteFileOrDirectory(edExRoot.Parent.FullName);
                                            PlayFabEditorDataService.RemoveEditorPrefs();


                                            AssetDatabase.Refresh();
                                        }
                                        catch (Exception ex)
                                        {
                                            RaiseStateUpdate(EdExStates.OnError, ex.Message);
                                            PlayFabServices();
                                        }   
                                    }
           
                                }
                                GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                        }
                    }

                    GUILayout.EndVertical();

                    PruneBlockingRequests();

                    try
                    {
                        Repaint();
                    }
                    catch //(Exception ex)
                    {
                       //RaiseStateUpdate(EdExStates.OnError, ex.Message);
                    }
               }

            }
            catch //(Exception ex)
            {
                //RaiseStateUpdate(EdExStates.OnError, ex.Message);
            }
        }

#endregion

#region menu and helper methods
        public static void RaiseStateUpdate(EdExStates state, string status = null, string json = null)
        {
            if(EdExStateUpdate != null)
            {
                EdExStateUpdate(state, status, json);
            }
        }

        public static void PruneBlockingRequests()
        {
            List<string> itemsToRemove = new List<string>();
            foreach(var req in blockingRequests)
            {
                if(req.Value + blockingRequestTimeOut < (float)EditorApplication.timeSinceStartup)
                {
                    itemsToRemove.Add(req.Key);
                }
            }

            foreach(var item in itemsToRemove)
            {
                ClearBlockingRequest(item);
                RaiseStateUpdate(EdExStates.OnWarning, string.Format(" Request {0} has timed out after {1} seconds.", item, blockingRequestTimeOut));
            }

        }

        public static void AddBlockingRequest(string state)
        {
            if(blockingRequests.ContainsKey(state))
            {
                blockingRequests[state] = (float)EditorApplication.timeSinceStartup;
            }
            else
            {
                blockingRequests.Add(state, (float)EditorApplication.timeSinceStartup);
            }
        }

        public static void ClearBlockingRequest(string state = null)
        {
           if(state == null)
           {
                blockingRequests.Clear();
           }
           else
           {
                if(blockingRequests.ContainsKey(state))
                {
                    blockingRequests.Remove(state);

                }
           }

        }


        /// <summary>
        /// Handles state updates within the editor extension.
        /// </summary>
        /// <param name="state">the state that triggered this event.</param>
        /// <param name="status">a generic message about the status.</param>
        /// <param name="json">a generic container for additional JSON encoded info.</param>
        public void StateUpdateHandler(EdExStates state, string status, string json)
        {
            switch(state)
            {
                case EdExStates.OnMenuItemClicked:
                    //Debug.Log(string.Format("MenuItem: {0} Clicked", status));
                break;

                case EdExStates.OnHttpReq:
                    object temp;
                    if(!string.IsNullOrEmpty(json) && Json.PlayFabSimpleJson.TryDeserializeObject(json, out temp))  // Json.JsonWrapper.DeserializeObject(json);)
                    {
                       Json.JsonObject deserialized = temp as Json.JsonObject;
                       object useSpinner = false;
                       object blockUi = false;

                        if(deserialized.TryGetValue("useSpinner", out useSpinner) && bool.Parse(useSpinner.ToString()))
                        {
                            ProgressBar.UpdateState(ProgressBar.ProgressBarStates.spin);
                        }

                        if(deserialized.TryGetValue("blockUi", out blockUi) && bool.Parse(blockUi.ToString()))
                        {
                            AddBlockingRequest(status);
                        }  
                    }
                break;

                case EdExStates.OnHttpRes:
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.off);
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.success);
                    ClearBlockingRequest(status);


                break;

                case EdExStates.OnError:

                    // deserialize and add json details
                    // clear blocking requests
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.error);
                    ClearBlockingRequest();
                    Debug.LogError(string.Format("PlayFab EditorExtensions: Caught an error:{0}", status)); 
                break;

                case EdExStates.OnWarning:
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.warning);
                    ClearBlockingRequest();
                    Debug.LogWarning(string.Format("PlayFab EditorExtensions: {0}", status)); 
                break;

                case EdExStates.OnSuccess:
                    ClearBlockingRequest();
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.success);
                    break;
            }
        }

        public static bool IsEventHandlerRegistered(PlayFabEdExStateHandler prospectiveHandler)
        {   
            if (EdExStateUpdate != null )
            {
                foreach ( PlayFabEdExStateHandler existingHandler in EdExStateUpdate.GetInvocationList() )
                {
                    if ( existingHandler == prospectiveHandler )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
#endregion
    }
}