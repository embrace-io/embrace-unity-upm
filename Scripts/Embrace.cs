using UnityEngine;
using System.Collections.Generic;

namespace EmbraceSDK
{
    public interface IEmbraceProvider
    {
        // called automatically on awake
        // does not start any montitoring or network calls
        void InitializeSDK();
        // Public API
        void StareetSDK(bool enableIntegrationTesting);
        void EndAppStartup(Dictionary<string, string> properties);
        void SetUserIdentifier(string identifier);
        void ClearUserIdentifier();
        void SetUsername(string username);
        void ClearUsername();
        void SetUserEmail(string email);
        void ClearUserEmail();
        void SetUserAsPayer();
        void ClearUserAsPayer();
        void SetUserPersona(string persona);
        void ClearUserPersona(string persona);
        void ClearAllUserPersonas();
        void enableDebugLogging();
        bool AddSessionProperty(string key, string value, bool permanent);
        void RemoveSessionProperty(string key);
        Dictionary<string, string> GetSessionProperties();
        void StartMoment(string name, string identifier, bool allowScreenshot, Dictionary<string, string> properties);
        void EndMoment(string name, string identifier, Dictionary<string, string> properties);
        void LogMessage(string message, EMBSeverity severity, Dictionary<string, string> properties, bool allowScreenshot);
        void LogBreadcrumb(string message);
        void EndSession(bool clearUserInfo);
        string GetDeviceId();
        bool StartView(string name);
        bool EndView(string name);
        void Crash();
        void SetMetaData(string version, string guid);
    }

    public enum EMBSeverity
    {
        Info,
        Warning,
        Error
    }

    public class Embrace : MonoBehaviour
    {
        private static Embrace instance = null;

        public static Embrace Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Embrace>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject { name = "Embrace" };
                        instance = go.AddComponent<Embrace>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        private IEmbraceProvider provider;

        // Called by Unity runtime
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
#if UNITY_ANDROID && !UNITY_EDITOR
                provider = new Embrace_Android();
#elif UNITY_IOS && !UNITY_EDITOR
                provider = new Embrace_iOS();
#else
                provider = new Embrace_Editor();
#endif
                provider.InitializeSDK();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void NoNullsError()
        {
            Debug.LogError("null is not allowed through the Embrace SDK.");
        }

        public void StartSDK(bool enableIntegrationTesting = false)
        {
            provider.StartSDK(enableIntegrationTesting);
            provider.SetMetaData(Application.unityVersion, Application.buildGUID);
        }

        public void EndAppStartup(Dictionary<string, string> properties = null)
        {
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.EndAppStartup(properties);
        }

        public void SetUserIdentifier(string identifier)
        {
            if (identifier == null) { NoNullsError(); return; }
            provider.SetUserIdentifier(identifier);
        }

        public void ClearUserIdentifier()
        {
            provider.ClearUserIdentifier();
        }

        public void SetUsername(string username)
        {
            if (username == null) { NoNullsError(); return; }
            provider.SetUsername(username);
        }

        public void ClearUsername()
        {
            provider.ClearUsername();
        }

        public void SetUserEmail(string email)
        {
            if (email == null) { NoNullsError(); return; }
            provider.SetUserEmail(email);
        }

        public void ClearUserEmail()
        {
            provider.ClearUserEmail();
        }

        public void SetUserAsPayer()
        {
            provider.SetUserAsPayer();
        }

        public void ClearUserAsPayer()
        {
            provider.ClearUserAsPayer();
        }

        public void SetUserPersona(string persona)
        {
            if (persona == null) { NoNullsError(); return; }
            provider.SetUserPersona(persona);
        }

        public void ClearUserPersona(string persona)
        {
            if (persona == null) { NoNullsError(); return; }
            provider.ClearUserPersona(persona);
        }

        public void enableDebugLogging()
        {
            provider.enableDebugLogging();
        }

        public void ClearAllUserPersonas()
        {
            provider.ClearAllUserPersonas();
        }

        public void AddSessionProperty(string key, string value, bool permanent)
        {
            if (key == null || value == null) { NoNullsError(); return; }
            provider.AddSessionProperty(key, value, permanent);
        }

        public void RemoveSessionProperty(string key)
        {
            if (key == null) { NoNullsError(); return; }
            provider.RemoveSessionProperty(key);
        }

        public Dictionary<string, string> GetSessionProperties()
        {
            var properties = provider.GetSessionProperties();
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            return properties;
        }

        public void StartMoment(string name, string identifier = null, bool allowScreenshot = true, Dictionary<string, string> properties = null)
        {
            if (name == null) { NoNullsError(); return; }
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.StartMoment(name, identifier, allowScreenshot, properties);
        }

        public void EndMoment(string name, string identifier = null, Dictionary<string, string> properties = null)
        {
            if (name == null) { NoNullsError(); return; }
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.EndMoment(name, identifier, properties);
        }

        public void LogMessage(string message, EMBSeverity severity, Dictionary<string, string> properties = null, bool allowScreenshot = false)
        {
            if (message == null) { NoNullsError(); return; }
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.LogMessage(message, severity, properties, allowScreenshot);
        }

        public void LogBreadcrumb(string message)
        {
            if (message == null) { NoNullsError(); return; }
            provider.LogBreadcrumb(message);
        }

        public void EndSession(bool clearUserInfo = false)
        {
            provider.EndSession(clearUserInfo);
        }

        public string GetDeviceId()
        {
            return provider.GetDeviceId();
        }

        public bool StartView(string name)
        {
            if (name == null) { NoNullsError(); return false; }
            return provider.StartView(name);
        }

        public bool EndView(string name)
        {
            if (name == null) { NoNullsError(); return false; }
            return provider.EndView(name);
        }

        public void Crash()
        {
            provider.Crash();
        }
    }
}