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

        internal static void MakeDownloadCall<TRequestType, TResultType>(string url, TRequestType request, Action<TResultType> resultCallback, Action<EditorModels.PlayFabError> errorCallback)
        {
            //TODO 
        }

//        public IEnumerator DownloadAndUnpackAsset(AssetBundleHelperObject asset)
//        {
//            Debug.Log(Caching.IsVersionCached(asset.GetUrl, asset.Version));
//
//            // Start a download of the given URL
//            Debug.Log("HTTP GET:" + asset.ContentKey);
//            var www = WWW.LoadFromCacheOrDownload (asset.GetUrl, asset.Version);
//
//            // wait until the download is done
//            while(www.progress < 1)
//            {
//                asset.progress = www.progress;
//                yield return www;
//            }
//
//            if(string.IsNullOrEmpty(www.error))
//            {
//            
//                asset.Error = "";
//                asset.Unpacked.ContentKey = asset.ContentKey;
//                asset.Unpacked.PromoId = asset.FileName;
//
//                asset.Bundle = www.assetBundle;
//
//                string[] names = asset.Bundle.GetAllAssetNames();
//                //Debug.Log(names.ToString());
//
//                foreach (var name in names) 
//                {
//                    string bannerURI = string.Empty;
//                    string splashURI = string.Empty;
//
//                    Debug.Log ("Unpacking:" + name);
//                    if(name.Contains("banner.png") || name.Contains("Banner.png") || name.Contains("banner.jpg") || name.Contains("banner.jpg"))
//                    {
//                        bannerURI = name;
//                    }
//                    else if(name.Contains("splash.png") || name.Contains("Splash.png") || name.Contains("splash.jpg") || name.Contains("Splash.jpg"))
//                    {
//                        splashURI = name;
//                    }
//
//                    if(string.IsNullOrEmpty(bannerURI) == false)
//                    {
//                        Texture2D banner = asset.Bundle.LoadAsset<Texture2D>(bannerURI);
//                        asset.Unpacked.Banner = banner;
//                    }
//                    else if(string.IsNullOrEmpty(splashURI) == false)
//                    {
//                        Texture2D splash = asset.Bundle.LoadAsset<Texture2D>(splashURI);
//                        asset.Unpacked.Splash = splash;
//                    }
//                    else
//                    {
//                        asset.Error += string.Format("[Err: Unplacking: {0} -- {1} ]", asset.FileName, name);
//                    }
//                }
//
//                asset.Bundle.Unload(false);
//                asset.IsUnpacked = true;
//                //this.isImageDownloaded = true;
//                yield break;
//            }
//            else
//            {
//                asset.Error = www.error;
//                Debug.Log("HTTP ERROR:" + asset.ContentKey);
//            }
//
//            yield break;
//        }
//





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

        private static IEnumerator PostDownload(UnityWebRequest www, Action<byte[]> callBack, Action<string> errorCallback)
        {
            yield return www.Send();

            if (www.isError)
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.downloadHandler.data);
            }
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