using System;
using UnityEngine.Android;
using UnityEngine;
using System.Collections.Generic;

namespace EmbraceSDK
{
#if UNITY_ANDROID
    public class Embrace_Android : IEmbraceProvider
    {
        private AndroidJavaObject embraceSharedInstance;
        private AndroidJavaObject applicationInstance;
        private AndroidJavaObject unityAppFramework;
        private AndroidJavaClass embraceClass;

        private const string _StartMethod = "start";
        private const string _SetUserIdentifierMethod = "setUserIdentifier";
        private const string _ClearUserIdentifierMethod = "clearUserIdentifier";
        private const string _SetUserEmailMethod = "setUserEmail";
        private const string _ClearUserEmailMethod = "clearUserEmail";
        private const string _SetUserAsPayerMethod = "setUserAsPayer";
        private const string _ClearUserAsPayerMethod = "clearUserAsPayer";
        private const string _SetUserPersonaMethod = "setUserPersona";
        private const string _ClearUserPersonaMethod = "clearUserPersona";
        private const string _ClearAllUserPersonasMethod = "clearAllUserPersonas";
        private const string _AddSessionPropertyMethod = "addSessionProperty";
        private const string _RemoveSessionPropertyMethod = "removeSessionProperty";
        private const string _GetSessionPropertiesMethod = "getSessionProperties";
        private const string _SetUsernameMethod = "setUsername";
        private const string _ClearUsernameMethod = "clearUsername";
        private const string _StartEventMethod = "startEvent";
        private const string _EndEventMethod = "endEvent";
        private const string _EndAppStartupMethod = "endAppStartup";
        private const string _LogInfoMethod = "logInfo";
        private const string _LogWarningMethod = "logWarning";
        private const string _LogErrorMethod = "logError";
        private const string _LogBreadcrumbMethod = "logBreadcrumb";
        private const string _EndSessionMethod = "endSession";
        private const string _GetDeviceIdMethod = "getDeviceId";
        private const string _StartFragmentMethod = "startFragment";
        private const string _EndFragmentMethod = "endFragment";
        private const string _ThrowExceptionMethod = "throwException";
        private const string _SetUnityMetaDataMethod = "setUnityMetaData";
        private const string _LogNetworkRequestMethod = "logNetworkRequest";
        private const string _enableDebugLoggingMethod = "enableDebugLogging";
        private const string _logUnhandledUnityExceptionMethod = "logUnhandledUnityException";

        // Java Map Reading
        IntPtr CollectionIterator;
        IntPtr MapEntrySet;
        IntPtr IteratorHasNext;
        IntPtr IteratorNext;
        IntPtr MapEntryGetKey;
        IntPtr MapEntryGetValue;
        IntPtr ObjectToString;

        // we need some jni pointers to read java maps, it is best to grab them once and cache them
        // these are just pointers to the methods, not actual objects.
        private void CacheJavaMapPointers()
        {
            IntPtr collectionRef = AndroidJNI.FindClass("java/util/Collection");
            IntPtr CollectionClass = AndroidJNI.NewGlobalRef(collectionRef);
            IntPtr mapRef = AndroidJNI.FindClass("java/util/Map");
            IntPtr MapClass = AndroidJNI.NewGlobalRef(mapRef);
            CollectionIterator = AndroidJNI.GetMethodID(CollectionClass, "iterator", "()Ljava/util/Iterator;");
            MapEntrySet = AndroidJNI.GetMethodID(MapClass, "entrySet", "()Ljava/util/Set;");
            IntPtr iterRef = AndroidJNI.FindClass("java/util/Iterator");
            IntPtr IteratorClass = AndroidJNI.NewGlobalRef(iterRef);
            IteratorHasNext = AndroidJNI.GetMethodID(IteratorClass, "hasNext", "()Z");
            IteratorNext = AndroidJNI.GetMethodID(IteratorClass, "next", "()Ljava/lang/Object;");
            IntPtr entryRef = AndroidJNI.FindClass("java/util/Map$Entry");
            IntPtr MapEntryClass = AndroidJNI.NewGlobalRef(entryRef);
            MapEntryGetKey = AndroidJNI.GetMethodID(MapEntryClass, "getKey", "()Ljava/lang/Object;");
            MapEntryGetValue = AndroidJNI.GetMethodID(MapEntryClass, "getValue", "()Ljava/lang/Object;");
            IntPtr objectRef = AndroidJNI.FindClass("java/lang/Object");
            ObjectToString = AndroidJNI.GetMethodID(objectRef, "toString", "()Ljava/lang/String;");
        }

        private bool ReadyForCalls()
        {
            bool result = true;
            if (embraceSharedInstance == null)
            {
                Debug.LogError("Embrace Unity SDK did not initialize, ensure the prefab is added to the scene.");
                result = false;
            }
            return result;
        }

        void IEmbraceProvider.InitializeSDK()
        {
            Debug.Log("Embrace Unity SDK initializing java objects");
            CacheJavaMapPointers();
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            applicationInstance = activityInstance.Call<AndroidJavaObject>("getApplication");
            embraceClass = new AndroidJavaClass("io.embrace.android.embracesdk.Embrace");
            embraceSharedInstance = embraceClass.CallStatic<AndroidJavaObject>("getInstance");
            AndroidJavaClass appFramework = new AndroidJavaClass("io.embrace.android.embracesdk.Embrace$AppFramework");
            unityAppFramework = appFramework.GetStatic<AndroidJavaObject>("UNITY");
        }

        void IEmbraceProvider.StartSDK(bool enableIntegrationTesting)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_StartMethod, applicationInstance, enableIntegrationTesting, unityAppFramework);
        }

        void IEmbraceProvider.EndAppStartup(Dictionary<string, string> properties)
        {
            if (!ReadyForCalls()) { return; }
            AndroidJavaObject javaMap = DictionaryToJavaMap(properties);
            embraceSharedInstance.Call(_EndAppStartupMethod, javaMap);
        }

        void IEmbraceProvider.SetUserIdentifier(string identifier)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_SetUserIdentifierMethod, identifier);
        }

        void IEmbraceProvider.ClearUserIdentifier()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ClearUserIdentifierMethod);
        }

        void IEmbraceProvider.SetUsername(string username)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_SetUsernameMethod, username);
        }

        void IEmbraceProvider.ClearUsername()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ClearUsernameMethod);
        }

        void IEmbraceProvider.SetUserEmail(string email)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_SetUserEmailMethod, email);
        }

        void IEmbraceProvider.ClearUserEmail()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ClearUserEmailMethod);
        }

        void IEmbraceProvider.SetUserAsPayer()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_SetUserAsPayerMethod);
        }

        void IEmbraceProvider.ClearUserAsPayer()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ClearUserAsPayerMethod);
        }

        void IEmbraceProvider.SetUserPersona(string persona)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_SetUserPersonaMethod, persona);
        }

        void IEmbraceProvider.ClearUserPersona(string persona)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ClearUserPersonaMethod, persona);
        }

        void IEmbraceProvider.ClearAllUserPersonas()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ClearAllUserPersonasMethod);
        }

        bool IEmbraceProvider.AddSessionProperty(string key, string value, bool permanent)
        {
            if (!ReadyForCalls()) { return false; }
            return embraceSharedInstance.Call<bool>(_AddSessionPropertyMethod, key, value, permanent);
        }

        void IEmbraceProvider.RemoveSessionProperty(string key)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call<bool>(_RemoveSessionPropertyMethod, key);
        }

        Dictionary<string, string> IEmbraceProvider.GetSessionProperties()
        {
            if (!ReadyForCalls()) { return null; }
            AndroidJavaObject javaMap = embraceSharedInstance.Call<AndroidJavaObject>(_GetSessionPropertiesMethod);
            Dictionary<string, string> dictionary = DictionaryFromJavaMap(javaMap.GetRawObject());
            return dictionary;
        }

        void IEmbraceProvider.StartMoment(string name, string identifier, bool allowScreenshot, Dictionary<string, string> properties)
        {
            if (!ReadyForCalls()) { return; }
            AndroidJavaObject javaMap = DictionaryToJavaMap(properties);
            embraceSharedInstance.Call(_StartEventMethod, name, identifier, allowScreenshot, javaMap);
        }

        void IEmbraceProvider.EndMoment(string name, string identifier, Dictionary<string, string> properties)
        {
            if (!ReadyForCalls()) { return; }
            AndroidJavaObject javaMap = DictionaryToJavaMap(properties);
            embraceSharedInstance.Call(_EndEventMethod, name, identifier, javaMap);
        }

        void IEmbraceProvider.LogMessage(string message, EMBSeverity severity, Dictionary<string, string> properties, bool allowScreenshot)
        {
            if (!ReadyForCalls()) { return; }
            AndroidJavaObject javaMap = DictionaryToJavaMap(properties);

            switch (severity)
            {
                case EMBSeverity.Info:
                    embraceSharedInstance.Call(_LogInfoMethod, message, javaMap);
                    break;
                case EMBSeverity.Warning:
                    embraceSharedInstance.Call(_LogWarningMethod, message, javaMap, allowScreenshot);
                    break;
                case EMBSeverity.Error:
                    embraceSharedInstance.Call(_LogErrorMethod, message, javaMap, allowScreenshot);
                    break;
            }
        }

        void IEmbraceProvider.LogBreadcrumb(string message)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_LogBreadcrumbMethod, message);
        }

        void IEmbraceProvider.EndSession(bool clearUserInfo)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_EndSessionMethod, clearUserInfo);
        }

        string IEmbraceProvider.GetDeviceId()
        {
            if (!ReadyForCalls()) { return null; }
            return embraceSharedInstance.Call<string>(_GetDeviceIdMethod);
        }

        bool IEmbraceProvider.StartView(string name)
        {
            if (!ReadyForCalls()) { return false; }
            return embraceSharedInstance.Call<bool>(_StartFragmentMethod, name);
        }

        bool IEmbraceProvider.EndView(string name)
        {
            if (!ReadyForCalls()) { return false; }
            return embraceSharedInstance.Call<bool>(_EndFragmentMethod, name);
        }

        void IEmbraceProvider.Crash()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_ThrowExceptionMethod);
        }

        void IEmbraceProvider.SetMetaData(string version, string guid)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_SetUnityMetaDataMethod, version, guid);
        }

        void IEmbraceProvider.LogNetworkRequest(string url, HTTPMethod method, long startms, long endms, int bytesin, int bytesout, int code, string error)
        {
            if (!ReadyForCalls()) { return; }
            Debug.Log("Embrace Unity SDK: Manual Network Request: " + url + " method: " + method + " start: " + startms + " end: " + endms + " bytesin: " + bytesin + " bytesout: " + bytesout + " error: " + error);
            embraceSharedInstance.Call(_LogNetworkRequestMethod, url, Embrace.__BridgedHTTPMethod(method), startms, endms, bytesout, bytesin, code, error);
        }

        void IEmbraceProvider.EnableDebugLogging()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_enableDebugLoggingMethod);
        }

        void IEmbraceProvider.logUnhandledUnityException(string exceptionMessage, string stack)
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_logUnhandledUnityExceptionMethod, exceptionMessage, stack);
        }

        private AndroidJavaObject DictionaryToJavaMap(Dictionary<string, string> dictionary)
        {
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            IntPtr putMethod = AndroidJNIHelper.GetMethodID(map.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
            foreach (var entry in dictionary)
            {
                AndroidJNI.CallObjectMethod(
                    map.GetRawObject(),
                    putMethod,
                    AndroidJNIHelper.CreateJNIArgArray(new object[] { entry.Key, entry.Value })
                );
            }
            return map;
        }

        private Dictionary<string, string> DictionaryFromJavaMap(IntPtr source)
        {
            var dict = new Dictionary<string, string>();

            IntPtr entries = AndroidJNI.CallObjectMethod(source, MapEntrySet, new jvalue[] { });
            IntPtr iterator = AndroidJNI.CallObjectMethod(entries, CollectionIterator, new jvalue[] { });
            AndroidJNI.DeleteLocalRef(entries);

            while (AndroidJNI.CallBooleanMethod(iterator, IteratorHasNext, new jvalue[] { }))
            {
                IntPtr entry = AndroidJNI.CallObjectMethod(iterator, IteratorNext, new jvalue[] { });
                string key = AndroidJNI.CallStringMethod(entry, MapEntryGetKey, new jvalue[] { });
                IntPtr value = AndroidJNI.CallObjectMethod(entry, MapEntryGetValue, new jvalue[] { });
                AndroidJNI.DeleteLocalRef(entry);

                if (value != null && value != IntPtr.Zero)
                {
                    dict.Add(key, AndroidJNI.CallStringMethod(value, ObjectToString, new jvalue[] { }));
                }
                AndroidJNI.DeleteLocalRef(value);
            }
            AndroidJNI.DeleteLocalRef(iterator);

            return dict;
        }

        public void enableDebugLogging()
        {
            if (!ReadyForCalls()) { return; }
            embraceSharedInstance.Call(_enableDebugLoggingMethod);
        }
    }
#endif
}
