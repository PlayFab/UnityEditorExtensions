namespace PlayFab.Editor
{
    using UnityEngine;
    using System;
    using System.Collections;
    using UnityEditor;
    using PlayFab.Editor.EditorModels;

    using System.Collections.Generic;
    using System.Linq;

    public class PlayFabEditorApi
    {
        public const string ApiEndpoint = @"https://editor.playfabapi.com"; //@"https://p-mns-unity-editor-api.us-west-2.elasticbeanstalk.com";
        public const string TitleEndPoint = @".playfabapi.com";

        public static void DownloadSDK(DownloadSDKRequest request, Action<DownloadSDKResponse> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
           // PlayFabEditorHttp.MakeDownloadCall("", "", request, null, resultCallback, errorCallback);
        }

        public static void GetSDKVersion(GetSDKVersionsRequest request, Action<GetSDKVersionsResponse> resultCallback, Action<EditorModels.PlayFabError> errorCallback)
        { 
         
            //PlayFabEditorHttp.MakeApiCall("", "", request, null, resultCallback, errorCallback);
        }


        public static void RegisterAccouint(RegisterAccountRequest request, Action<RegisterAccountResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/RegisterAccount", ApiEndpoint, null, request, resultCallback, errorCallback);            
        }

        public static void Login(LoginRequest request, Action<LoginResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/Login", ApiEndpoint, null, request, resultCallback, errorCallback);
        }

        public static void GetStudios(GetStudiosRequest request, Action<GetStudiosResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            var token = EditorPrefs.GetString("PlayFabDevClientToken");
            request.DeveloperClientToken = token;
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/GetStudios", ApiEndpoint, token, request, resultCallback, errorCallback);
        }

        public static void CreateTitle(CreateTitleRequest request, Action<RegisterAccountResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            var token = EditorPrefs.GetString("PlayFabDevClientToken");
            request.DeveloperClientToken = token;
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/CreateTitle", ApiEndpoint, token, request, resultCallback, errorCallback);
        }





        public static void GetTitleData( Action<GetTitleDataResult> resultCb, Action<EditorModels.PlayFabError> errorCallback)
        {
            //TODO this will break without the SDK installed...
            var titleId = PlayFabSettings.TitleId;
            var apiEndpoint = String.Format("https://{0}{1}", titleId, TitleEndPoint);
            PlayFabEditorHttp.MakeApiCall<GetTitleDataRequest, GetTitleDataResult>("/Server/GetTitleData", apiEndpoint, null, new GetTitleDataRequest(), resultCb, errorCallback);
        }


    }
}