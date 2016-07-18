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

    public class RegisterAccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string StudioName { get; set; }
        public string DeveloperToolProductName { get; set; }
        public string DeveloperToolProductVersion { get; set; }
    }

    public class RegisterAccountResult
    {
        public string DeveloperClientToken { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeveloperToolProductName { get; set; }
        public string DeveloperToolProductVersion { get; set; }
    }


    public class LoginResult
    {
        public string DeveloperClientToken { get; set; }
    }


    public class GetStudiosRequest
    {
        public string DeveloperClientToken { get; set; }
    }

    public class GetStudiosResult
    {
        public Studio[] Studios { get; set; }
    }

    public class CreateTitleRequest
    {
        public string DeveloperClientToken { get; set; }

        public string Name { get; set; }

        public string StudioId { get; set; }
    }

    public class CreateTitleResult
    {
        public Title Title { get; set; }
    }

    public class Title
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SecretKey { get; set; }

        public string GameManagerUrl { get; set; }
    }

    public class Studio
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Title[] Titles { get; set; }
    }

    public class PlayFabError
    {
        public int HttpCode;
        public string HttpStatus;
        public PlayFabErrorCode Error;
        public string ErrorMessage;
        public Dictionary<string, List<string>> ErrorDetails;
        public object CustomData;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            if (ErrorDetails != null)
            {
                foreach (var kv in ErrorDetails)
                {
                    sb.Append(kv.Key);
                    sb.Append(": ");
                    sb.Append(string.Join(", ", kv.Value.ToArray()));
                    sb.Append(" | ");
                }
            }
            return string.Format("PlayFabError({0}, {1}, {2} {3}", Error, ErrorMessage, HttpCode, HttpStatus) + (sb.Length > 0 ? " - Details: " + sb.ToString() + ")" : ")");
        }
    }


    public class HttpResponseObject
    {
        public int code;
        public string status;
        public object data;
    }

}