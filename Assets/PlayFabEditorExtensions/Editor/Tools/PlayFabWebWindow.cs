namespace PlayFab.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Reflection;

    public class PlayFabWebWindow : EditorWindow
    {
        public static string Url = "https://developer.playfab.com/";
        private static PlayFabWebWindow window;

        public static void OpenWindow(string url = null)
        {
            window = PlayFabWebWindow.GetWindow<PlayFabWebWindow>();
            window.minSize = new Vector2(1024, 768);
            window.maxSize = new Vector2(1024, 768);
            window.maximized = false;
            window.Show();
            OpenWebView(window, url);
        }

        static void OpenWebView(PlayFabWebWindow window, string url = null)
        {
            var thisWindowGuiView =
                typeof (EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(window);

            Type webViewType = GetTypeFromAllAssemblies("WebView");
            var webView = ScriptableObject.CreateInstance(webViewType);

            Rect webViewRect = new Rect(0, 24, 1024, window.position.height);
            webViewType.GetMethod("InitWebView")
                .Invoke(webView,
                    new object[]
                    {
                        thisWindowGuiView, (int) webViewRect.x, (int) webViewRect.y, (int) webViewRect.width,
                        (int) webViewRect.height, true
                    });
            webViewType.GetMethod("LoadURL").Invoke(webView, new object[] {string.IsNullOrEmpty(url) ? Url : url});
        }

        void OnGUI()
        {
            window.maximized = false;
        }

        public static Type GetTypeFromAllAssemblies(string typeName)
        {
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase) ||
                        type.Name.Contains('+' + typeName)) //+ check for inline classes
                        return type;
                }
            }
            return null;
        }
    }

}