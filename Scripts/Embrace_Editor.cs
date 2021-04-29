using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmbraceSDK
{
#if UNITY_EDITOR
    public class Embrace_Editor: IEmbraceProvider
    {
        void IEmbraceProvider.InitializeSDK()
        {
            Debug.Log("Embrace Unity SDK InitializeSDK");
        }

        void IEmbraceProvider.StartSDK(bool enableIntegrationTesting)
        {
            Debug.Log("Embrace Unity SDK: StartSDK");
        }

        void IEmbraceProvider.EndAppStartup(Dictionary<string, string> properties)
        {
            Debug.Log("Embrace Unity SDK: EndAppStartup");
        }

        void IEmbraceProvider.SetUserIdentifier(string identifier)
        {
            Debug.LogFormat("Embrace Unity SDK: SetUserIdentifier {0}", identifier);
        }

        void IEmbraceProvider.ClearUserIdentifier()
        {
            Debug.Log("Embrace Unity SDK: ClearUserIdentifier");
        }

        void IEmbraceProvider.SetUsername(string username)
        {
            Debug.LogFormat("Embrace Unity SDK: SetUsername {0}", username);
        }

        void IEmbraceProvider.ClearUsername()
        {
            Debug.Log("Embrace Unity SDK: ClearUsername");
        }

        void IEmbraceProvider.SetUserEmail(string email)
        {
            Debug.LogFormat("Embrace Unity SDK: SetUserEmail {0}", email);
        }

        void IEmbraceProvider.ClearUserEmail()
        {
            Debug.Log("Embrace Unity SDK: ClearUserEmail");
        }

        void IEmbraceProvider.SetUserAsPayer()
        {
            Debug.Log("Embrace Unity SDK: SetUserAsPayer");
        }

        void IEmbraceProvider.ClearUserAsPayer()
        {
            Debug.Log("Embrace Unity SDK: ClearUserAsPayer");
        }

        void IEmbraceProvider.SetUserPersona(string persona)
        {
            Debug.LogFormat("Embrace Unity SDK: SetUserPersona {0}", persona);
        }

        void IEmbraceProvider.ClearUserPersona(string persona)
        {
            Debug.LogFormat("Embrace Unity SDK: ClearUserPersona {0}", persona);
        }

        void IEmbraceProvider.ClearAllUserPersonas()
        {
            Debug.Log("Embrace Unity SDK: ClearAllUserPersonas");
        }

        bool IEmbraceProvider.AddSessionProperty(string key, string value, bool permanent)
        {
            Debug.LogFormat("Embrace Unity SDK: AddSessionProperty key: {0} value: {1}", key, value);
            return true;
        }

        void IEmbraceProvider.RemoveSessionProperty(string key)
        {
            Debug.LogFormat("Embrace Unity SDK: RemoveSessionProperty key: {0}", key);
        }

        Dictionary<string, string> IEmbraceProvider.GetSessionProperties()
        {
            Debug.Log("Embrace Unity SDK: GetSessionProperties");
            return new Dictionary<string, string>();
        }

        void IEmbraceProvider.StartMoment(string name, string identifier, bool allowScreenshot, Dictionary<string, string> properties)
        {
            Debug.LogFormat("Embrace Unity SDK: StartMoment {0}", name);
        }

        void IEmbraceProvider.EndMoment(string name, string identifier, Dictionary<string, string> properties)
        {
            Debug.LogFormat("Embrace Unity SDK: EndMoment {0}", name);
        }

        void IEmbraceProvider.LogMessage(string message, EMBSeverity severity, Dictionary<string, string> properties, bool allowScreenshot)
        {
            string severityString = "";

            switch (severity)
            {
                case EMBSeverity.Info:
                    severityString = "ïnfo";
                    break;
                case EMBSeverity.Warning:
                    severityString = "warning";
                    break;
                case EMBSeverity.Error:
                    severityString = "error";
                    break;
            }

            Debug.LogFormat("Embrace Unity SDK: LogMessage severity: {0} message: {1}", severityString, message);
        }

        void IEmbraceProvider.LogBreadcrumb(string message)
        {
            Debug.LogFormat("Embrace Unity SDK: LogBreadcrumb {0}", message);
        }

        void IEmbraceProvider.EndSession(bool clearUserInfo)
        {
            Debug.Log("Embrace Unity SDK: EndSession");
        }

        string IEmbraceProvider.GetDeviceId()
        {
            Debug.Log("Embrace Unity SDK: GetDeviceId");
            return "";
        }

        bool IEmbraceProvider.StartView(string name)
        {
            Debug.LogFormat("Embrace Unity SDK: StartView {0}", name);
            return true;
        }

        bool IEmbraceProvider.EndView(string name)
        {
            Debug.LogFormat("Embrace Unity SDK: EndView {0}", name);
            return true;
        }

        void IEmbraceProvider.Crash()
        {
            Debug.Log("Embrace Unity SDK: Crash");
        }

        void IEmbraceProvider.SetMetaData(string version, string guid)
        {
            Debug.LogFormat("Embrace Unity SDK: Unity Version = {0} GUID = {1}", version, guid);
        }

        public void enableDebugLogging()
        void IEmbraceProvider.LogNetworkRequest(string url, HTTPMethod method, long startms, long endms, int bytesin, int bytesout, int code, string error)
        {
            Debug.Log("Embrace Unity SDK: Manual Network Request: " + url + " method: " + method + " start: " + startms + " end: " + endms + " bytesin: " + bytesin + " bytesout: " + bytesout + " error: " + error);
        }

        {
            Debug.Log("Embrace now in debug logging mode");
        }

        void IEmbraceProvider.logUnhandledUnityException(string exceptionMessage, string stack)
        {
            Debug.Log("Embrace Unity SDK: Unhandled Exception: " + exceptionMessage + " : stack : " + stack);
        }
    }
#endif
}
