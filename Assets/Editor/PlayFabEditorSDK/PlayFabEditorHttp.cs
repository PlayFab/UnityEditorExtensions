using PlayFab.Editor.EditorModels;
using UnityEngine;

namespace PlayFab.Editor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
#if UNITY_5_4
    using UnityEngine.Networking;
#else
    using UnityEngine.Experimental.Networking;
#endif
    using PlayFab.Editor.Json;
    using PlayFab.Editor.EditorModels;

    public class PlayFabEditorHttp : Editor
    {

        internal static void MakeDownloadCall(string url, Action<string> resultCallback, Action<EditorModels.PlayFabError> errorCallback)
        {
            
            //var www = new UnityWebRequest(url);
            var www = new WWW(url);

            EditorCoroutine.start(PostDownload(www, (response) =>
            {
                string fileSaveLocation = string.Format(Application.dataPath + "/Editor/Tools/Resources/PlayFabUnitySdk.unitypackage");
                System.IO.File.WriteAllBytes(fileSaveLocation, response);
                resultCallback(fileSaveLocation);

            }, (error) => 
            {
                errorCallback(new EditorModels.PlayFabError(){ErrorMessage = error});
            }), www);
        }


        internal static void MakeApiCall<TRequestType, TResultType>(string api, string apiEndpoint, string token, TRequestType request,
            Action<TResultType> resultCallback, Action<EditorModels.PlayFabError> errorCallback)
        {
            var url = apiEndpoint + api;
            var req = JsonWrapper.SerializeObject(request, PlayFabEditorUtil.ApiSerializerStrategy);
            //Set headers
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"X-ReportErrorAsSuccess", "true"},
                {"X-PlayFabSDK", "PlayFabEditorExtensionsSDK_0.1.0"}
            };

            //Encode Payload
            var payload = System.Text.Encoding.UTF8.GetBytes(req.Trim());

            var www = new WWW(url, payload, headers);

            EditorCoroutine.start(Post(www, (response) =>
            {
                var httpResult = JsonWrapper.DeserializeObject<HttpResponseObject>(response,
                       PlayFabEditorUtil.ApiSerializerStrategy);

                if (httpResult.code == 200)
                {
                    try
                    {
                        var dataJson = JsonWrapper.SerializeObject(httpResult.data,
                            PlayFabEditorUtil.ApiSerializerStrategy);
                        var result = JsonWrapper.DeserializeObject<TResultType>(dataJson,
                            PlayFabEditorUtil.ApiSerializerStrategy);

                        if (resultCallback != null)
                        {
                            resultCallback(result);
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogException(e);
                    }

                }
                else
                {
                    if (errorCallback != null)
                    {
                        var playFabError = GeneratePlayFabError(response);
                        errorCallback(playFabError);
                    }
                }


            }, (error) =>
            {
                if (errorCallback != null)
                {
                    var playFabError = GeneratePlayFabError(error);
                    errorCallback(playFabError);
                }
            }),www);

        }


        internal static void MakeGitHubApiCall(string url, Action<string> resultCallback, Action<EditorModels.PlayFabError> errorCallback)
        {
            var www = new WWW(url);

            EditorCoroutine.start(Post(www, (response) =>
            {
                List<Object> jsonResponse = JsonWrapper.DeserializeObject<List<Object>>(response);

                // list seems to come back in ascending order (oldest -> newest)
                if(jsonResponse != null && jsonResponse.Count > 0)
                {
                    JsonObject latestSdkTag = (JsonObject)jsonResponse[jsonResponse.Count -1];

                    object tag;
                    if(latestSdkTag.TryGetValue("ref", out tag))
                    {
                        if(resultCallback != null)
                        {
                            int startIndex = tag.ToString().LastIndexOf('/')+1;
                            int length = tag.ToString().Length - startIndex;   
                            resultCallback(tag.ToString().Substring(startIndex, length));
                        }
                    }
                    else
                    {
                        if(resultCallback != null)
                        {
                            resultCallback(null);
                        }  
                    }
                    return;
                }


//                if (httpResult.code == 200)
//                {
//                    try
//                    {
//                        var dataJson = JsonWrapper.SerializeObject(httpResult.data,
//                            PlayFabEditorUtil.ApiSerializerStrategy);
//                        var result = JsonWrapper.DeserializeObject<TResultType>(dataJson,
//                            PlayFabEditorUtil.ApiSerializerStrategy);
//
//                        if (resultCallback != null)
//                        {
//                            resultCallback(result);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        UnityEngine.Debug.LogException(e);
//                    }
//
//                }
//                else
//                {
//                    if (errorCallback != null)
//                    {
//                        var playFabError = GeneratePlayFabError(response);
//                        errorCallback(playFabError);
//                    }
//                }


            }, (error) =>
            {
                if (errorCallback != null)
                {
                    var playFabError = GeneratePlayFabError(error);
                    errorCallback(playFabError);
                }
            }),www);

        }

        private static IEnumerator Post(WWW www, Action<string> callBack, Action<string> errorCallback)
        {
            yield return www;
            
            if (!string.IsNullOrEmpty(www.error))
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.text);
            }
        }

       //private static IEnumerator PostDownload(UnityWebRequest www, Action<byte[]> callBack, Action<string> errorCallback)
        private static IEnumerator PostDownload(WWW www, Action<byte[]> callBack, Action<string> errorCallback)
        {
            //www.downloadHandler = new UnityEngine.Experimental.Networking.DownloadHandlerBuffer();
           
            yield return www;//www.Send();

            if (!string.IsNullOrEmpty(www.error))
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.bytes);
            }
            yield break;
        }


        protected internal static PlayFabError GeneratePlayFabError(string json, object customData = null)
        {
            JsonObject errorDict = null;
            Dictionary<string, List<string>> errorDetails = null;
            try
            {
                //deserialize the error
                errorDict = JsonWrapper.DeserializeObject<JsonObject>(json, PlayFabEditorUtil.ApiSerializerStrategy);

                
                if (errorDict.ContainsKey("errorDetails"))
                {
                    var ed =
                        JsonWrapper.DeserializeObject<Dictionary<string, List<string>>>(
                            errorDict["errorDetails"].ToString());
                    errorDetails = ed;
                }
            }
            catch (Exception e)
            {
                return new PlayFabError()
                {
                    ErrorMessage = json
                };
            }
            //create new error object
            return new PlayFabError
            {
                HttpCode = errorDict.ContainsKey("code") ? Convert.ToInt32(errorDict["code"]) : 400,
                HttpStatus = errorDict.ContainsKey("status")
                    ? (string)errorDict["status"]
                    : "BadRequest",
                Error = errorDict.ContainsKey("errorCode")
                    ? (PlayFabErrorCode)Convert.ToInt32(errorDict["errorCode"])
                    : PlayFabErrorCode.ServiceUnavailable,
                ErrorMessage = errorDict.ContainsKey("errorMessage")
                    ? (string)errorDict["errorMessage"]
                    : string.Empty,
                ErrorDetails = errorDetails,
                CustomData = customData ?? new object()
            };
        }


    }
}