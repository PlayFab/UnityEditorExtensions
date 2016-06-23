namespace PlayFab.Editor.EditorModels
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;

    public class DownloadSDKRequest
    {
        
    }

    public class DownloadSDKResponse
    {
        public byte[] data;
    }

    public class GetSDKVersionsRequest { }

    public class GetSDKVersionsResponse
    {
        public string description;
        public Dictionary<string, string> sdkVersion;
        public Dictionary<string, string> links;
    }
}