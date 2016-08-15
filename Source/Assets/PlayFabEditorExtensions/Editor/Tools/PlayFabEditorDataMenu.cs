namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    [InitializeOnLoad]
    public class PlayFabEditorDataMenu : Editor
    {
        public static TitleDataViewer tdViewer;
       

        public static bool isEnabled = false;
        public static MenuComponent menu = null;

        public enum DataMenuStates { TitleData }
        public static DataMenuStates currentState = DataMenuStates.TitleData;

        private static Vector2 scrollPos = Vector2.zero;

        static PlayFabEditorDataMenu()
        {
            if(!PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate += StateUpdateHandler;
            }

            RegisterMenu();
        }

        public static void OnTitleDataClicked()
        {
            currentState = DataMenuStates.TitleData;
        }


        public static void DrawDataPanel()
        {
            if(menu != null)
            {
                menu.DrawMenu();

                switch(currentState)
                {
                    case DataMenuStates.TitleData:
                        if(tdViewer == null)
                        {
                            tdViewer = ScriptableObject.CreateInstance<TitleDataViewer>();
                            foreach(var item in PlayFabEditorDataService.envDetails.titleData)
                            {
                                tdViewer.items.Add(new KvpItem(item.Key, item.Value));
                            }
                        }
                        else
                        {
                            scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                            tdViewer.Draw();
                            GUILayout.EndScrollView();
                        }
                       
                    break;

                    default:
                        EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                            GUILayout.Label("Coming Soon!", PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                        GUILayout.EndHorizontal();
                    break;
                   
                }
            }
            else
            {
                RegisterMenu();
            }

        }


        public static void OnDisable()
        {

        }
     

        public void OnDestroy()
        {
            if(PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate -= StateUpdateHandler;
            }
        }


        public static void RegisterMenu()
        {
            if ( menu == null)
            {
                menu = ScriptableObject.CreateInstance<MenuComponent>();
                menu.RegisterMenuItem("TITLE DATA", OnTitleDataClicked);
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
}
