using PlayFab.PfEditor.EditorModels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    public class PlayFabEditorToolsMenu : UnityEditor.Editor
    {
        public enum ToolSubMenuStates
        {
            CloudScript,
            QuickScript,
            Maps,
        }

        public static ToolSubMenuStates currentState = ToolSubMenuStates.CloudScript;

        public static float buttonWidth = 200;
        public static Vector2 scrollPos = Vector2.zero;
        private static SubMenuComponent _menu = null;

        private static void Draw3DMaps()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("3D Maps! 200+ possible cities!");
            if (GUILayout.Button("Add 3D Maps (+Examples)", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
            {
                var link = "https://github.com/microsoft/MapsSDK-Unity/releases/download/0.2.3/Microsoft.Maps.Unity-Core+Examples-0.2.3.unitypackage";
                System.Diagnostics.Process.Start(link);
            }
            if (GUILayout.Button("Add 3D Maps (Code Only)", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
            {
                var link = "https://github.com/microsoft/MapsSDK-Unity/releases/download/0.2.3/Microsoft.Maps.Unity-Core-0.2.3.unitypackage";
                System.Diagnostics.Process.Start(link);
            }
            GUILayout.FlexibleSpace();
        }

        private static bool quickStartActivated = false;

        private static bool tagStart = false;
        private static bool tagEnable = false;
        private static bool tagDisable = false;
        private static bool tagUpdate = false;
        private static bool tagLateUpdate = false;
        private static bool tagEnd = false;

        private static void DrawQuickStart()
        {
            GUILayout.FlexibleSpace();
            if (!quickStartActivated)
            {
                EditorGUILayout.LabelField("Auto-Tagger: Add telemetry calls to Assets you care about.");
                if (GUILayout.Button("AutoTag My Assets!", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                {
                    GetAssetFiles();
                    quickStartActivated = true;
                }
            }
            else
            {
                // TODO: v2 possibly have to add each of these 3 to all file options... may not look very good.
                // Can we pop out a window instead?

                // TODO: add option buttons:
                EditorGUILayout.LabelField("Start, only emits once per session");
                tagStart = GUILayout.Toggle(tagStart, "Tag Start");
                EditorGUILayout.LabelField("On Enable, emits anytime this object come back");
                tagEnable = GUILayout.Toggle(tagEnable, "Tag Enable");

                EditorGUILayout.LabelField("On Update");
                tagUpdate = GUILayout.Toggle(tagUpdate, "Tag Update");

                // STRETCH! howabout we try to find Late Update?
                EditorGUILayout.LabelField("On Late Update");
                tagLateUpdate = GUILayout.Toggle(tagLateUpdate, "Tag Late Update");

                // STRETCH! may need to add this function if it doesn't already exist.
                EditorGUILayout.LabelField("On Disable");
                tagDisable = GUILayout.Toggle(tagDisable, "Tag Disable");

                if (localAssets.Count < 1)
                {
                    EditorGUILayout.LabelField("No MonoBehaviors were found!");
                }
                else
                {
                    foreach (var file in localAssets)
                    {
                        // TODO: add these local assets to a check box list.
                        EditorGUILayout.LabelField(file);
                        GUILayout.Toggle(false, file);
                    }
                    if (GUILayout.Button("Add Telemetry", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        WriteAssetFiles();
                        quickStartActivated = false;
                    }
                }
            }
            GUILayout.FlexibleSpace();
        }

        private static void DrawCloudScript()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
            buttonWidth = EditorGUIUtility.currentViewWidth > 400 ? EditorGUIUtility.currentViewWidth / 2 : 200;

            using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
            {
                EditorGUILayout.LabelField("CLOUD SCRIPT:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));
                GUILayout.Space(10);
                if (GUILayout.Button("IMPORT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(30)))
                {
                    ImportCloudScript();
                }
                GUILayout.Space(10);
                if (File.Exists(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath))
                {
                    if (GUILayout.Button("REMOVE", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(30)))
                    {
                        PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath = string.Empty;
                        PlayFabEditorDataService.SaveEnvDetails();
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("EDIT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(30)))
                    {
                        EditorUtility.OpenWithDefaultApp(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath);
                    }
                }
            }

            if (File.Exists(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath))
            {
                var path = File.Exists(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath) ? PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath : PlayFabEditorHelper.CLOUDSCRIPT_PATH;
                var shortPath = "..." + path.Substring(path.LastIndexOf('/'));

                using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(shortPath, PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinWidth(110), GUILayout.MinHeight(20)))
                    {
                        EditorUtility.RevealInFinder(path);
                    }
                    //                            GUILayout.Space(10);
                    //                            if (GUILayout.Button("EDIT LOCALLY", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinWidth(90), GUILayout.MinHeight(20)))
                    //                            {
                    //                                EditorUtility.OpenWithDefaultApp(path);
                    //                            }
                    GUILayout.FlexibleSpace();
                }

                using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("SAVE TO PLAYFAB", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                    {
                        if (EditorUtility.DisplayDialog("Deployment Confirmation", "This action will upload your local Cloud Script changes to PlayFab?", "Continue", "Cancel"))
                        {
                            BeginCloudScriptUpload();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
            }
            else
            {
                using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("No Cloud Script files added. Import your file to get started.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public static void DrawToolsPanel()
        {
            if (_menu == null)
            {
                RegisterMenu();
                return;
            }

            _menu.DrawMenu();
            switch ((ToolSubMenuStates)PlayFabEditorPrefsSO.Instance.curSubMenuIdx)
            {
                case ToolSubMenuStates.CloudScript:
                    DrawCloudScript();
                    break;
                case ToolSubMenuStates.QuickScript:
                    DrawQuickStart();
                    break;
                case ToolSubMenuStates.Maps:
                    Draw3DMaps();
                    break;

                default:
                    using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
                    {
                        EditorGUILayout.LabelField("Coming Soon!", PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                    }
                    break;
            }
            GUILayout.EndScrollView();
        }

        private static void WriteAssetFiles()
        {
            // We may want to ask user if they want to add telemetry to Start or Update?
            foreach(var file in localAssets)
            {
                // TODO: finish this write, (ARE WE ALLOWED TO EVEN WRITE TO FILES?)
                var text = System.IO.File.ReadAllText(file);
                // TODO: get correct filename
                var filename = file.Split('\\');

                if(tagStart)
                {
                    // Look for the overload for Start()
                    if(text.Contains("IEnumerable Start()") || text.Contains("void Start()"))
                    {
                        // text.
                        // Add in the Playstream event
                    }
                }

                if(tagEnable)
                {
                    // Look for the overload for OnEnable
                }

                if(tagUpdate)
                {
                    
                }

                if(tagLateUpdate)
                {

                }

                if(tagDisable)
                {

                }
            }
        }

        private static void GetAssetFiles()
        {
            List<string> possibleTag = new List<string>();
            var projAssetPath = Application.dataPath;
            DirSearch(projAssetPath);
        }

        private static List<string> localAssets = new List<string>();
        static void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    if (!d.Contains("PlayFab"))
                    {
                        foreach (string f in Directory.GetFiles(d))
                        {
                            if (f.EndsWith(".cs"))
                            {
                                string fileText = System.IO.File.ReadAllText(f);
                                if (fileText.Contains(": MonoBehaviour"))
                                {
                                    // May be faster to keep a ref to that file itself. 
                                    // right now we are loading all text in, which can be severly slow.
                                    localAssets.Add(f);
                                }
                            }
                            Console.WriteLine(f);
                        }
                        DirSearch(d);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void ImportCloudScript()
        {
            var dialogResponse = EditorUtility.DisplayDialogComplex("Selcet an Import Option", "What Cloud Script file do you want to import?", "Use my latest PlayFab revision", "Cancel", "Use my local file");
            switch (dialogResponse)
            {
                case 0:
                    // use PlayFab
                    GetCloudScriptRevision();
                    break;
                case 1:
                    // cancel
                    return;
                case 2:
                    //use local
                    SelectLocalFile();
                    break;
            }
        }

        private static void GetCloudScriptRevision()
        {
            // empty request object gets latest versions
            PlayFabEditorApi.GetCloudScriptRevision(new EditorModels.GetCloudScriptRevisionRequest(), (GetCloudScriptRevisionResult result) =>
            {
                var csPath = PlayFabEditorHelper.CLOUDSCRIPT_PATH;
                var location = Path.GetDirectoryName(csPath);
                try
                {
                    if (!Directory.Exists(location))
                        Directory.CreateDirectory(location);
                    if (!File.Exists(csPath))
                        using (var newfile = File.Create(csPath)) { }
                    File.WriteAllText(csPath, result.Files[0].FileContents);
                    Debug.Log("CloudScript uploaded successfully!");
                    PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath = csPath;
                    PlayFabEditorDataService.SaveEnvDetails();
                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    // PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                    return;
                }
            }, PlayFabEditorHelper.SharedErrorCallback);
        }

        private static void SelectLocalFile()
        {
            var starterPath = File.Exists(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath) ? Application.dataPath : PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath;
            var cloudScriptPath = string.Empty;
            cloudScriptPath = EditorUtility.OpenFilePanel("Select your Cloud Script file", starterPath, "js");

            if (!string.IsNullOrEmpty(cloudScriptPath))
            {
                PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath = cloudScriptPath;
                PlayFabEditorDataService.SaveEnvDetails();
            }
        }

        private static void BeginCloudScriptUpload()
        {
            var filePath = File.Exists(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath) ? PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath : PlayFabEditorHelper.CLOUDSCRIPT_PATH;

            if (!File.Exists(PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath) && !File.Exists(PlayFabEditorHelper.CLOUDSCRIPT_PATH))
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, "Cloud Script Upload Failed: null or corrupt file at path(" + filePath + ").");
                return;
            }

            var s = File.OpenText(filePath);
            var contents = s.ReadToEnd();
            s.Close();

            var request = new UpdateCloudScriptRequest();
            request.Publish = EditorUtility.DisplayDialog("Deployment Options", "Do you want to make this Cloud Script live after uploading?", "Yes", "No");
            request.Files = new List<CloudScriptFile>(){
                new CloudScriptFile() {
                    Filename = PlayFabEditorHelper.CLOUDSCRIPT_FILENAME,
                    FileContents = contents
                }
            };

            PlayFabEditorApi.UpdateCloudScript(request, (UpdateCloudScriptResult result) =>
            {
                PlayFabEditorPrefsSO.Instance.LocalCloudScriptPath = filePath;
                PlayFabEditorDataService.SaveEnvDetails();

                Debug.Log("CloudScript uploaded successfully!");

            }, PlayFabEditorHelper.SharedErrorCallback);
        }
        public static void RegisterMenu()
        {
            if (_menu != null)
                return;

            _menu = CreateInstance<SubMenuComponent>();
            _menu.RegisterMenuItem("CloudScript", OnCloudScriptClicked);
            _menu.RegisterMenuItem("QuickStart", OnQuickStartClicked);
            _menu.RegisterMenuItem("MapsSDK", OnMapsSdkClicked);
        }
        public static void OnCloudScriptClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, ToolSubMenuStates.CloudScript.ToString(), "" + (int)ToolSubMenuStates.CloudScript);
        }

        public static void OnQuickStartClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, ToolSubMenuStates.QuickScript.ToString(), "" + (int)ToolSubMenuStates.QuickScript);
        }

        public static void OnMapsSdkClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, ToolSubMenuStates.Maps.ToString(), "" + (int)ToolSubMenuStates.Maps);
        }
    }
}
