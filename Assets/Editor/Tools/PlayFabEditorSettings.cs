using System.Text;

namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    public class PlayFabEditorSettings : Editor
    {
        internal static List<string> buildTargets; 
        private const string AdminAPI = "ENABLE_PLAYFABADMIN_API";
        private const string ServerAPI = "ENABLE_PLAYFABSERVER_API";
        private const string DebugRequestTiming = "PLAYFAB_REQUEST_TIMING";

        private static Texture2D CheckmarkIconOn =
            EditorGUIUtility.Load("Assets/Editor/images/checkmark_on.png") as Texture2D;
        private static Texture2D CheckmarkIconOff =
            EditorGUIUtility.Load("Assets/Editor/images/checkmark_off.png") as Texture2D;

        private static bool _isAdminSdkEnabled;
        private static bool _isServerSdkEnabled;
        private static bool _IsDebugRequestTiming;

        private static Vector3 _colorVector = PlayFabEditorHelper.GetColorVector(62);
        private static readonly Texture2D Background = PlayFabEditorHelper.MakeTex(1, 1, new Color(_colorVector.x, _colorVector.y, _colorVector.z));

        public static void DrawSettingsPanel()
        {
            var style = PlayFabEditorHelper.GetTextButtonStyle();
            style.fixedHeight = 135;
            style.normal.background = Background;
            style.hover.background = Background;

            var toggleStyle = new GUIStyle();
            toggleStyle.normal.background = CheckmarkIconOff;
            toggleStyle.hover.background = CheckmarkIconOff;
            toggleStyle.active.background = CheckmarkIconOff;

            toggleStyle.onNormal.background = CheckmarkIconOn;
            toggleStyle.onHover.background = CheckmarkIconOn;
            toggleStyle.onActive.background = CheckmarkIconOn;

            toggleStyle.fixedHeight = 20;
            toggleStyle.fixedWidth = 20;
            
            //textFieldStyle.normal.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
            //textFieldStyle.hover.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));
            //textFieldStyle.active.background = PlayFabEditorHelper.MakeTex(1, 1, PlayFabEditorHelper.GetColor(255, 255, 255));

            var labelStyle = PlayFabEditorHelper.GetTextButtonStyle();
            labelStyle.font = PlayFabEditorHelper.buttonFontBold;
            labelStyle.fontSize = 14;
            labelStyle.alignment = TextAnchor.MiddleLeft;

            GUILayout.Space(10);
            GUILayout.BeginVertical(style);
            GUILayout.Space(10);
            var adminLabel = new GUIContent("Is Admin API Enabled: ");
            using (new FixedWidthLabel(adminLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(adminLabel).x - 40);
                _isAdminSdkEnabled = EditorGUILayout.Toggle(_isAdminSdkEnabled, toggleStyle,  GUILayout.MinHeight(25));
                
            }

            GUILayout.Space(10);

            var serverLabel = new GUIContent("Is Server API Enabled: ");
            using (new FixedWidthLabel(serverLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(serverLabel).x - 40);
                _isServerSdkEnabled = EditorGUILayout.Toggle(_isServerSdkEnabled, toggleStyle, GUILayout.MinHeight(25));

            }

            GUILayout.Space(10);

            var debugRequestLabel = new GUIContent("Debug Request Times: ");
            using (new FixedWidthLabel(debugRequestLabel, labelStyle))
            {
                GUILayout.Space(EditorGUIUtility.currentViewWidth - labelStyle.CalcSize(debugRequestLabel).x - 40);
                _IsDebugRequestTiming = EditorGUILayout.Toggle(_IsDebugRequestTiming, toggleStyle, GUILayout.MinHeight(25));

            }

            GUILayout.EndVertical();
        }

        public static void Update()
        {
            buildTargets = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            if (EditorPrefs.HasKey(AdminAPI))
            {
                _isAdminSdkEnabled = true;
            }
            if (EditorPrefs.HasKey(ServerAPI))
            {
                _isServerSdkEnabled = true;
            }
            if (EditorPrefs.HasKey(DebugRequestTiming))
            {
                _IsDebugRequestTiming = true;
            }
        }

        public static void After()
        {
            if (_isAdminSdkEnabled && !buildTargets.Contains(AdminAPI))
            {
                var str = AddToBuildTarget(buildTargets, AdminAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(AdminAPI,"1");
            }
            else if (!_isAdminSdkEnabled && buildTargets.Contains(AdminAPI))
            {
                var str = RemoveToBuildTarget(buildTargets, AdminAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(AdminAPI);
            }

            if (_isServerSdkEnabled && !buildTargets.Contains(ServerAPI))
            {
                var str = AddToBuildTarget(buildTargets, ServerAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(ServerAPI, "1");
            }
            else if (!_isServerSdkEnabled && buildTargets.Contains(ServerAPI))
            {
                var str = RemoveToBuildTarget(buildTargets, ServerAPI);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(ServerAPI);
            }

            if (_IsDebugRequestTiming && !buildTargets.Contains(DebugRequestTiming))
            {
                var str = AddToBuildTarget(buildTargets, DebugRequestTiming);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.SetString(DebugRequestTiming, "1");
            }
            else if (!_IsDebugRequestTiming && buildTargets.Contains(DebugRequestTiming))
            {
                var str = RemoveToBuildTarget(buildTargets, DebugRequestTiming);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                EditorPrefs.DeleteKey(DebugRequestTiming);
            }

        }

        public static string AddToBuildTarget(List<string> targets, string define)
        {
            targets.Add(define);
            return string.Join(";", targets.ToArray());
        }

        public static string RemoveToBuildTarget(List<string> targets, string define)
        {
            targets.Remove(define);
            return string.Join(";", targets.ToArray());
        }


    }
}