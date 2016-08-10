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
        public static ListDisplay listDisplay;
        public static bool isEnabled = false;


        public static void OnEnable()
        {
            
            if ( listDisplay == null)
            {
                listDisplay = CreateInstance<ListDisplay>();
                listDisplay.displayTitle = "Title Data:";
                listDisplay.items.Add(new KvpItem("Key1", "Value1"));
                listDisplay.items.Add(new KvpItem("Key2", "Nullam id augue nibh. Proin commodo neque non sem fermentum finibus. Fusce tincidunt felis risus, semper feugiat diam aliquam a. Aliquam erat volutpat. Nunc euismod, turpis eu fringilla fermentum, ligula arcu elementum ipsum, eu interdum nibh sapien placerat nisi. Ut ipsum nisl, elementum eget nisl eget, lobortis lacinia tortor. Sed rhoncus velit fermentum sem varius, in accumsan mauris dignissim. Quisque id tempor ante. Nullam id augue nibh. Proin commodo neque non sem fermentum finibus. Fusce tincidunt felis risus, semper feugiat diam aliquam a. Aliquam erat volutpat. Nunc euismod, turpis eu fringilla fermentum, ligula arcu elementum ipsum, eu interdum nibh sapien placerat nisi. Ut ipsum nisl, elementum eget nisl eget, lobortis lacinia tortor. Sed rhoncus velit fermentum sem varius, in accumsan mauris dignissim. Quisque id tempor ante. "));

                listDisplay.settings = (BaseUiComponent.ComponentSettings.useScrollBar | BaseUiComponent.ComponentSettings.fillHorizontal);
                listDisplay.Init(new Rect(20,20,450,120), PlayFabEditor.window.position, Color.gray, PlayFabEditorHelper.uiStyle.GetStyle("listDisplay"));
                isEnabled = true;
            }
            else if(!isEnabled)
            {
                isEnabled = true;
                //listDisplay.OnEnable();
            }

//            if(listDisplay.postDrawCall == null)
//            {
//                listDisplay.postDrawCall = () => 
//                {
//                    GUILayout.TextArea(Event.current.mousePosition.ToString());
//
//                    EditorGUILayout.TextArea(listDisplay.bounds.ToString());
//                    EditorGUILayout.TextArea(listDisplay.parentBounds.ToString());
//                    if(GUILayout.Button("Submit"))
//                    {
//                        //BaseUiAnimationController.StartAlphaFade(1, 0, listDisplay);
//                    }
//                };
//            }
        }




        public static void OnDisable()
        {
            isEnabled = false;
            //listDisplay.OnDisable();
            //listDisplay = null;
        }


        static PlayFabEditorDataMenu()
        {
            Debug.LogWarning("INIT FIRED!");
            if(!PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate += StateUpdateHandler;
            }

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
}
