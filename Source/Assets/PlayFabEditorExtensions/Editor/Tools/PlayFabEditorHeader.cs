namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorHeader : Editor
    {
        //private enum ProgressBarStates { off = 0, on = 1, reverse = 2}
        //private static ProgressBarStates currentProgressBarState = ProgressBarStates.off;
        public static ProgressBar progressBar = new ProgressBar();

        public static void DrawHeader(float progress = 0f)
        {

            //using Begin Vertical as our container.
            GUILayout.BeginHorizontal(GUILayout.Height(52));
           

            //Set the image in the container
            if (EditorGUIUtility.currentViewWidth < 375)
            {
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("pfLogo"), GUILayout.MaxHeight(40), GUILayout.Width(186));
            }
            else
            {
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("pfLogo"), GUILayout.MaxHeight(50), GUILayout.Width(233));
            }

//            if (GUILayout.Button("TEST BAR", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(105)))
//            {
////                 var at = PlayFabEditorDataService.activeTitle;
////                 Debug.Log(at);
//               if(ProgressBar.currentProgressBarState == ProgressBar.ProgressBarStates.off)
//               {
//                    PlayFabEditor.AddBlockingRequest("TEST BAR");
//                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.spin);
//               }
//               else
//               {
//                    PlayFabEditor.ClearBlockingRequest();
//                    ProgressBar.UpdateState(ProgressBar.ProgressBarStates.off);
//               }
//            }


            float gmAnchor = EditorGUIUtility.currentViewWidth - 30;


                if (EditorGUIUtility.currentViewWidth > 375)
                {
                    gmAnchor = EditorGUIUtility.currentViewWidth - 140;
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, 140, 42));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("GAME MANAGER", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(105)))
                    {
                        OnDashbaordClicked();
                    }
                }
                else
                {
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, EditorGUIUtility.currentViewWidth * .25f, 42));
                    GUILayout.BeginHorizontal();
                }

                if (GUILayout.Button("", PlayFabEditorHelper.uiStyle.GetStyle("gmIcon")))
                    {
                        OnDashbaordClicked();
                    }
               GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //end the vertical container
            GUILayout.EndHorizontal();

            //define a progress bar or just create empty space where the bar would go.
//            if (progress > 0)
//            {
//                DrawProgressBar(progress); 
//            }
            ProgressBar.Draw();




            /*
            else
            {
                GUILayout.Space(10);
            }*/

        }

        private static void DrawProgressBar(float progress) //progress = 0 -> 1
        {
            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("progressBarBg"));
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("progressBarFg"), GUILayout.Width(EditorGUIUtility.currentViewWidth * progress));
            GUILayout.EndHorizontal();
        }

        private static void OnDashbaordClicked()
        {
            Debug.Log("Dashboard Clicked");

            //TODO use saved URL 
            var url = @"https://developer.playfab.com";


            Help.BrowseURL(EditorPrefs.HasKey("PlayFabActiveTitleUrl") ? EditorPrefs.GetString("PlayFabActiveTitleUrl") : url);
        }

    }

    public class ProgressBar
    {
        public enum ProgressBarStates { off = 0, on = 1, spin = 2, error = 3, warning = 4, success = 5 }
        public static ProgressBarStates currentProgressBarState = ProgressBarStates.off;

        public static float progress = 0;
        private static GUIStyle pbarStyle = PlayFabEditorHelper.uiStyle.GetStyle("progressBarFg");
        private static float progressWidth = 0;
        private static float animationSpeed = 1f;
        private static float tickRate = .15f;
        private static float stTime;
        private static float endTime;
        private static float lastUpdateTime;
        private static bool isReveresed;

        public static void UpdateState(ProgressBarStates state)
        {
            if(currentProgressBarState == ProgressBarStates.off && state != ProgressBarStates.off)
            {
                stTime = (float)EditorApplication.timeSinceStartup;
                endTime = stTime + animationSpeed;
            }


           currentProgressBarState = state;

        }

        public static void UpdateProgress(float p)
        {
            progress = p;
        }



        public static void Draw()
        {
            //TODO explore how to better handle conc
            if(currentProgressBarState == ProgressBarStates.off)
            {
                stTime = 0;
                endTime = 0;
                progressWidth = 0;
                lastUpdateTime = 0;
                isReveresed = false;
                return;
            }
            else if(EditorWindow.focusedWindow != PlayFabEditor.window)
            {
                // pause draw while we are in the bg
                return;
            }
            else if(currentProgressBarState == ProgressBarStates.success)
            {
                if((float)EditorApplication.timeSinceStartup - stTime < animationSpeed)
                {
                    progressWidth = EditorGUIUtility.currentViewWidth;
                    pbarStyle = PlayFabEditorHelper.uiStyle.GetStyle("progressBarSuccess");
                }
                else if(PlayFabEditor.blockingRequests.Count > 0)
                {
                    UpdateState(ProgressBarStates.spin);
                }
                else
                {
                    UpdateState(ProgressBarStates.off);
                }
            }
            else if(currentProgressBarState == ProgressBarStates.warning)
            {
                if((float)EditorApplication.timeSinceStartup - stTime < animationSpeed)
                {
                    progressWidth = EditorGUIUtility.currentViewWidth;
                    pbarStyle = PlayFabEditorHelper.uiStyle.GetStyle("progressBarWarn");
                }
                else if(PlayFabEditor.blockingRequests.Count > 0)
                {
                    UpdateState(ProgressBarStates.spin);
                }
                else
                {
                    UpdateState(ProgressBarStates.off);
                }
            }
            else if(currentProgressBarState == ProgressBarStates.error)
            {
                if((float)EditorApplication.timeSinceStartup - stTime < animationSpeed)
                {
                    progressWidth = EditorGUIUtility.currentViewWidth;
                    pbarStyle = PlayFabEditorHelper.uiStyle.GetStyle("progressBarError");
                }
                else if(PlayFabEditor.blockingRequests.Count > 0)
                {
                    UpdateState(ProgressBarStates.spin);
                }
                else
                {
                    UpdateState(ProgressBarStates.off);
                }
            }
            else
            {
               
                if( (float)EditorApplication.timeSinceStartup - lastUpdateTime > tickRate)
                {
                    lastUpdateTime = (float)EditorApplication.timeSinceStartup;
                    pbarStyle = PlayFabEditorHelper.uiStyle.GetStyle("progressBarFg");

                    if(currentProgressBarState == ProgressBarStates.on)
                    {
                        progressWidth = EditorGUIUtility.currentViewWidth * progress;
                    }
                    else if(currentProgressBarState == ProgressBarStates.spin)
                    {
                        var currentTime = (float)EditorApplication.timeSinceStartup;
                        if(currentTime < endTime && !isReveresed)
                        { 
                            UpdateProgress((currentTime - stTime) / animationSpeed);  
                            progressWidth = EditorGUIUtility.currentViewWidth * progress;
                        }
                        else if(currentTime < endTime && isReveresed)
                        {
                            UpdateProgress((currentTime - stTime) / animationSpeed);
                            progressWidth = EditorGUIUtility.currentViewWidth - EditorGUIUtility.currentViewWidth * progress;
                        }
                        else
                        {
                            isReveresed = !isReveresed;
                            stTime = (float)EditorApplication.timeSinceStartup;
                            endTime = stTime + animationSpeed;
                        }
                    }
                }
            
            }

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("progressBarBg"));
                if(isReveresed)
                {
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Label("", pbarStyle, GUILayout.Width(progressWidth));
            GUILayout.EndHorizontal();
        }

        //TODO subscribe for stateupdates
        //static 

    }



}



