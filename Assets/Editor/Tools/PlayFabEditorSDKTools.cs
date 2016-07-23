using System;
using System.Linq;

namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEditor;

    public class PlayFabEditorSDKTools : Editor
    {
        internal static bool IsInstalled = IsSDKInstalled();
        private static Vector3 ColorVector = PlayFabEditorHelper.GetColorVector(62);
        private static Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(ColorVector.x, ColorVector.y, ColorVector.z));
        private static string SdkVersion = string.Empty;
        private static string latestSdkVersion = string.Empty;
        private static bool isCheckingLatestVersion = false;

        public static void DrawSdkPanel()
        {
            if (IsInstalled)
            {
                //SDK is installed.
                string version = CheckSdkVersion();

                if(isCheckingLatestVersion == false)
                {
                    GetLatestSdkVersion();
                }

                GUIStyle labelStyle = new GUIStyle(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 18;


                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                    GUILayout.Label(string.Format("SDK v{0} is installed!", string.IsNullOrEmpty(SdkVersion) ? "Unknown" : SdkVersion), labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));


                GUILayout.EndVertical();

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

                if(ShowSDKUpgrade())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Upgrade to " + latestSdkVersion, PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
                    {
                        UpgradeSdk();
                    }
                    GUILayout.FlexibleSpace();
                
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("You have the latest SDK!", labelStyle, GUILayout.MinHeight(32));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                //NO SDK Is Installed..

                //Create a GUI Style
//                var style = new GUIStyle();
//                //Set the fixed height of this container
//                style.fixedHeight = 100f;
//                style.margin.top = 10;
//                style.normal.background = Background;
//
//                var textStyle = PlayFabEditorHelper.GetTextButtonStyle();
//                textStyle.alignment = TextAnchor.MiddleCenter;
//                textStyle.fontSize = 14;
                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

                GUIStyle labelStyle = new GUIStyle(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 18;

                GUILayout.Label("No SDK is installed.", labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                GUILayout.Space(20);

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                var buttonWidth = 150;

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Install PlayFab SDK", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32)))
                {
                    ImportSDK();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            //CheckSdkVersion();
        }

        public static void ImportSDK()
        {
            //
            PlayFabEditorHttp.MakeDownloadCall("https://github.com/PlayFab/UnitySDK/raw/versioned/Packages/UnitySDK.unitypackage" ,(fileName) => 
            {
                Debug.Log("PlayFab SDK Complete");
                AssetDatabase.ImportPackage(fileName, false); 

            }, (error) => { Debug.LogError(error.ErrorMessage); });
        }

        private static bool IsSDKInstalled()
        {
            var playfabVersionType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabSettings"
                        select type);
            
            //Debug.Log(playfabVersionType.GetType());

            return playfabVersionType.ToList().Count > 0;
        }


        //TODO clean this up...
        private static string CheckSdkVersion()
        {
            if(string.IsNullOrEmpty(SdkVersion))
            {
                
                List<Type> types = new List<Type>();

                // check for older SDK versions first
                var oldPlayfabVersionTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabVersion"
                        select type);

                types = oldPlayfabVersionTypes.ToList();

                if(types.Count > 0)
                {
                    foreach(var type in types)
                    {
                        var properties = (from property in type.GetProperties()
                            where property.Name == "SdkVersion" || property.Name == "SdkRevision" 
                            select property);

                        foreach(var prop in properties)
                        {
                            SdkVersion += prop.GetValue(prop,null).ToString();
                        }
                    }
                    if(!String.IsNullOrEmpty(SdkVersion))
                    {
                        return SdkVersion;
                    }
                }
            }

            if(string.IsNullOrEmpty(SdkVersion))
            {
                
                List<Type> types = new List<Type>();

                // check for older SDK versions first
                var oldPlayfabVersionTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabSettings"
                        select type);

                types = oldPlayfabVersionTypes.ToList();

                if(types.Count > 0)
                {
                    foreach(var type in types)
                    {
                        var fields = (from field in type.GetFields()
                            where field.Name == "SdkVersion" || field.Name == "SdkRevision" 
                            select field);

                        foreach(var f in fields)
                        {
                            SdkVersion += f.GetValue(f).ToString();
                        }
                    }
                    if(!String.IsNullOrEmpty(SdkVersion))
                    {
                        return SdkVersion;
                    }
                }
            }
            return null;


            // check for less old SDK versions next

            // check for newer SDK versions last  
        }


        private static bool ShowSDKUpgrade()
        {
            if(string.IsNullOrEmpty(latestSdkVersion) || latestSdkVersion == "Unknown")
            {
                return false;
            }


            if(string.IsNullOrEmpty(SdkVersion))
            {
                return true;
            }

           string[] currrent = SdkVersion.Split('.');
           string[] latest = latestSdkVersion.Split('.');

           if(int.Parse(latest[0]) > int.Parse(currrent[0]))
           {
                return true;
           }
            else if(int.Parse(latest[1]) > int.Parse(currrent[1]))
           {
                return true;
           }
            else if(int.Parse(latest[2]) > int.Parse(currrent[2]))
           {
                return true;
           }

           return false;
        }

        private static void UpgradeSdk()
        {
            Debug.LogError("SDK Upgrade not yet implemented...");
            throw new System.NotImplementedException("SDK Upgrade not yet implemented...");
        }

        private static void GetLatestSdkVersion()
        {
            if(string.IsNullOrEmpty(latestSdkVersion))
            {
                isCheckingLatestVersion = true;
                PlayFabEditorHttp.MakeGitHubApiCall("https://api.github.com/repos/PlayFab/UnitySDK/git/refs/tags", (version) => 
                {
                    isCheckingLatestVersion = false;
                    latestSdkVersion = version ?? "Unknown";
                    //Debug.Log(latestSdkVersion);
                }, (error) => 
                { 
                    isCheckingLatestVersion = false;
                });
            }
        }
    }
}