using System;
using UnityEngine;
using System.Collections;
using PlayFab.Editor;
using UnityEditor;
using UnityEngine.Experimental.Networking;
using PlayFab.Editor.Json;

public class PlayFabEditorHttp : Editor {
    public class PlayFabError
    {
        
    }

    internal static void MakeDownloadCall<TRequestType, TResultType>(string api, string apiEndpoint,
        TRequestType request,
        string authType,
        Action<TResultType> resultCallback, Action<PlayFabEditorHttp.PlayFabError> errorCallback)
    {
        
    }

    internal static void MakeApiCall<TRequestType, TResultType>(string api, string apiEndpoint, TRequestType request,
            string authType,
            Action<TResultType> resultCallback, Action<PlayFabEditorHttp.PlayFabError> errorCallback)
    {
        var req = PlayFab.SimpleJson.SerializeObject(request, PlayFabEditorUtil.ApiSerializerStrategy);
        //EditorCoroutine.start(Post(api));

    }

    private static IEnumerator Post(UnityWebRequest www, Action<string> callBack, Action<string> errorCallback)
    {
        yield return www.Send();

        if (www.isError)
        {
            errorCallback(www.error);
        }
        else
        {
            callBack(www.downloadHandler.text);
        }
    }

    private static IEnumerator Post(UnityWebRequest www, Action<byte[]> callBack, Action<string> errorCallback)
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



}
