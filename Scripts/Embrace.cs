#define EMBRACE_USE_THREADING 
using UnityEngine;
using System.Collections.Generic;
#if EMBRACE_USE_THREADING
using System.Threading;
#endif

namespace EmbraceSDK
{
    public interface IEmbraceProvider
    {
        /// <summary>
        /// Called automatically on awake. Does not start any monitoring or network calls.
        /// </summary>
        void InitializeSDK();
        // Public API
        void StartSDK(bool enableIntegrationTesting);
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
        void LogNetworkRequest(string url, HTTPMethod method, long startms, long endms, int bytesin, int bytesout, int code, string error);
        void logUnhandledUnityException(string exceptionMessage, string stack);
        // only has effect on Android
        void EnableDebugLogging();
    }

    public enum HTTPMethod
    {
        GET = 0,
        POST,
        PUT,
        DELETE,
        PATCH,
        OTHER
    }

    public enum EMBSeverity
    {
        /// <summary>INFO severity level</summary>
        Info,
        /// <summary>WARNING severity level</summary>
        Warning,
        /// <summary>ERROR severity level</summary>
        Error
    }

    public class Embrace : MonoBehaviour
    {
        private static Embrace instance = null;
        private Thread mainThread;

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

        public void Start()
        {
            mainThread = Thread.CurrentThread;
        }

        private void NoNullsError()
        {
            Debug.LogError("null is not allowed through the Embrace SDK.");
        }

        /// <summary>
        /// Starts instrumentation of the application using the Embrace SDK. This should be called during creation of the application, as early as possible.
        /// See Embrace Docs for integration instructions. For compatibility with other SDKs, the Embrace SDK must be initialized after any other SDK.
        /// </summary>
        /// <param name="enableIntegrationTesting">If true, debug sessions (those which are not part of a release APK) will go to the live integration testing tab of the dashboard. If false, they will appear in 'recent sessions'.</param>
        public void StartSDK(bool enableIntegrationTesting = false)
        {
            provider.StartSDK(enableIntegrationTesting);
            provider.SetMetaData(Application.unityVersion, Application.buildGUID);
            Application.logMessageReceived += Embrace_Log_Handler;
#if EMBRACE_USE_THREADING
            Application.logMessageReceivedThreaded += Embrace_Threaded_Log_Handler;
#endif
        }

        bool isMainThread()
        {
            return mainThread.Equals(Thread.CurrentThread);
        }

#if EMBRACE_USE_THREADING
        void Embrace_Threaded_Log_Handler(string message, string stack, LogType type)
        {
            if (isMainThread())
            {
                return;
            }
            Embrace_Log_Handler(message, stack, type);
        }
#endif

        public void Embrace_Log_Handler(string message, string stack, LogType type)
        {

            if (type == LogType.Exception || type == LogType.Assert)
            {
                provider.logUnhandledUnityException(message, stack);
            }
        }

        /// <summary>
        /// Signals that the app has completed startup.
        /// </summary>
        /// <param name="properties">Properties to include as part of the startup moment</param>
        public void EndAppStartup(Dictionary<string, string> properties = null)
        {
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.EndAppStartup(properties);
        }

        /// <summary>
        /// Sets the user ID. This would typically be some form of unique identifier such as a UUID or database key for the user.
        /// </summary>
        /// <param name="identifier">the unique identifier for the user</param>
        public void SetUserIdentifier(string identifier)
        {
            if (identifier == null) { NoNullsError(); return; }
            provider.SetUserIdentifier(identifier);
        }

        public void ClearUserIdentifier()
        {
            provider.ClearUserIdentifier();
        }

        /// <summary>
        /// Sets the username of the currently logged in user.
        /// </summary>
        /// <param name="username">the username to set</param>
        public void SetUsername(string username)
        {
            if (username == null) { NoNullsError(); return; }
            provider.SetUsername(username);
        }

        /// <summary>
        /// Clears the username of the currently logged in user, for example if the user has logged out.
        /// </summary>
        public void ClearUsername()
        {
            provider.ClearUsername();
        }

        /// <summary>
        /// Sets the current user's email address.
        /// </summary>
        /// <param name="email">the email address of the current user</param>
        public void SetUserEmail(string email)
        {
            if (email == null) { NoNullsError(); return; }
            provider.SetUserEmail(email);
        }

        /// <summary>
        /// Clears the currently set user's email address.
        /// </summary>
        public void ClearUserEmail()
        {
            provider.ClearUserEmail();
        }

        /// <summary>
        /// Sets this user as a paying user. This adds a persona to the user's identity.
        /// </summary>
        public void SetUserAsPayer()
        {
            provider.SetUserAsPayer();
        }

        /// <summary>
        /// Clears this user as a paying user. This would typically be called if a user is no longer paying for the service and has reverted back to a basic user.
        /// </summary>
        public void ClearUserAsPayer()
        {
            provider.ClearUserAsPayer();
        }

        /// <summary>
        /// Sets a custom user persona. A persona is a trait associated with a given user.
        /// </summary>
        /// <param name="persona">the persona to set</param>
        public void SetUserPersona(string persona)
        {
            if (persona == null) { NoNullsError(); return; }
            provider.SetUserPersona(persona);
        }

        /// <summary>
        /// Clears the custom user persona, if it is set.
        /// </summary>
        /// <param name="persona">the persona to clear</param>
        public void ClearUserPersona(string persona)
        {
            if (persona == null) { NoNullsError(); return; }
            provider.ClearUserPersona(persona);
        }

        /// <summary>
        /// Clears all custom user personas from the user.
        /// </summary>
        public void ClearAllUserPersonas()
        {
            provider.ClearAllUserPersonas();
        }

        /// <summary>
        /// Annotates the session with a new property. Use this to track permanent and ephemeral features of the session. A permanent property is added to all sessions submitted from this device, use this for properties such as work site, building, owner. A non-permanent property is added to only the currently active session.
        /// There is a maximum of 10 total properties in a session.
        /// </summary>
        /// <param name="key">the key for this property, must be unique within session properties</param>
        /// <param name="value">the value to store for this property</param>
        /// <param name="permanent">if true the property is applied to all sessions going forward, persist through app launches.</param>
        public void AddSessionProperty(string key, string value, bool permanent)
        {
            if (key == null || value == null) { NoNullsError(); return; }
            provider.AddSessionProperty(key, value, permanent);
        }

        /// <summary>
        /// Removes a property from the session. If that property was permanent then it is removed from all future sessions as well.
        /// </summary>
        /// <param name="key">the key for the property you wish to remove</param>
        public void RemoveSessionProperty(string key)
        {
            if (key == null) { NoNullsError(); return; }
            provider.RemoveSessionProperty(key);
        }

        /// <summary>
        /// Get a read-only representation of the currently set session properties.
        /// </summary>
        /// <returns>the properties as key-value pairs of strings</returns>
        public Dictionary<string, string> GetSessionProperties()
        {
            var properties = provider.GetSessionProperties();
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            return properties;
        }

        /// <summary>
        /// Starts recording data for an app moment with the provided name, optional identifier, screenshot flag, and optional key/value metadata
        /// </summary>
        /// <param name="name">the name used to identify the moment</param>
        /// <param name="identifier">an identifier that is combined with the name to create a unique key for the moment (can be null)</param>
        /// <param name="allowScreenshot">a flag for whether to take a screenshot if the moment is late (defaults to true)</param>
        /// <param name="properties">an optional dictionary containing metadata about the moment to be recorded (limited to 10 keys)</param>
        public void StartMoment(string name, string identifier = null, bool allowScreenshot = true, Dictionary<string, string> properties = null)
        {
            if (name == null) { NoNullsError(); return; }
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.StartMoment(name, identifier, allowScreenshot, properties);
        }

        /// <summary>
        /// Stops recording data for an app moment with the provided name (and identifier), and adds properties to the moment.
        /// This marks the moment as “completed.” If no moment is found with the provided name (and an empty identifier), this call will be ignored. Additionally, if an app moment was started with a name and identifier, the same identifier must be used to end it.
        /// </summary>
        /// <param name="name">the name used to identify the moment</param>
        /// <param name="identifier">an identifier that is combined with the name to create a unique key for the moment (can be null)</param>
        /// <param name="properties">an optional dictionary containing metadata about the moment to be recorded (limited to 10 keys)</param>
        public void EndMoment(string name, string identifier = null, Dictionary<string, string> properties = null)
        {
            if (name == null) { NoNullsError(); return; }
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.EndMoment(name, identifier, properties);
        }

        /// <summary>
        /// Logs an event in your application for aggregation and debugging on the Embrace.io dashboard with an optional dictionary of up to 10 properties and the ability to enable or disable a screenshot.
        /// </summary>
        /// <param name="message">the name of the message, which is how it will show up on the dashboard</param>
        /// <param name="severity">will flag the message as one of info, warning, or error for filtering on the dashboard</param>
        /// <param name="properties">an optional dictionary of up to 10 key/value pairs</param>
        /// <param name="allowScreenshot">a flag for whether the SDK should take a screenshot of the application window to display on the dashboard (defaults to false)</param>
        public void LogMessage(string message, EMBSeverity severity, Dictionary<string, string> properties = null, bool allowScreenshot = false)
        {
            if (message == null) { NoNullsError(); return; }
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }
            provider.LogMessage(message, severity, properties, allowScreenshot);
        }

        /// <summary>
        /// Logs a breadcrumb.
        /// Breadcrumbs track a user's journey through the application and will be shown on the timeline.
        /// </summary>
        /// <param name="message">the name of the breadcrumb to log</param>
        public void LogBreadcrumb(string message)
        {
            if (message == null) { NoNullsError(); return; }
            provider.LogBreadcrumb(message);
        }

        /// <summary>
        /// Ends the current session and starts a new one.
        /// </summary>
        /// <param name="clearUserInfo">if true, clears all the user info on the device</param>
        public void EndSession(bool clearUserInfo = false)
        {
            provider.EndSession(clearUserInfo);
        }

        /// <summary>
        /// Get the user identifier assigned to the device by Embrace.
        /// </summary>
        /// <returns>the device identifier created by Embrace</returns>
        public string GetDeviceId()
        {
            return provider.GetDeviceId();
        }

        /// <summary>
        /// Opens a view. There is a limit to 10 "started" views.
        /// </summary>
        /// <param name="name">name of the view state as it will show up on our dashboard</param>
        /// <returns>a boolean indicating whether the operation was successful or not</returns>
        public bool StartView(string name)
        {
            if (name == null) { NoNullsError(); return false; }
            return provider.StartView(name);
        }

        /// <summary>
        /// Closes the view state for the specified view or logs a warning if the view is not found.
        /// </summary>
        /// <param name="name">name of the view state</param>
        /// <returns>a boolean indicating whether the operation was successful or not</returns>
        public bool EndView(string name)
        {
            if (name == null) { NoNullsError(); return false; }
            return provider.EndView(name);
        }

        /// <summary>
        /// Causes a crash. Use this for test purposes only.
        /// </summary>
        public void Crash()
        {
            provider.Crash();
        }

        public void LogNetworkRequest(string url, HTTPMethod method, long startms, long endms, int bytesin, int bytesout, int code, string error)
        {
            if (url == null) { NoNullsError(); return; }
            if (error == null) { NoNullsError(); return; }
            provider.LogNetworkRequest(url, method, startms, endms, bytesin, bytesout, code, error);
        }

        /// <summary>
        /// Enables debug logging.
        /// </summary>
        public void EnableDebugLogging()
        {
            provider.EnableDebugLogging();
        }

        public void logUnhandledUnityException(string exceptionMessage, string stack)
        {
            if (exceptionMessage == null) { NoNullsError(); return; }
            if (stack == null) { NoNullsError(); return; }
            provider.logUnhandledUnityException(exceptionMessage, stack);
        }

        public static int __BridgedHTTPMethod(HTTPMethod method)
        {
            switch (method)
            {
                case HTTPMethod.GET: return 1;
                case HTTPMethod.POST: return 2;
                case HTTPMethod.PUT: return 3;
                case HTTPMethod.DELETE: return 4;
                case HTTPMethod.PATCH: return 5;
                default: return 0;
            }
        }
    }
}