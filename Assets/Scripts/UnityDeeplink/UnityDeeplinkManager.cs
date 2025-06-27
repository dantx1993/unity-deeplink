using System;
using System.Collections.Generic;
using TheArchitect.Attributes;
using TheArchitect.Unity;
using UnityEngine;

namespace UnityDeeplinkDemo
{
    
    [Resource("UnityDeeplink/UnityDeeplinkManager", isDontDestroyOnLoad: true)]
    public class UnityDeeplinkManager : Singleton<UnityDeeplinkManager>
    {
        private string _deeplinkUrl;
        private bool _isInitDeeplink = false;
        private static Action<string> s_onHandleDeeplink;
        private string _promoValue;

        public string DeeplinkUrl
        {
            get => _deeplinkUrl;
            private set => _deeplinkUrl = value;
        }

        private void Awake()
        {
            InitDeepLink();
        }

        protected override void Init()
        {
            base.Init();
            InitDeepLink();
        }

        private void InitDeepLink()
        {
            if (_isInitDeeplink)
            {
                return;
            }

            Debug.Log("[Deeplink Debug] - Init Deeplink");

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
            else DeeplinkUrl = "[none]";

            Application.deepLinkActivated += OnDeepLinkActivated;
            _isInitDeeplink = true;
        }

        public void OnDeepLinkActivated(string url)
        {
            DeeplinkUrl = url;
            Debug.Log("[Deeplink Debug] Short link received: " + url);
            _promoValue = GetParamUri(DeeplinkUrl);
            if (!string.IsNullOrEmpty(_promoValue))
            {
                s_onHandleDeeplink?.Invoke(_promoValue);
            }
        }

        private string GetParamUri(string deeplinkUrl)
        {
            string promoValue = null;
            Uri uri;
            try
            {
                uri = new Uri(deeplinkUrl);
            }
            catch (Exception e)
            {
                Debug.LogError("Invalid URI: " + e.Message);
                return promoValue;
            }

            string query = uri.Query;
            Dictionary<string, string> parameters = ParseQueryString(query);

            if (parameters.ContainsKey("p"))
            {
                promoValue = parameters["p"];
                Debug.Log("[Deeplink Debug] promo = " + promoValue);
                return promoValue;
            }
            else
            {
                Debug.Log($"[Deeplink Debug] Can't find 'p' in deeplink URL: {deeplinkUrl}");
                return null;
            }
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query))
                return result;
            if (query.StartsWith("?"))
                query = query.Substring(1);

            string[] pairs = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                string[] kv = pair.Split(new char[] { '=' }, 2);
                if (kv.Length == 2)
                {
                    string key = Uri.UnescapeDataString(kv[0]);
                    string value = Uri.UnescapeDataString(kv[1]);
                    result[key] = value;
                }
            }
            return result;
        }

        public static void RegisterDeeplinkHandler(Action<string> onHandleDeeplink)
        {
            s_onHandleDeeplink += onHandleDeeplink;
        }
        public static void RemoveDeeplinkHandler(Action<string> onHandleDeeplink)
        {
            s_onHandleDeeplink -= onHandleDeeplink;
        }
        public static void ClearDeeplinkHandler()
        {
            s_onHandleDeeplink = null;
        }
    }
}
