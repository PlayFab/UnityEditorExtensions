namespace PlayFab.Editor
{
    using UnityEngine;
    using System;
    using System.Collections;
    using UnityEditor;
    using PlayFab.Editor.EditorModels;
    public class PlayFabEditorApi 
    {

        public static void DownloadSDK(DownloadSDKRequest request, Action<DownloadSDKResponse> resultCallback,
            Action<PlayFabEditorHttp.PlayFabError> errorCallback)
        {
            PlayFabEditorHttp.MakeDownloadCall("", "", request, null, resultCallback, errorCallback);
        }

        public static void GetSDKVersion(GetSDKVersionsRequest request, Action<GetSDKVersionsResponse> resultCallback, Action<PlayFabEditorHttp.PlayFabError> errorCallback)
        { 
         
            PlayFabEditorHttp.MakeApiCall("", "", request, null, resultCallback, errorCallback);
        }

    }
}