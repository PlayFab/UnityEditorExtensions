namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

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
                listDisplay.items.Add(new KvpItem("Key2", "Value2"));

                listDisplay.settings = (BaseUiComponent.ComponentSettings.useScrollBar | BaseUiComponent.ComponentSettings.fillHorizontal);
                listDisplay.Init(new Rect(20,20,450,120), PlayFabEditor.window.position, Color.gray, PlayFabEditorHelper.uiStyle.GetStyle("listDisplay"));
                isEnabled = true;
            }
            else if(!isEnabled)
            {
                isEnabled = true;
                listDisplay.OnEnable();
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
            listDisplay.OnDisable();
            //listDisplay = null;
        }

   }
}
