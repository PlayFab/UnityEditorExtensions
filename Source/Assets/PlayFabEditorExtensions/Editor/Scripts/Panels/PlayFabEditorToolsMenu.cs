using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using PlayFab.Editor.EditorModels;

namespace PlayFab.Editor
{
    public class PlayFabEditorToolsMenu : UnityEditor.Editor
    {
        public static float buttonWidth = 200;
        public static Vector2 scrollPos = Vector2.zero;

        public static void DrawToolsPanel()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                buttonWidth = EditorGUIUtility.currentViewWidth > 400 ? EditorGUIUtility.currentViewWidth/2 : 200;

                GUILayout.BeginVertical();
               

                    GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("UPLOAD CLOUD SCRIPT", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                        {
                            BeginCloudScriptUpload();
                        }

                        GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

         
                GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        public static void BeginCloudScriptUpload()
        {
            string filePath = EditorUtility.OpenFilePanel("Select your Cloud Script file", Application.dataPath, "js");

            if(string.IsNullOrEmpty(filePath))
                return;

            StreamReader s = File.OpenText(filePath);
            string contents = s.ReadToEnd();
            s.Close();

            if(string.IsNullOrEmpty(filePath))
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, "Cloud Script file was null or corrupted. Update cannot continue.");
                return;
            }

            UpdateCloudScriptRequest request = new UpdateCloudScriptRequest();
            request.Publish = EditorUtility.DisplayDialog("Deployment Options", "Do you want to make this Cloud Script live after uploading?", "Yes", "No");
            request.Files = new List<CloudScriptFile>(){ 
                new CloudScriptFile() { 
                    Filename = "CloudScript.js",
                    FileContents = contents
                }
            };

            PlayFabEditorApi.UpdateCloudScript(request, (UpdateCloudScriptResult result) => {
                Debug.Log("CloudScript uploaded successfully!");

            }, PlayFabEditorHelper.SharedErrorCallback);

        }
    }





}
