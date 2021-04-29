using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmbraceSDK
{
#if UNITY_IOS
    public class Embrace_iOS : IEmbraceProvider
    {
        [DllImport("__Internal")]
        private static extern void embrace_sdk_start();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_endAppStartup(string properties);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_setUserIdentifier(string identifier);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_clearUserIdentifier();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_setUsername(string username);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_clearUsername();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_setUserEmail(string email);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_clearUserEmail();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_setUserAsPayer();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_clearUserAsPayer();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_setUserPersona(string persona);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_clearUserPersona(string persona);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_clearAllUserPersonas();

        [DllImport("__Internal")]
        private static extern bool embrace_sdk_addSessionProperty(string key, string value, bool permanent);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_removeSessionProperty(string key);

        [DllImport("__Internal")]
        private static extern string embrace_sdk_getSessionProperties();

        [DllImport("__Internal")]
        private static extern void embrace_sdk_startMoment(string name, string identifier, bool allowScreenshot, string properties);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_endMoment(string name, string identifier, string properties);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_logMessage(string message, string severity, string properties, bool allowScreenshot);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_logBreadcrumb(string message);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_endSession(bool clearUserInfo);

        [DllImport("__Internal")]
        private static extern string embrace_sdk_getDeviceId();

        [DllImport("__Internal")]
        private static extern bool embrace_sdk_startView(string name);

        [DllImport("__Internal")]
        private static extern bool embrace_sdk_endView(string name);

        [DllImport("__Internal")]
        private static extern bool embrace_sdk_crash();

        [DllImport("__Internal")]
        private static extern bool embrace_sdk_setUnityMetaData(string version, string guid);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_logNetworkRequest(string url, int method, long startms, long endms, int bytesin, int bytesout, int code, string error);

        [DllImport("__Internal")]
        private static extern void embrace_sdk_logUnhandledUnityException(string message, string stacktrace);

        void IEmbraceProvider.InitializeSDK()
        {
            Debug.Log("Embrace Unity SDK initializing Objc objects");
        }

        void IEmbraceProvider.StartSDK(bool enableIntegrationTesting)
        {
            embrace_sdk_start();
        }

        void IEmbraceProvider.EndAppStartup(Dictionary<string, string> properties)
        {
            embrace_sdk_endAppStartup(DictionaryToJson(properties));
        }

        void IEmbraceProvider.SetUserIdentifier(string identifier)
        {
            embrace_sdk_setUserIdentifier(identifier);
        }

        void IEmbraceProvider.ClearUserIdentifier()
        {
            embrace_sdk_clearUserIdentifier();
        }

        void IEmbraceProvider.SetUsername(string username)
        {
            embrace_sdk_setUsername(username);
        }

        void IEmbraceProvider.ClearUsername()
        {
            embrace_sdk_clearUsername();
        }

        void IEmbraceProvider.SetUserEmail(string email)
        {
            embrace_sdk_setUserEmail(email);
        }

        void IEmbraceProvider.ClearUserEmail()
        {
            embrace_sdk_clearUserEmail();
        }

        void IEmbraceProvider.SetUserAsPayer()
        {
            embrace_sdk_setUserAsPayer();
        }

        void IEmbraceProvider.ClearUserAsPayer()
        {
            embrace_sdk_clearUserAsPayer();
        }
        
        void IEmbraceProvider.SetUserPersona(string persona)
        {
            embrace_sdk_setUserPersona(persona);
        }
    
        void IEmbraceProvider.ClearUserPersona(string persona)
        {
            embrace_sdk_clearUserPersona(persona);
        }

        void IEmbraceProvider.ClearAllUserPersonas()
        {
            embrace_sdk_clearAllUserPersonas();
        }
            
        bool IEmbraceProvider.AddSessionProperty(string key, string value, bool permanent)
        {
            return embrace_sdk_addSessionProperty(key, value, permanent);
        }

        void IEmbraceProvider.RemoveSessionProperty(string key)
        {
            embrace_sdk_removeSessionProperty(key);
        }
        
        Dictionary<string, string> IEmbraceProvider.GetSessionProperties()
        {
            return JsonToDictionary(embrace_sdk_getSessionProperties());
        }
        
        void IEmbraceProvider.StartMoment(string name, string identifier, bool allowScreenshot, Dictionary<string, string> properties)
        {
            embrace_sdk_startMoment(name, identifier, allowScreenshot, DictionaryToJson(properties));
      
        }

        void IEmbraceProvider.EndMoment(string name, string identifier, Dictionary<string, string> properties)
        {
            embrace_sdk_endMoment(name, identifier, DictionaryToJson(properties));
        }

        void IEmbraceProvider.LogMessage(string message, EMBSeverity severity, Dictionary<string, string> properties, bool allowScreenshot)
        {
            string severityString = "";

            switch (severity)
            {
                case EMBSeverity.Info:
                    severityString = "info";
                    break;
                case EMBSeverity.Warning:
                    severityString = "warning";
                    break;
                case EMBSeverity.Error:
                    severityString = "error";
                    break;
            }

            embrace_sdk_logMessage(message, severityString, DictionaryToJson(properties), allowScreenshot);
        }

        void IEmbraceProvider.LogBreadcrumb(string message)
        {
            embrace_sdk_logBreadcrumb(message);
        }

        void IEmbraceProvider.EndSession(bool clearUserInfo)
        {
            embrace_sdk_endSession(clearUserInfo);
        }
            
        string IEmbraceProvider.GetDeviceId()
        {
            return embrace_sdk_getDeviceId();
        }

        bool IEmbraceProvider.StartView(string name)
        {
            return embrace_sdk_startView(name);
        }
    
        bool IEmbraceProvider.EndView(string name)
        {
            return embrace_sdk_endView(name);
        }

        void IEmbraceProvider.Crash()
        {
            embrace_sdk_crash();
        }

        void IEmbraceProvider.SetMetaData(string version, string guid)
        {
            embrace_sdk_setUnityMetaData(version, guid);
        }

        private string DictionaryToJson(Dictionary<string, string> dictionary)
        {   
            var kvs = dictionary.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, kvp.Value));
            return string.Concat("{", string.Join(",", kvs), "}");
        }

        private Dictionary<string, string> JsonToDictionary(string json)
        {
            string[] kva = json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');
            return kva.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
        }

        void IEmbraceProvider.LogNetworkRequest(string url, HTTPMethod method, long startms, long endms, int bytesin, int bytesout, int code, string error)
        {
            embrace_sdk_logNetworkRequest(url, Embrace.__BridgedHTTPMethod(method), startms, endms, bytesin, bytesout, code, error);
        }

        void IEmbraceProvider.EnableDebugLogging()
        {
            Debug.Log("Embrace Unity SDK: EnableDebugLogging not supported on iOS yet.");
        }

        void IEmbraceProvider.logUnhandledUnityException(string exceptionMessage, string stack)
        {
            embrace_sdk_logUnhandledUnityException(exceptionMessage, stack);

        }
    }
#endif
}
