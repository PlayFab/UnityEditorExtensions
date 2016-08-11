

namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UI;
    using PlayFab.Editor.EditorModels;

    public class PlayFabEditor : EditorWindow
    {
        //public static List<Studio> Studios = new List<Studio>();

        public enum EdExStates { OnEnable, OnDisable, OnLogin, OnLogout, OnMenuItemClicked, OnSubmenuItemClicked, OnHttpReq, OnHttpRes, OnError, OnWaitBegin, OnWaitEnd, OnSuccess, OnWarning  }


        public delegate void PlayFabEdExStateHandler(EdExStates state, string status, string misc);
        public static event PlayFabEdExStateHandler EdExStateUpdate;

        // testing alt update loop
        public delegate void playFabEditorUpdate();
        public static event playFabEditorUpdate UpdateLoopTick;

        internal static PlayFabEditor window;
        internal static float Progress = 0f;
        internal static bool HasEditorShown; 

        private ListDisplay listDisplay;
        public static bool isGuiEnabled = true;



        public static Dictionary<string, float> blockingRequests = new Dictionary<string, float>(); // key and blockingRequest start time
        private static float blockingRequestTimeOut = 10f; // abandon the block after this many seconds.

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
            UnityEngine.Object.DestroyImmediate(listDisplay);

            EditorPrefs.DeleteKey("PlayFabToolsShown");



//            if(IsEventHandlerRegistered(StateUpdateResponse))
//            {
//                EdExStateUpdate -= StateUpdateResponse;
//            }
        }

        void OnFocus()
        {
            OnEnable();
        }

        [MenuItem("Window/PlayFab/Editor Extensions")]
        static void PlayFabServices()
        {
            var editorAsm = typeof (Editor).Assembly;
            //var inspWndType = editorAsm.GetType("UnityEditor.SceneHierarchyWindow"); //UnityEditor.InspectorWindow
            var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
            window = EditorWindow.GetWindow<PlayFabEditor>(inspWndType);
            window.titleContent = new GUIContent("PlayFab");

            EditorPrefs.SetBool("PlayFabToolsShown", true);
        }

        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                if (EditorPrefs.HasKey("PlayFabToolsShown") || !PlayFabEditorSDKTools.IsInstalled)
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
        }


        void OnGUI()
        {
            try
            {
                GUILayout.BeginVertical();

                //Run all updaters prior to drawing;  
                PlayFabEditorAuthenticate.Update();
                PlayFabEditorSettings.Update();

                PlayFabEditorHeader.DrawHeader(Progress);

                GUI.enabled = blockingRequests.Count > 0 ? false : true;

                if (PlayFabEditorAuthenticate.IsAuthenticated())
                {
                    //Try catching to avoid Draw errors that do not actually impact the functionality
                    //EG. Mismatch Draw Layout errors.
                    try
                    {
                        if (Progress >= .99f)
                        {
                            Progress = 0f;
                        }


                        PlayFabEditorMenu.DrawMenu();

                        switch (PlayFabEditorMenu._menuState)
                        {
                            case PlayFabEditorMenu.MenuStates.Sdks:
                                PlayFabEditorSDKTools.DrawSdkPanel();
                                break;
                            case PlayFabEditorMenu.MenuStates.Services:
                                break;
                            case PlayFabEditorMenu.MenuStates.Settings:
                                PlayFabEditorSettings.DrawSettingsPanel();
                                PlayFabEditorSettings.After(); //TODO why is this getting called every frame?
                                break;
                            case PlayFabEditorMenu.MenuStates.Help:
                                PlayFabEditorHelpMenu.DrawHelpPanel();
                                break;
                     
                            default:
                                break;
                        }

                    }
                    catch (Exception e)
                    {
                        //Do Nothing.
                        //Debug.LogException(e); // currently gettting a few errores: Getting control 1's position in a group with only 1 controls when doing Repaint
                    }
                }
                else
                {
                    PlayFabEditorAuthenticate.DrawAuthPanels();
                }

                if(UpdateLoopTick != null)
                {
                    UpdateLoopTick();
                } 

                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                    GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.EndVertical();

                PruneBlockingRequests();

                try
                {
                    Repaint();
                }
                catch
                {
                    //Do nothing.
                }

            }
            catch (Exception e)
            {
                //Do Nothing.. 
            }

        }



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
                case EdExStates.OnMenuItemClicked:
                    Debug.Log(string.Format("MenuItem: {0} Clicked", status));
                break;
//
//                case EdExStates.OnSubmenuItemClicked:
//                   
//                break;
//
                case EdExStates.OnHttpReq:
                    //JsonWrapper.SerializeObject(request, PlayFabEditorUtil.ApiSerializerStrategy);
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
                    //var httpResult = JsonWrapper.DeserializeObject<HttpResponseObject>(response, PlayFabEditorUtil.ApiSerializerStrategy);
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.off);
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.success);
                    ClearBlockingRequest(status);
                break;
//
                case EdExStates.OnError:

                    // deserialize and add json details
                    // clear blocking requests
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.error);
                    ClearBlockingRequest();
                    Debug.LogError(string.Format("PlayFab EditorExtensions: Caught an error:{0}", status)); 
                break;
//
//                case EdExStates.OnWaitBegin:
//                   
//                break;
//
//                case EdExStates.OnWaitEnd:
//                    Debug.LogError(string.Format("PlayFab EditorExtensions: Caught an error:{0}", status)); 
//                break;
                case EdExStates.OnWarning:
                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.warning);
                    ClearBlockingRequest();
                    Debug.LogWarning(string.Format("PlayFab EditorExtensions: {0}", status)); 
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
    }
}


public class PlayFabEdExSavedSettings
{
    
}

//public class PlayFab